using System;

namespace FS.Test
{
    public class Program
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Starting");
            new FS.Consumer.Dokan.DokanConsumer ().Start (new FS.Provider.Memory.MemoryProvider (), new MountOptions {
                VolumeLabel = "Dokan",
                MountPoint = "f:\\",
                FileSystemName = "Virtual",
                RemovableDrive = true
            });
            Console.WriteLine ("Finished");
        }
    }
}

