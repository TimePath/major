using System;
using System.Drawing;
using System.ComponentModel;
using Dokan;

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
				DokanOptions opts = new DokanOptions {
					// Limit to one thread during debugging
					ThreadCount = (ushort)(System.Diagnostics.Debugger.IsAttached ? 1 : 0), 
					DebugMode = true, 
					UseStdErr = true,
					UseKeepAlive = true,
					NetworkDrive = false,
					RemovableDrive = false,
					VolumeLabel = Mount.Label,
					MountPoint = Mount.Path
				};
				// DokanNet.DokanMain (opts, new DokanMemoryStreamOperations ());
			};
			_dokanWorker.RunWorkerAsync ();
		}
	}
}

