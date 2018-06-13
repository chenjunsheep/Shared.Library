using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shared.Util
{
    public class TypeParser
    {
        public static bool IsDbNull(object value)
        {
            try
            {
                return Convert.IsDBNull(value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int? GetInt(object val)
        {
            if (val == null || IsDbNull(val))
            {
                return null;
            }
            else
            {
                try
                {
                    if (int.TryParse(val.ToString().Trim(), out int rVal))
                    {
                        return rVal;
                    }
                    else
                    {
                        if (val is bool)
                        {
                            return Convert.ToInt32(val);
                        }
                        else if (val.ToString().Trim().ToLower() == "true")
                        {
                            return 1;
                        }
                        else if (val.ToString().Trim().ToLower() == "false")
                        {
                            return 0;
                        }

                        if (val is char)
                        {
                            return Convert.ToInt32(Convert.ToChar(val));
                        }

                        if (val != null && val.GetType().IsEnum)
                        {
                            return Convert.ToInt32(val);
                        }

                        var result = GetDouble(val);
                        if (result.HasValue && result.Value <= int.MaxValue && result.Value >= int.MinValue)
                        {
                            return Convert.ToInt32(result.Value);
                        }
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static int GetInt32Value(object val, int def = 0)
        {
            if (val == null || IsDbNull(val))
            {
                return def;
            }
            else
            {
                var result = GetInt(val);
                if (result.HasValue)
                {
                    return result.Value;
                }

                return def;
            }
        }

        public static int GetInt32Value(DataRow row, string columnName, int def = 0)
        {
            if (row == null)
            {
                return def;
            }
            else
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return def;
                }
                else
                {
                    if (row.Table.Columns.Contains(columnName))
                    {
                        return GetInt32Value(row[columnName], def);
                    }
                    else
                    {
                        return def;
                    }
                }
            }
        }

        public static double? GetDouble(object val)
        {
            if (val == null || IsDbNull(val))
            {
                return null;
            }
            else
            {
                try
                {
                    var strVal = val.ToString().Trim().ToLower();
                    if (double.TryParse(strVal, out double rVal))
                    {
                        return rVal;
                    }
                    else
                    {
                        if (val is bool)
                        {
                            return Convert.ToDouble(val);
                        }
                        else if (strVal == "true")
                        {
                            return 1;
                        }
                        else if (strVal == "false")
                        {
                            return 0;
                        }

                        if (val.GetType().IsEnum)
                        {
                            return Convert.ToDouble(val);
                        }

                        var regex = new Regex(@"^\d+(\.\d{1,})?([Ee][+-]{0,1}\d+)?$");
                        if (regex.IsMatch(strVal))
                        {
                            return Convert.ToDouble(val);
                        }
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static double GetDoubleValue(object val, double def = 0)
        {
            try
            {
                var result = GetDouble(val);
                if (result.HasValue)
                {
                    return result.Value;
                }

                return def;
            }
            catch (Exception)
            {
                return def;
            }
        }

        public static double GetDoubleValue(DataRow row, string columnName, double def = 0)
        {
            if (row == null)
            {
                return def;
            }
            else
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return def;
                }
                else
                {
                    if (row.Table.Columns.Contains(columnName))
                    {
                        return GetDoubleValue(row[columnName], def);
                    }
                    else
                    {
                        return def;
                    }
                }
            }
        }

        public static double GetDoubleRoundValue(object val, int digits = 2)
        {
            try
            {
                var result = GetDouble(val);
                if (result.HasValue)
                {
                    return Math.Round(result.Value, digits);
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static decimal GetDecimalValue(object val, decimal def = 0)
        {
            if (val == null || IsDbNull(val))
            {
                return def;
            }
            else
            {
                try
                {
                    var result = GetDecimal(val);
                    if ((result.HasValue))
                    {
                        return result.Value;
                    }

                    return def;
                }
                catch (Exception)
                {
                    return def;
                }
            }
        }

        public static decimal? GetDecimal(object val)
        {
            if (val == null || IsDbNull(val))
            {
                return null;
            }
            else
            {
                try
                {
                    if (val is string)
                    {
                        val = val.ToString().Replace(" ", string.Empty);
                        val = val.ToString().Replace("$", string.Empty);
                    }

                    if (decimal.TryParse(val.ToString().Trim(), out decimal rVal))
                    {
                        return rVal;
                    }
                    else
                    {
                        if (val is bool)
                        {
                            return Convert.ToDecimal(val);
                        }
                        else if (val.ToString().Trim().ToLower() == "true")
                        {
                            return 1;
                        }
                        else if (val.ToString().Trim().ToLower() == "false")
                        {
                            return 0;
                        }

                        if (val != null && val.GetType().IsEnum)
                        {
                            return Convert.ToDecimal(val);
                        }
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string GetStringValue(object val, string def = "")
        {
            if (val == null || IsDbNull(val))
            {
                return def;
            }
            else
            {
                try
                {
                    return val.ToString().Trim();
                }
                catch (Exception)
                {
                    return def;
                }
            }
        }

        public static string GetStringValue(DataRow row, string columnName, string def)
        {
            if (string.IsNullOrEmpty(def))
            {
                def = string.Empty;
            }

            if (row == null)
            {
                return def;
            }
            else
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return def;
                }
                else
                {
                    if (row.Table.Columns.Contains(columnName))
                    {
                        return GetStringValue(row[columnName], def);
                    }
                    else
                    {
                        return def;
                    }
                }
            }
        }

        public static string GetStringRound(object val, int digits, string formatStringWhenZeroOrEmpty = "")
        {
            var ret = GetDoubleValue(val, 0);
            if (ret == 0)
            {
                return formatStringWhenZeroOrEmpty;
            }

            return Math.Round(GetDoubleValue(val), digits).ToString();
        }

        public static bool GetBoolValue(object val, bool def = false)
        {
            if (val == null || IsDbNull(val))
            {
                return def;
            }
            else
            {
                try
                {
                    if (val.ToString().Trim().ToLower() == "true" || val.ToString().Trim().ToLower() == "false")
                    {
                        return Convert.ToBoolean(val);
                    }

                    var result = GetDouble(val);
                    if (result.HasValue)
                    {
                        return Convert.ToBoolean(result.Value);
                    }

                    return def;
                }
                catch (Exception)
                {
                    return def;
                }
            }
        }

        public static bool GetBoolValue(DataRow row, string columnName, bool def = false)
        {
            if (row == null)
            {
                return def;
            }
            else
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return def;
                }
                else
                {
                    if (row.Table.Columns.Contains(columnName))
                    {
                        return GetBoolValue(row[columnName], def);
                    }
                    else
                    {
                        return def;
                    }
                }
            }
        }

        public static DateTime GetDateTimeValue(object val, DateTime def)
        {
            if (val == null || IsDbNull(val))
            {
                return def;
            }
            else
            {
                try
                {
                    if (val.ToString() == "{}")
                    {
                        return def;
                    }
                    else
                    {
                        return Convert.ToDateTime(val);
                    }
                }
                catch (Exception)
                {
                    return def;
                }
            }
        }

        public static DateTime? GetDateTimeValue(object val)
        {
            DateTime? res;
            if (val == null || IsDbNull(val))
            {
                res = new DateTime?();
            }
            else
            {
                try
                {
                    res = new DateTime?(Convert.ToDateTime(val));
                }
                catch (Exception)
                {
                    res = new DateTime?();
                }
            }

            return res;
        }

        public static DateTime? GetDateTime(object val, CultureInfo ci)
        {
            DateTime? res;
            if (val == null || IsDbNull(val))
            {
                res = new DateTime?();
            }
            else
            {
                try
                {
                    res = new DateTime?(Convert.ToDateTime(val, ci));
                }
                catch (Exception)
                {
                    res = new DateTime?();
                }
            }

            return res;
        }

        public static DateTime? GetDateTimeValueExact(object val, string partten)
        {
            DateTime? res;
            if (val == null || IsDbNull(val))
            {
                res = null;
            }
            else
            {
                try
                {
                    res = DateTime.ParseExact(val.ToString(), partten, null);
                }
                catch (Exception)
                {
                    res = null;
                }
            }

            return res;
        }
    }
}