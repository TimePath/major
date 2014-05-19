using System;
using System.IO;
using System.Collections.Generic;

namespace FS.Provider.Memory
{
    public class MemoryProvider : VFSProvider
    {
        string mock = "Dummy data.txt";
        int mockLen = Int16.MaxValue;
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
            if (filename != ROOT + mock) {
                return VFSConstants.ERROR;
            }
            //Buffer.BlockCopy (dummy, 0, buf, 0, dummy.Length);
            readBytes = (uint)Math.Min (mockLen, buffer.Length);
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

        int VFSProvider.GetFileInformation (string filename, out VFileInfo info)
        {
            info = new VFileInfo ();
            if (filename == ROOT) {
                info.Attributes = FileAttributes.Directory;
                info.Length = 0;
            } else {
                info.Attributes = FileAttributes.Normal;
                info.Length = mockLen;
            }
            info.LastAccessTime = DateTime.Now;
            info.LastWriteTime = DateTime.Now;
            info.CreationTime = DateTime.Now;

            return VFSConstants.SUCCESS;
        }

        int VFSProvider.List (string path, out IList<VFileInfo> files)
        {
            files = new List<VFileInfo> ();
            if (path == ROOT) {
                files.Add (new VFileInfo {
                    Attributes = FileAttributes.Normal,
                    Length = mockLen,
                    Name = mock
                });
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

