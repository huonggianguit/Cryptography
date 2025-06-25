using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Pro.ServerConnect
{
    public class myWriter
    {
        public sbyte[] buffer = new sbyte[2048];

        private int posWrite;

        private int lenght = 2048;

        public myWriter()
        {
        }

        public myWriter(int len)
        {
            buffer = new sbyte[len];
            lenght = len;
        }

        public void writeSByte(sbyte value)
        {
            checkLenght(0);
            buffer[posWrite++] = value;
        }

        public void writeSByteUncheck(sbyte value)
        {
            buffer[posWrite++] = value;
        }

        public void writeByte(sbyte value)
        {
            writeSByte(value);
        }

        public void writeByte(int value)
        {
            writeSByte((sbyte)value);
        }

        public void writeChar(char value)
        {
            writeSByte(0);
            writeSByte((sbyte)value);
        }

        public void writeUnsignedByte(byte value)
        {
            writeSByte((sbyte)value);
        }

        public void writeUnsignedByte(byte[] value)
        {
            checkLenght(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writeSByteUncheck((sbyte)value[i]);
            }
        }

        public void writeSByte(sbyte[] value)
        {
            checkLenght(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                writeSByteUncheck(value[i]);
            }
        }

        public void writeShort(short value)
        {
            checkLenght(2);
            for (int num = 1; num >= 0; num--)
            {
                writeSByteUncheck((sbyte)(value >> num * 8));
            }
        }

        public void writeShort(int value)
        {
            checkLenght(2);
            short num = (short)value;
            for (int num2 = 1; num2 >= 0; num2--)
            {
                writeSByteUncheck((sbyte)(num >> num2 * 8));
            }
        }

        public void writeUnsignedShort(ushort value)
        {
            checkLenght(2);
            for (int num = 1; num >= 0; num--)
            {
                writeSByteUncheck((sbyte)(value >> num * 8));
            }
        }

        public void writeInt(int value)
        {
            checkLenght(4);
            for (int num = 3; num >= 0; num--)
            {
                writeSByteUncheck((sbyte)(value >> num * 8));
            }
        }

        public void writeLong(long value)
        {
            checkLenght(8);
            for (int num = 7; num >= 0; num--)
            {
                writeSByteUncheck((sbyte)(value >> num * 8));
            }
        }

        public void writeBoolean(bool value)
        {
            writeSByte((sbyte)(value ? 1 : 0));
        }

        public void writeBool(bool value)
        {
            writeSByte((sbyte)(value ? 1 : 0));
        }

        public void writeString(string value)
        {
            char[] array = value.ToCharArray();
            writeShort((short)array.Length);
            checkLenght(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writeSByteUncheck((sbyte)array[i]);
            }
        }

        //public void writeUTF(string value)
        //{
        //	Encoding unicode = Encoding.Unicode;
        //	Encoding encoding = Encoding.GetEncoding(65001);
        //	byte[] bytes = unicode.GetBytes(value);
        //	byte[] array = Encoding.Convert(unicode, encoding, bytes);
        //	writeShort((short)array.Length);
        //	checkLenght(array.Length);
        //	for (int i = 0; i < array.Length; i++)
        //	{
        //		sbyte value2 = (sbyte)array[i];
        //		writeSByteUncheck(value2);
        //	}
        //}
        public void writeUTF(string value)
        {
            if (value == null)
                value = "";

            // Encode chuỗi thành UTF-8 (đúng chuẩn Java dùng trong DataOutputStream.writeUTF)
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);

            // Java writeUTF giới hạn độ dài ở 65535 bytes (vì dùng writeShort 2 byte)
            if (utf8Bytes.Length > 65535)
                throw new Exception("UTF string too long for writeUTF (max 65535 bytes)");

            // Ghi độ dài (2 byte - unsigned short)
            writeShort((short)utf8Bytes.Length);

            checkLenght(utf8Bytes.Length); // Nếu mày có check dung lượng buffer

            // Ghi từng byte UTF-8 vào buffer (giống hệt Java)
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                writeSByteUncheck((sbyte)utf8Bytes[i]);
            }
        }

        public void write(ref sbyte[] data, int arg1, int arg2)
        {
            if (data == null)
            {
                return;
            }
            for (int i = 0; i < arg2; i++)
            {
                writeSByte(data[i + arg1]);
                if (posWrite > buffer.Length)
                {
                    break;
                }
            }
        }

        public void write(sbyte[] value)
        {
            writeSByte(value);
        }

        public sbyte[] getData()
        {
            if (posWrite <= 0)
            {
                return null;
            }
            sbyte[] array = new sbyte[posWrite];
            for (int i = 0; i < posWrite; i++)
            {
                array[i] = buffer[i];
            }
            return array;
        }

        public void checkLenght(int ltemp)
        {
            if (posWrite + ltemp > lenght)
            {
                sbyte[] array = new sbyte[lenght + 1024 + ltemp];
                for (int i = 0; i < lenght; i++)
                {
                    array[i] = buffer[i];
                }
                buffer = null;
                buffer = array;
                lenght += 1024 + ltemp;
            }
        }

        public void Close()
        {
            buffer = null;
        }

        public void close()
        {
            buffer = null;
        }
    }
}
