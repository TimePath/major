using System;
using FS;
using System.Diagnostics;

namespace GUI
{
    public static class OS
    {
        public static readonly bool IsWindows = (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX);

        public static void Open (this MountOptions opts)
        {
            if (IsWindows) {
                string args = string.Format ("/e,{0}", opts.MountPoint);
                Process.Start ("explorer.exe", args);
            } else {
                string args = string.Format ("file://{0}/", opts.MountPoint);
                Process.Start (args);
            }
        }
    }
}

