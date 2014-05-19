using System;
using System.Collections.Generic;
using Mono.Fuse;
using Mono.Unix.Native;
using System.Diagnostics;

namespace FS.Consumer.FUSE
{
    public class FUSEConsumer : FileSystem, VFSConsumer
    {

        VFSProvider data;
        #region VFSConsumer implementation
        public void Start (VFSProvider data, MountOptions opts)
        {
            this.data = data;
            base.MountPoint = opts.MountPoint;
            Console.WriteLine("Unmounting previous...");
            Process p = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "fusermount",
                    Arguments = "-u " + base.MountPoint
                }
            };
            p.Start();
            p.WaitForExit();
            Console.WriteLine("Done. Mounting...");
            base.Start ();
        }

        new public void Stop ()
        {
            base.Stop ();
            Console.WriteLine ("Stopped FUSE");
        }
        #endregion
        #region FileSystem implementation
        protected override Errno OnGetPathStatus (string path, out Stat stbuf)
        {
            VFileInfo info;
            data.GetFileInformation (path, out info);
            // return Errno.ENOENT;
            stbuf = new Stat ();
            stbuf.st_mode = info.IsDirectory () ? FilePermissions.S_IFDIR : FilePermissions.S_IFREG;
            stbuf.st_atime = info.LastAccessTime.ToUnix ();
            stbuf.st_ctime = info.CreationTime.ToUnix ();
            stbuf.st_mtime = info.LastWriteTime.ToUnix ();
            stbuf.st_size = info.IsDirectory () ? 0 : info.Length;
            stbuf.st_uid = Syscall.getuid ();
            stbuf.st_gid = Syscall.getgid ();
            switch (path) {
            case "/":
                stbuf.st_mode |= NativeConvert.FromOctalPermissionString ("0755");
                stbuf.st_nlink = 2;
                break;
            default:
                stbuf.st_mode |= NativeConvert.FromOctalPermissionString ("0444");
                stbuf.st_nlink = 1;
                break;
            }
            return 0;
        }

        protected override Errno OnReadDirectory (string path, OpenedPathInfo fi, out IEnumerable<DirectoryEntry> paths)
        {
            paths = null;
            if (path != "/") {
                return Errno.ENOENT;
            }

            IList<VFileInfo> vfiles;
            data.List (path, out vfiles);

            IList<DirectoryEntry> files = new List<DirectoryEntry> ();
            files.Add (new DirectoryEntry ("."));
            files.Add (new DirectoryEntry (".."));
            foreach (VFileInfo vf in vfiles) {
                files.Add (new DirectoryEntry (vf.Name));
            }
            paths = files;
            return 0;
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
            bytesWritten = 0;
            VFileInfo info;
            if (data.GetFileInformation (path, out info) != 0) {
                return Errno.ENOENT;
            }
            uint ubytesWritten = 0;
            data.Read (path, offset, buf, out ubytesWritten);
            bytesWritten = (int)ubytesWritten;
            return 0;
        }
        #endregion
    }
}
