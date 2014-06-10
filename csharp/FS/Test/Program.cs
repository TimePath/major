using System;
using FS.Consumer.FUSE;
using FS.Provider.Memory;

namespace FS.Test
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();

        public static void Main (string[] args)
        {
            logger.Info ("Starting");
            VFSConsumer c = new FUSEConsumer ();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                logger.Debug ("Process exiting");
                c.Stop ();
            };
            c.Start (new MemoryProvider (), new MountOptions {
                VolumeLabel = "Dokan",
                //MountPoint = "f:\\",
                MountPoint = Environment.GetEnvironmentVariable ("HOME") + "/mnt/test1",
                FileSystemName = "Virtual",
                RemovableDrive = true
            });
            logger.Info ("Finished");
        }
    }
}
