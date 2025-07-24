using System;

namespace RuntimeSceneInjector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NullableAttribute : Attribute
    {
    }
}