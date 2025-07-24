using System;

namespace RuntimeSceneInjector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }
}