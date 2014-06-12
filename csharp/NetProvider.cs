using System;
using FS;
using System.IO;
using Net.Client;
using System.Collections.Generic;
using Major.Proto;

namespace GUI
{
    public class NetProvider : VFSProvider
    {
        ProtoConnection conn;

        public NetProvider (ProtoConnection conn)
        {
            if (conn == null)
                throw new ArgumentNullException ("conn");
            this.conn = conn;
        }

        string ROOT = "/";

        #region implemented abstract members of VFSProvider

        int VFSProvider.CreateFile (string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.OpenDirectory (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.CreateDirectory (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Cleanup (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Close (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Read (string filename, long offset, byte[] buffer, out uint readBytes)
        {
            readBytes = 0;
            //Buffer.BlockCopy (dummy, 0, buf, 0, dummy.Length);
            readBytes = (uint)Math.Min (42, buffer.Length);
            for (int i = 0; i < readBytes; i++) {
                buffer [i] = 42;
            }
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Write (string filename, long offset, byte[] buffer, out uint writtenBytes)
        {
            writtenBytes = 0;
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Flush (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        VFileInfo wrap (Major.Proto.File file)
        {
            return new VFileInfo {
                Name = file.Name,
                Attributes = (file.Type == Major.Proto.File.Types.FileType.DIRECTORY ? FileAttributes.Directory : FileAttributes.Normal),
                Length = (file.Type == Major.Proto.File.Types.FileType.DIRECTORY ? 0 : 42),
                LastAccessTime = DateTime.Now,
                LastWriteTime = file.HasLastModified ? file.LastModified.ToDateTime() : DateTime.Now,
                CreationTime = DateTime.Now
            };
        }

        int VFSProvider.GetFileInformation (string filename, out VFileInfo info)
        {
            InfoResponse ir = conn.Write<InfoResponse> (Meta.CreateBuilder ()
                .SetTag ((int)DateTime.Now.Ticks)
                .SetInfoRequest (InfoRequest.CreateBuilder ()
                    .SetPath (filename)
                    .Build ())
                .Build ());
            info = wrap (ir.File);

            return VFSConstants.SUCCESS;
        }

        int VFSProvider.List (string path, out IList<VFileInfo> files)
        {
            ListResponse lr = conn.Write<ListResponse> (Meta.CreateBuilder ()
                .SetTag ((int)DateTime.Now.Ticks)
                .SetListRequest (ListRequest.CreateBuilder ()
                    .SetPath (path)
                    .Build ())
                .Build ());
            files = new List<VFileInfo> ();
            foreach (var file in lr.FileList) {
                files.Add (wrap(file));
            }
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.SetFileAttributes (string filename, FileAttributes attr)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.SetFileTime (string filename, DateTime ctime, DateTime atime, DateTime mtime)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.DeleteFile (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.DeleteDirectory (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.MoveFile (string filename, string newname, bool replace)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Truncate (string filename, long length)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.SetAllocationSize (string filename, long length)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.LockFile (string filename, long offset, long length)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.UnlockFile (string filename, long offset, long length)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.GetDiskFreeSpace (ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes)
        {
            totalBytes = int.MaxValue;
            freeBytesAvailable = totalBytes / 2;
            totalFreeBytes = totalBytes / 2;
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.Unmount ()
        {
            return VFSConstants.SUCCESS;
        }

        #endregion

    }
}

