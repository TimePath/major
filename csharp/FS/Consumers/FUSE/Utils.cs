using System;

namespace FS.Consumer.FUSE
{
    public static class DateTimeUtils
    {
        public static long ToUnix(this DateTime dateTime) {
            return (long)(dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
    }
}

