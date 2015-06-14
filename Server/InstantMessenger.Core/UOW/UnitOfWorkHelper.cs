using System;
using System.Linq;
using System.Reflection;
using InstantMessenger.Core.Base;

namespace InstantMessenger.Core.UOW
{
    public static class UnitOfWorkHelper
    {
        public static bool HasAttribute(MethodInfo methodInfo, Type attribute)
        {
            return Attribute.IsDefined(methodInfo, attribute);
        }

        public static bool IsRepositoryMethod(MethodInfo methodInfo)
        {
            return IsRepositoryClass(methodInfo.DeclaringType);
        }

        public static bool IsRepositoryClass(Type type)
        {
            return type.IsGenericType &&
                   type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>));
        }
    }
}
