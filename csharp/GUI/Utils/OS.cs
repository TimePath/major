using System;

namespace GUI
{
    public class OS
    {
        public static readonly bool IsWindows = (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX);
    }
}

