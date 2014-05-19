using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace FS
{
    public class MountOptions
    {
        public bool RemovableDrive;
        public string VolumeLabel;
        public string FileSystemName;
        public string MountPoint;
    }

    public class VFileInfo
    {
        public bool IsDirectory() {
            return Attributes == FileAttributes.Directory;
        }
        public FileAttributes Attributes;
        public DateTime CreationTime = DateTime.Now;
        public DateTime LastAccessTime = DateTime.Now;
        public DateTime LastWriteTime = DateTime.Now;
        public long Length;
        public string Name;
    }

    public class VFSConstants
    {
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_PATH_NOT_FOUND = 3;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_SHARING_VIOLATION = 32;
        public const int ERROR_INVALID_NAME = 123;
        public const int ERROR_FILE_EXISTS = 80;
        public const int ERROR_ALREADY_EXISTS = 183;
        public const int SUCCESS = 0;
        /// General Error
        public const int ERROR = -1;
        /// Bad Drive letter
        public const int DRIVE_LETTER_ERROR = -2;
        /// Can't install driver
        public const int DRIVER_INSTALL_ERROR = -3;
        /// Driver something wrong
        public const int START_ERROR = -4;
        /// Can't assign drive letter
        public const int MOUNT_ERROR = -5;
        /// Mount point is invalid
        public const int MOUNT_POINT_ERROR = -6;
    }

    public interface VFSConsumer
    {
        void Start (VFSProvider data, MountOptions opts);

        void Stop ();
    }

    /// <summary>
    /// TODO: enums, exceptions
    /// </summary>
    public interface VFSProvider
    {

        /// <summary>
        /// Check if unmounting is possible
        /// </summary>
        int Unmount ();

        int CreateFile (string filename, FileAccess access, FileShare share, FileMode mode, FileOptions options);

        /// <summary>
        /// cd
        /// </summary>
        /// <returns>The directory.</returns>
        /// <param name="path">Path.</param>
        int OpenDirectory (string path);

        /// <summary>
        /// mkdir
        /// </summary>
        /// <returns>The directory.</returns>
        /// <param name="path">Path.</param>
        int CreateDirectory (string path);

        int Cleanup (string filename);

        int Close (string filename);

        int Read (string filename, long offset, byte[] buffer, out uint readBytes);

        int Write (string filename, long offset, byte[] buffer, out uint writtenBytes);

        int Flush (string filename);

        int GetFileInformation (string filename, out VFileInfo info);

        /// <summary>
        /// ls
        /// </summary>
        /// <returns>The files.</returns>
        /// <param name="path">Path.</param>
        /// <param name="files">Files.</param>
        int List (string path, out IList<VFileInfo> files);

        int SetFileAttributes (string filename, FileAttributes attr);

        /// <summary>
        /// touch
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="ctime">Creation time.</param>
        /// <param name="atime">Access time.</param>
        /// <param name="mtime">Modification time.</param>
        int SetFileTime (string filename, DateTime ctime, DateTime atime, DateTime mtime);

        /// <summary>
        /// rm
        /// </summary>
        /// <param name="filename">Filename.</param>
        int DeleteFile (string filename);

        /// <summary>
        /// rmdir
        /// </summary>
        /// <param name="path">Path.</param>
        int DeleteDirectory (string path);

        /// <summary>
        /// mv
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="newname">Newname.</param>
        /// <param name="replace">If set to <c>true</c> replace.</param>
        int MoveFile (string filename, string newname, bool replace);

        int Truncate (string filename, long length);

        int SetAllocationSize (string filename, long length);

        int LockFile (string filename, long offset, long length);

        int UnlockFile (string filename, long offset, long length);

        int GetDiskFreeSpace (ref ulong freeBytesAvailable, ref ulong totalBytes, ref ulong totalFreeBytes);
    }
}
