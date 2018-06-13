using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Shared.Util.Extension
{
    public static class TypeExtension
    {
        public static Boolean IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$") || type.Name.Contains("AnonymousType"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}