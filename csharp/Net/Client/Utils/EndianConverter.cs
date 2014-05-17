using System;
using System.IO;

namespace Net.Client
{
    public static class EndianConverter
    {
        /// <summary>
        /// Flip the array if on a little endian system
        /// </summary>
        public static byte[] Big (this byte[] b)
        {
            if (BitConverter.IsLittleEndian) {
                Array.Reverse (b);
            }
            return b;
        }

        public static Int32 ReadInt32BE (this BinaryReader br)
        {
            return BitConverter.ToInt32 (br.ReadFully (sizeof(Int32)).Big (), 0);
        }

        private static byte[] ReadFully (this BinaryReader br, int byteCount)
        {
            var result = br.ReadBytes (byteCount);

            if (result.Length != byteCount) {
                throw new EndOfStreamException (string.Format ("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));
            }

            return result;
        }
    }
}

