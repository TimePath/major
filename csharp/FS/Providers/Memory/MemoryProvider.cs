using System;
using System.IO;
using System.Collections.Generic;

namespace FS.Provider.Memory
{
    public class MemoryProvider : VFSProvider
    {
        string theFile = "Dummy data.txt";
        int mult = Int16.MaxValue;

        #region implemented abstract members of VFS

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

        int VFSProvider.CloseFile (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.ReadFile (string filename, byte[] buffer, ref uint readBytes, long offset)
        {
            if (filename != @"\" + theFile) {
                return VFSConstants.ERROR;
            }
            readBytes = (uint)Math.Min (mult, buffer.Length);
            for (int i = 0; i < readBytes; i++) {
                buffer [i] = 42;
            }
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.WriteFile (string filename, byte[] buffer, ref uint writtenBytes, long offset)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.FlushFileBuffers (string filename)
        {
            return VFSConstants.SUCCESS;
        }

        int VFSProvider.GetFileInformation (string filename, VFileInfo info)
        {
            if (filename == @"\") {
                info.Attributes = FileAttributes.Directory;
                info.Length = 0;
            } else {
                info.Attributes = FileAttributes.Normal;
                info.Length = mult;
            }
            info.LastAccessTime = DateTime.Now;
            info.LastWriteTime = DateTime.Now;
            info.CreationTime = DateTime.Now;

            return VFSConstants.SUCCESS;
        }

        int VFSProvider.FindFiles (string filename, IList<VFileInfo> files)
        {
            files.Add (new VFileInfo {
                Attributes = FileAttributes.Normal,
                LastAccessTime = DateTime.Now,
                LastWriteTime = DateTime.Now,
                CreationTime = DateTime.Now,
                Length = mult,
                FileName = theFile
            });
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

