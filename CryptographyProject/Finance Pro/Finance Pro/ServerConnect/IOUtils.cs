using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Pro.ServerConnect
{
    public class IOUtils
    {
      
        // Chuyển int thành byte[] (4 bytes, little-endian)
        public static byte[] IntToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        // Chuyển short thành byte[] (2 bytes, little-endian)
        public static byte[] ShortToBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        // Chuyển string (Unicode) thành byte[] sử dụng Encoding.UTF8 (hoặc Encoding.Unicode nếu muốn)
        public static byte[] StringToBytes(string str, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;  // Mặc định dùng UTF8, nếu muốn dùng UTF16 (Unicode) thì đổi thành Encoding.Unicode

            return encoding.GetBytes(str);
        }
        public static byte[] StringToUTFBytes(string str)
        {
            var utf8 = Encoding.UTF8.GetBytes(str);
            using (var ms = new MemoryStream())
            {
                ushort len = (ushort)utf8.Length;
                ms.WriteByte((byte)(len >> 8));  // high byte
                ms.WriteByte((byte)(len));       // low byte
                ms.Write(utf8, 0, utf8.Length);
                return ms.ToArray();
            }
        }

        // Nếu muốn đảo thứ tự byte (ví dụ chuyển từ little-endian sang big-endian) thì dùng thêm hàm này
        public static byte[] ReverseBytes(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        // Chuyển về đọc chuẩn big-endian (BE) từ BinaryReader
        public static short ReadInt16BE(BinaryReader reader)
        {
            byte high = reader.ReadByte();
            byte low = reader.ReadByte();
            return (short)((high << 8) | low);
        }

        public static ushort ReadUInt16BE(BinaryReader reader)
        {
            byte high = reader.ReadByte();
            byte low = reader.ReadByte();
            return (ushort)((high << 8) | low);
        }
        public static int ReadUInt24BE(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            byte b3 = reader.ReadByte();
            return (b1 << 16) | (b2 << 8) | b3;
        }

        public static int ReadInt32BE(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            byte b3 = reader.ReadByte();
            byte b4 = reader.ReadByte();
            return (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
        }

        public static string ReadUTF(BinaryReader reader)
        {
            ushort length = ReadUInt16BE(reader);
            byte[] utfBytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(utfBytes);
        }

    }
}
