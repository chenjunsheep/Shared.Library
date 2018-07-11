using System;

namespace Shared.Util.Extension
{
    public static class TimeExtension
    {
        public static long GetTimeStamp()
        {
            var dateTime = DateTime.Now;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }
    }
}
