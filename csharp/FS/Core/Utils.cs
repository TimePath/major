using System;

namespace FS
{
    public static class DateTimeUtils
    {
        private static readonly DateTime Epoch = new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnix (this DateTime dateTime)
        {
            return (long)(dateTime - Epoch).TotalSeconds;
        }

        public static DateTime ToDateTime (this long unix)
        {
            return Epoch.AddMilliseconds (unix);
        }

        public static long Nanoseconds (this DateTime self)
        {
            return self.Ticks % TimeSpan.TicksPerSecond * 100;
        }
    }
}

