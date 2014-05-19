using System;
using Dokan;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FS.Consumer.Dokan
{
    public class DokanConsumer : DokanOperations, VFSConsumer
    {

        VFSProvider data;
        #region VFSConsumer implementation
        public void Start (VFSProvider data, MountOptions opts)
        {
            this.data = data;
            DokanOptions dokanOpts = new DokanOptions () {
                DebugMode = true, 
                UseStdErr = true,
                MountPoint = opts.MountPoint,
                VolumeLabel = opts.VolumeLabel,
                FileSystemName = opts.FileSystemName
            };
            int status = DokanNet.DokanMain (dokanOpts, this);
            switch (status) {
            case DokanNet.DOKAN_SUCCESS:
                Console.WriteLine ("Clean shutdown");
                break;
            case DokanNet.DOKAN_DRIVE_LETTER_ERROR:
                Console.WriteLine ("Drive letter error");
                break;
            case DokanNet.DOKAN_DRIVER_INSTALL_ERROR:
                Console.WriteLine ("Driver install error");
                break;
            case DokanNet.DOKAN_MOUNT_ERROR:
                Console.WriteLine ("Mount error");
                break;
            case DokanNet.DOKAN_START_ERROR:
                Console.WriteLine ("Start error");
                break;
            case DokanNet.DOKAN_ERROR:
                Console.WriteLine ("Unknown error");
                break;
            default:
                Console.WriteLine ("Unknown status: %d", status);
                break;
            }
        }

        public void Stop ()
        {
            Console.WriteLine ("TODO: stop Dokan");
        }
        #endregion
        #region DokanOperations implementation
        int DokanOperations.CreateFile (string filename, System.IO.FileAccess access, System.IO.FileShare share, System.IO.FileMode mode, System.IO.FileOptions options, DokanFileInfo info)
        {
            return data.CreateFile (filename, access, share, mode, options);
        }

        int DokanOperations.OpenDirectory (string filename, DokanFileInfo info)
        {
            return data.OpenDirectory (filename);
        }

        int DokanOperations.CreateDirectory (string filename, DokanFileInfo info)
        {
            return data.CreateDirectory (filename);
        }

        int DokanOperations.Cleanup (string filename, DokanFileInfo info)
        {
            return data.Cleanup (filename);
        }

        int DokanOperations.CloseFile (string filename, DokanFileInfo info)
        {
            return data.Close (filename);
        }

        int DokanOperations.ReadFile (string filename, byte[] buffer, ref uint readBytes, long offset, DokanFileInfo info)
        {
            return data.Read (filename, offset, buffer, out readBytes);
        }

        int DokanOperations.WriteFile (string filename, byte[] buffer, ref uint writtenBytes, long offset, DokanFileInfo info)
        {
            return data.Write (filename, offset, buffer, out writtenBytes);
        }

        int DokanOperations.FlushFileBuffers (string filename, DokanFileInfo info)
        {
            return data.Flush (filename);
        }

        int DokanOperations.GetFileInformation (string filename, FileInformation fileinfo, DokanFileInfo info)
        {
            VFileInfo vinfo;
            int status = data.GetFileInformation (filename, out vinfo);
            fileinfo.Attributes = vinfo.Attributes;
            fileinfo.CreationTime = vinfo.CreationTime;
            fileinfo.LastAccessTime = vinfo.LastAccessTime;
            fileinfo.LastWriteTime = vinfo.LastWriteTime;
            fileinfo.Length = vinfo.Length;
            return status;
        }

        int DokanOperations.FindFiles (string filename, System.Collections.ArrayList files, DokanFileInfo info)
        {
            IList<VFileInfo> vfiles;
            int status = data.List (filename, out vfiles);
            foreach (VFileInfo i in vfiles) {
                files.Add (new FileInformation {
                    Attributes = i.Attributes,
                    CreationTime = i.CreationTime,
                    FileName = i.Name,
                    LastAccessTime = i.LastAccessTime,
                    LastWriteTime = i.LastWriteTime,
                    Length = i.IsDirectory() ? 0 : i.Length
                });
            }
            return status;
        }

        int DokanOperations.SetFileAttributes (string filename, System.IO.FileAttributes attr, DokanFileInfo info)
        {
            return data.SetFileAttributes (filename, attr);
        }

        int DokanOperations.SetFileTime (string filename, DateTime ctime, DateTime atime, DateTime mtime, DokanFileInfo info)
        {
            return data.SetFileTime (filename, ctime, atime, mtime);
        }

        int DokanOperations.DeleteFile (string filename, DokanFileInfo info)
        {
            return data.DeleteFile (filename);
        }

        int DokanOperations.DeleteDirectory (string filename, DokanFileInfo info)
        {
            return data.DeleteDirectory (filename);
        }

        int DokanOperations.MoveFile (string filename, string newname, bool replace, DokanFileInfo info)
        {
            return data.MoveFile (filename, newname, replace);
        }

        int DokanOperations.SetEndOfFile (string filename, long length, DokanFileInfo info)
        {
            return data.Truncate (filename, length);
        }

        int DokanOperations.SetAllocationSize (string filename, long length, DokanFileInfo info)
        {
            return data.SetAllocationSize (filename, length);
        }

        int DokanOperations.LockFile (string filename, long offset, long length, DokanFileInfo info)
        {
            return data.LockFile (filename, offset, length);
        }

        int DokanOperations.UnlockFile (string filename, long offset, long length, DokanFileInfo info)
        {
            return data.UnlockFile (filename, offset, length);
        }

        int DokanOperations.GetDiskFreeSpace (ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes, DokanFileInfo info)
        {
            return data.GetDiskFreeSpace (ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes);
        }

        int DokanOperations.Unmount (DokanFileInfo info)
        {
            return data.Unmount ();
        }
        #endregion
    }
}
