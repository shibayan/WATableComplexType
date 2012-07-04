using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebRole1
{
    internal static class TypeHelpers
    {
        public static object Create(Type type)
        {
            return Activator.CreateInstance(IsCollection(type) ? typeof(List<>).MakeGenericType(type.GetGenericArguments()) : type);
        }

        public static bool IsComplexType(Type type)
        {
            return !TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }

        public static bool IsCollection(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(List<>))
                {
                    return true;
                }

                if (genericTypeDefinition == typeof(IEnumerable<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IList<>))
                {
                    return true;
                }
            }

            return false;
        }
    }
}