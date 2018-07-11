namespace Shared.Util.Extension
{
    public static class StringExtension
    {
        public static string SubLeftString(this string str, int length, string fillMark = "")
        {
            var tmp = TypeParser.GetStringValue(str);
            if (str.Length > length)
            {
                str = $"{str.Substring(0, length - 1)}{fillMark}";
            }

            return tmp;
        }

        public static string SubRightString(this string str, int length, string fillMark = "")
        {
            var tmp = TypeParser.GetStringValue(str);
            if (str.Length > length)
            {
                var diff = str.Length - length;
                str = $"{str.Substring(diff, length - diff)}{fillMark}";
            }

            return tmp;
        }
    }
}
