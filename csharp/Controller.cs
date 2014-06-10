using System;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using FS;

namespace GUI
{
    public class IconDescription
    {
        public Bitmap Bitmap;
        public string Tooltip;
    }

    public class MountDescription
    {
        public string Label;
        public string Path;
    }

    public class Controller
    {
        public void Browse ()
        {
            Mount.Open ();
        }

        NetController net;

        /// <summary>
        /// Connect the specified user and password.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="password">Password.</param>
        public void Connect (string user, string password)
        {
            net.Connect (user, password);
        }

        public readonly IconDescription Icon;
        public readonly MountOptions Mount = new MountOptions {
            VolumeLabel = "NetFS",
            MountPoint = OS.IsWindows ? "f:\\" : Environment.GetEnvironmentVariable ("HOME") + "/mnt/test1",
            FileSystemName = "Virtual",
            RemovableDrive = true
        };

        public Controller ()
        {
            Icon = new IconDescription {
                Bitmap = IconFactory.CreateIcon (Mount.MountPoint, System.Drawing.Color.Green),
                Tooltip = String.Format ("{0} on {1}", Mount.VolumeLabel, Mount.MountPoint)
            };
            net = new NetController (Mount);
        }
    }
}

