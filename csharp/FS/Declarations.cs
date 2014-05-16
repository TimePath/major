using System;
using System.IO;
using System.Collections;

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
		public bool IsDirectory;
		public FileAttributes Attributes;
		public DateTime CreationTime;
		public DateTime LastAccessTime;
		public DateTime LastWriteTime;
		public long Length;
		public string FileName;
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

		int CreateFile (
			string filename,
			FileAccess access,
			FileShare share,
			FileMode mode,
			FileOptions options
		);

		int OpenDirectory (string filename);

		int CreateDirectory (string filename);

		int Cleanup (string filename);

		int CloseFile (string filename);

		int ReadFile (
			string filename,
			byte[] buffer,
			ref uint readBytes,
			long offset
		);

		int WriteFile (
			string filename,
			byte[] buffer,
			ref uint writtenBytes,
			long offset
		);

		int FlushFileBuffers (string filename);

		int GetFileInformation (
			string filename,
			VFileInfo info
		);

		int FindFiles (string filename, System.Collections.ArrayList files);

		int SetFileAttributes (
			string filename,
			FileAttributes attr
		);

		int SetFileTime (
			string filename,
			DateTime ctime,
			DateTime atime,
			DateTime mtime
		);

		int DeleteFile (string filename);

		int DeleteDirectory (string filename);

		int MoveFile (
			string filename,
			string newname,
			bool replace
		);

		int Truncate (
			string filename,
			long length
		);

		int SetAllocationSize (
			string filename,
			long length
		);

		int LockFile (
			string filename,
			long offset,
			long length
		);

		int UnlockFile (
			string filename,
			long offset,
			long length
		);

		int GetDiskFreeSpace (
			ref ulong freeBytesAvailable,
			ref ulong totalBytes,
			ref ulong totalFreeBytes
		);
	}
}

