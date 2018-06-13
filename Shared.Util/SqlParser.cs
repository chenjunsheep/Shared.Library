using System;

namespace Shared.Util
{
    public class SqlParser
    {
        public static object GetInt(object val, object def = null)
        {
            var tmp = TypeParser.GetInt(val);
            if (tmp.HasValue)
            {
                return tmp.Value;
            }
            else
            {
                if (def == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    return def;
                }
            }
        }

        public static object GetDouble(object val, object def = null)
        {
            var tmp = TypeParser.GetDouble(val);
            if (tmp.HasValue)
            {
                return tmp.Value;
            }
            else
            {
                if (def == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    return def;
                }
            }
        }

        public static object GetString(object val, object def = null)
        {
            var tmp = TypeParser.GetStringValue(val);
            if (!string.IsNullOrEmpty(tmp))
            {
                return tmp;
            }
            else
            {
                if (def == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    return def;
                }
            }
        }

        public static object GetDateTimeValueExact(object val, string partten, object def = null)
        {
            var tmp = TypeParser.GetDateTimeValueExact(val, partten);
            if (tmp.HasValue)
            {
                return tmp.Value;
            }
            else
            {
                if (def == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    return def;
                }
            }
        }

        public static object GetBool(object val, bool def = false)
        {
            var tmp = TypeParser.GetBoolValue(val, def);
            return tmp;
        }
    }
}
