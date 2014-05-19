using System;
using FS.Consumer.FUSE;
using FS.Provider.Memory;

namespace FS.Test
{
    public class Program
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Starting");
            VFSConsumer c = new FUSEConsumer ();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                Console.WriteLine ("Process exiting");
                c.Stop ();
            };
            c.Start (new MemoryProvider (), new MountOptions {
                VolumeLabel = "Dokan",
                //MountPoint = "f:\\",
                MountPoint = Environment.GetEnvironmentVariable("HOME") + "/mnt/test1",
                FileSystemName = "Virtual",
                RemovableDrive = true
            });
            Console.WriteLine ("Finished");
        }
    }
}

