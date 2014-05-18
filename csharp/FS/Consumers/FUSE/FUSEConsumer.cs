using System;
using System.Collections.Generic;
using Mono.Fuse;
using Mono.Unix.Native;

namespace FS.Consumer.FUSE
{
    public class FUSEConsumer : FileSystem, VFSConsumer
    {
        byte[] dummy = System.Text.Encoding.UTF8.GetBytes("Test");

        public static void Main (string[] args)
        {
            Console.WriteLine ("Starting");
            new FUSEConsumer ().Start (new FS.Provider.Memory.MemoryProvider (), new MountOptions {
                VolumeLabel = "Dokan",
                MountPoint = Environment.GetEnvironmentVariable("HOME") + "/mnt/test",
                FileSystemName = "Virtual",
                RemovableDrive = true
            });
            Console.WriteLine ("Finished");
        }

        #region VFSConsumer implementation

        public void Start (VFSProvider data, MountOptions opts)
        {
            MountPoint = opts.MountPoint;
            base.Start ();
        }

        #endregion

        #region FileSystem implementation

        protected override Errno OnGetPathStatus (string path, out Stat stbuf)
        {
            stbuf = new Stat ();
            switch (path) {
                case "/":
                stbuf.st_mode = FilePermissions.S_IFDIR | 
                    NativeConvert.FromOctalPermissionString ("0755");
                stbuf.st_nlink = 2;
                return 0;
                default:
                stbuf.st_mode = FilePermissions.S_IFREG |
                    NativeConvert.FromOctalPermissionString ("0444");
                stbuf.st_nlink = 1;
                stbuf.st_size = dummy.Length;
                return 0;
                // return Errno.ENOENT;
            }
        }

        protected override Errno OnReadDirectory (string path, OpenedPathInfo fi,
                                                  out IEnumerable<DirectoryEntry> paths)
        {
            paths = null;
            if (path != "/")
                return Errno.ENOENT;

            paths = GetEntries ();
            return 0;
        }

        private IEnumerable<DirectoryEntry> GetEntries ()
        {
            yield return new DirectoryEntry (".");
            yield return new DirectoryEntry ("..");
            yield return new DirectoryEntry ("test");
        }

        protected override Errno OnOpenHandle (string path, OpenedPathInfo fi)
        {
            // return Errno.ENOENT; // Doesn't exist
            //if (fi.OpenAccess != OpenFlags.O_RDONLY)
            //    return Errno.EACCES; // Read only
            return 0;
        }

        protected override Errno OnReadHandle (string path, OpenedPathInfo fi, byte[] buf, long offset, out int bytesWritten)
        {
            Buffer.BlockCopy (dummy, 0, buf, 0, dummy.Length);
            bytesWritten = buf.Length;
            // return Errno.ENOENT;
            return 0;
        }

        #endregion

    }
}
