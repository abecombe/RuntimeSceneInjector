using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RuntimeSceneInjector
{
    [DefaultExecutionOrder(-9999)]
    public class InterfaceInjector : MonoBehaviour, IInterfaceInjector
    {
        [Header("Settings")]
        [SerializeField] private bool _injectOnAwake = true;

        private static readonly HashSet<Type> SupportedCollectionTypes = new()
        {
            typeof(List<>),
            typeof(IList<>),
            typeof(IReadOnlyList<>),
            typeof(ICollection<>),
            typeof(IReadOnlyCollection<>),
            typeof(ISet<>),
            typeof(IEnumerable<>)
        };

        private void Awake()
        {
            if (_injectOnAwake)
            {
                ExecuteInjection();
            }
        }

        public void ExecuteInjection()
        {
            var typedLookUp = FindObjectsByType<Component>(FindObjectsSortMode.None)
                .SelectMany(component => component.GetType().GetInterfaces(), (component, iface) => (iface, component))
                .ToLookup(pair => pair.iface, pair => pair.component);

            var allMonobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            foreach (var behaviour in allMonobehaviours)
            {
                TryFieldInjection(behaviour, typedLookUp);
                TryMethodInjection(behaviour, typedLookUp);
            }
        }

        private static void TryFieldInjection(MonoBehaviour behaviour, ILookup<Type, Component> typedLookUp)
        {
            var currentType = behaviour.GetType();

            while (currentType != null && currentType != typeof(MonoBehaviour))
            {
                var fields = currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                foreach (var field in fields)
                {
                    if (!field.IsDefined(typeof(InjectAttribute), true)) continue;

                    bool isNullable = field.IsDefined(typeof(NullableAttribute), true);
                    object value = ResolveArgument(field.FieldType, typedLookUp);

                    if (value != null)
                    {
                        field.SetValue(behaviour, value);
                    }
                    else if (!isNullable)
                    {
                        LogError(behaviour, $"Could not resolve required dependency for field '{field.Name}' ({field.FieldType.Name}).");
                    }
                }
                currentType = currentType.BaseType;
            }
        }

        private static void TryMethodInjection(MonoBehaviour behaviour, ILookup<Type, Component> typedLookUp)
        {
            var allInjectMethods = new List<MethodInfo>();
            var currentType = behaviour.GetType();

            while (currentType != null && currentType != typeof(MonoBehaviour))
            {
                var methodsOnThisLevel = currentType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(m => m.IsDefined(typeof(InjectAttribute), true));
                allInjectMethods.AddRange(methodsOnThisLevel);
                currentType = currentType.BaseType;
            }

            var sortedInjectMethods = allInjectMethods.OrderByDescending(m => GetInheritanceDepth(m.DeclaringType));

            foreach (var injectMethod in sortedInjectMethods)
            {
                var parameters = injectMethod.GetParameters();
                var arguments = new object[parameters.Length];
                bool allParamsResolved = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    bool isNullable = param.IsDefined(typeof(NullableAttribute), true);
                    object arg = ResolveArgument(param.ParameterType, typedLookUp);
                    arguments[i] = arg;

                    if (arg == null && !isNullable)
                    {
                        LogError(behaviour, $"Could not resolve required dependency for parameter '{param.Name}' ({param.ParameterType.Name}). Aborting call to method '{injectMethod.Name}'.");
                        allParamsResolved = false;
                        break;
                    }
                }

                if (allParamsResolved)
                {
                    injectMethod.Invoke(behaviour, arguments);
                }
            }
        }

        private static int GetInheritanceDepth(Type type)
        {
            int depth = 0;
            var current = type;
            while (current != null && current != typeof(MonoBehaviour))
            {
                depth++;
                current = current.BaseType;
            }
            return depth;
        }

        private static object ResolveArgument(Type argumentType, ILookup<Type, Component> typedLookUp)
        {
            Type elementType = GetCollectionElementType(argumentType);
            if (elementType is { IsInterface: true })
            {
                var implementations = typedLookUp[elementType].ToList();
                return CreateCollection(argumentType, elementType, implementations);
            }

            return argumentType.IsInterface ? typedLookUp[argumentType].FirstOrDefault() : null;
        }

        private static Type GetCollectionElementType(Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                if (SupportedCollectionTypes.Contains(genericTypeDef))
                {
                    return type.GetGenericArguments()[0];
                }
            }
            return null;
        }

        private static object CreateCollection(Type collectionType, Type elementType, List<Component> implementations)
        {
            if (collectionType.IsArray)
            {
                var array = Array.CreateInstance(elementType, implementations.Count);
                for (int i = 0; i < implementations.Count; i++) array.SetValue(implementations[i], i);
                return array;
            }
            else
            {
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                foreach (var impl in implementations) list.Add(impl);
                return list;
            }
        }

        private static void LogError(MonoBehaviour behaviour, string message)
        {
            Debug.LogError($"[RuntimeSceneInjector] {behaviour.GetType().Name} ({behaviour.name}): {message}");
        }
    }
}