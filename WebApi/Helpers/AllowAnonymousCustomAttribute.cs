using System;

namespace WebApi.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousCustomAttribute : Attribute
    { }
}
