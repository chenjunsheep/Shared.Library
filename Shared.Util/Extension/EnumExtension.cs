using System;

namespace Shared.Util.Extension
{
    public static class EnumExtension
    {
        public static string ConvertToString(this Enum val)
        {
            return Enum.GetName(val.GetType(), val);
        }

        public static T ConvertToEnum<T>(this string val)
        {
            return (T)Enum.Parse(typeof(T), val, true);
        }
    }
}
