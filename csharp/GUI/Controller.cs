using System;
using System.Drawing;
using System.ComponentModel;
using FS;

namespace GUI
{
	public class Controller
	{
		public readonly IconDescription Icon;

		public class IconDescription
		{
			public Bitmap Bitmap;
			public string Tooltip;
		}

		public readonly MountDescription Mount;

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

		public Controller ()
		{
			Mount = CreateMount ();
			Icon = new IconDescription {
				Bitmap = IconUtils.CreateIcon (Mount.Path, System.Drawing.Color.Green),
				Tooltip = String.Format ("{0} on {1}", Mount.Label, Mount.Path)
			};
		}

		BackgroundWorker _dokanWorker;

		MountDescription CreateMount ()
		{
			MountDescription desc = new MountDescription ();
			desc.Label = "Dokan";
			desc.Path = (OS.IsWindows)
			             ? Environment.GetEnvironmentVariable ("HOME") 
			             : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");
			return desc;
		}

		public void Start ()
		{
			Console.WriteLine ("Starting");
			_dokanWorker = new BackgroundWorker ();
			_dokanWorker.DoWork += delegate { 
				MountOptions opts = new MountOptions {
					RemovableDrive = false,
					VolumeLabel = Mount.Label,
					MountPoint = Mount.Path
				};
			};
			_dokanWorker.RunWorkerAsync ();
		}
	}
}

