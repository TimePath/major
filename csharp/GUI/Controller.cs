using System;
using System.Drawing;
using System.ComponentModel;

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

        public void Open ()
        {
            if (OS.IsWindows) {
                string args = string.Format ("/e,{0}", Path);
                System.Diagnostics.Process.Start ("explorer.exe", args);
            } else {
                string args = string.Format ("file://{0}", Path);
                System.Diagnostics.Process.Start (args);
            }
        }
    }

    public class Controller
    {
        public readonly IconDescription Icon;
        public readonly MountDescription Mount;

        public Controller ()
        {
            Mount = CreateMount ();
            Icon = new IconDescription {
                Bitmap = IconFactory.CreateIcon (Mount.Path, System.Drawing.Color.Green),
                Tooltip = String.Format ("{0} on {1}", Mount.Label, Mount.Path)
            };
        }

        BackgroundWorker worker;

        MountDescription CreateMount ()
        {
            MountDescription desc = new MountDescription ();
            desc.Label = "Dokan";
            desc.Path = "f:\\";
            string home = (OS.IsWindows)
			             ? Environment.GetEnvironmentVariable ("HOME") 
			             : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");
            return desc;
        }

        public void Start ()
        {
            worker = new BackgroundWorker ();
            worker.DoWork += delegate { 
                Console.WriteLine ("Starting");
                Console.WriteLine ("Finished");
            };
            worker.RunWorkerAsync ();
        }
    }
}

