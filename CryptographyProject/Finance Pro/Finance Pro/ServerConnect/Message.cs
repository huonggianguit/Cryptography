using System;
using System.IO;
using System.Text;

namespace Finance_Pro.ServerConnect
{
    public class Message : IDisposable
    {
        private sbyte _cmd;
        private sbyte _subCmd;

        private MemoryStream _buff;  // Bộ đệm chứa raw
    

        private BinaryWriter _writer; //bọc vào để ghi
        private BinaryReader _reader; // bọc vào để đọc. 

        public Message(int cmd,int subCmd)
        {
            _cmd = (sbyte)cmd;
            _subCmd = (sbyte)subCmd;
            _buff = new MemoryStream(); // Đoạn này ko cần cấp buffer mặc định đâu, vì ở dưới nó tự mở rộng khi thêm r
            _writer = new BinaryWriter(_buff);
        }

        public Message(int cmd, int subCmd, byte[] data)
        {
            _cmd = (sbyte)cmd;
            _subCmd = (sbyte)subCmd;
            _buff = new MemoryStream(data);
            _reader = new BinaryReader(_buff);
        }

        // === WRITE METHODS ===
        public void WriteSByte(sbyte value) => _writer.Write(value);
        public void WriteByte(byte value) => _writer.Write(value);

        public void WriteInt16(short value) => _writer.Write(value);
        public void WriteUInt16(ushort value) => _writer.Write(value);

        public void WriteInt32(int value) => _writer.Write(value);
        public void WriteUInt32(uint value) => _writer.Write(value);

        public void WriteInt64(long value) => _writer.Write(value);
        public void WriteUInt64(ulong value) => _writer.Write(value);

        public void WriteFloat(float value) => _writer.Write(value);
        public void WriteDouble(double value) => _writer.Write(value);

        public void WriteBool(bool value) => _writer.Write(value);
        public void WriteBytes(byte[] data) => _writer.Write(data);
        public void WriteUInt16BigEndian(ushort value)
        {
            _writer.Write((byte)(value >> 8));     // byte cao
            _writer.Write((byte)(value & 0xFF));   // byte thấp
        }
        public void WriteUInt24BigEndian(int value)
        {
            if (value < 0 || value > 0xFFFFFF)
                throw new ArgumentOutOfRangeException(nameof(value), "Giá trị phải nằm trong 0 đến 16,777,215 (0xFFFFFF)");

            _writer.Write((byte)(value >> 16));        // byte cao nhất
            _writer.Write((byte)((value >> 8) & 0xFF)); // byte giữa
            _writer.Write((byte)(value & 0xFF));        // byte thấp nhất
        }

        public void WriteString(string value)
        {
            if (value == null)
            {
                _writer.Write((ushort)0);
                return;
            }
            var bytes = Encoding.UTF8.GetBytes(value);
            _writer.Write((ushort)bytes.Length); // Ghi độ dài trước
            _writer.Write(bytes);
        }
        public void WriteUTF(string value)
        {
            if (value == null)
            {
                _writer.Write((ushort)0); // hoặc ghi 2 byte 0 theo Big Endian
                return;
            }

            using var msTemp = new MemoryStream();
            foreach (char c in value)
            {
                if (c >= 0x0001 && c <= 0x007F)
                {
                    msTemp.WriteByte((byte)c);
                }
                else if (c > 0x07FF)
                {
                    msTemp.WriteByte((byte)(0xE0 | ((c >> 12) & 0x0F)));
                    msTemp.WriteByte((byte)(0x80 | ((c >> 6) & 0x3F)));
                    msTemp.WriteByte((byte)(0x80 | (c & 0x3F)));
                }
                else
                {
                    msTemp.WriteByte((byte)(0xC0 | ((c >> 6) & 0x1F)));
                    msTemp.WriteByte((byte)(0x80 | (c & 0x3F)));
                }
            }

            byte[] data = msTemp.ToArray();
            if (data.Length > 65535)
                throw new Exception("UTF string too long");

            // 🟡 Ghi độ dài theo BIG ENDIAN (Java yêu cầu)
            _writer.Write((byte)((data.Length >> 8) & 0xFF)); // high byte
            _writer.Write((byte)(data.Length & 0xFF));        // low byte

            // 🟡 Ghi chuỗi byte UTF
            _writer.Write(data);
        }



        // === READ METHODS ===

        public sbyte ReadSByte() => _reader.ReadSByte();
        public byte ReadByte() => _reader.ReadByte();

        public short ReadInt16() => _reader.ReadInt16();

        public ushort ReadUInt16() => _reader.ReadUInt16();
        // Đọc 2 byte không dấu (Big Endian)
        public ushort ReadUInt16BE()
        {
            byte high = _reader.ReadByte();
            byte low = _reader.ReadByte();
            return (ushort)((high << 8) | low);
        }

        // Đọc 3 byte không dấu (Big Endian)
        public int ReadInt24BE()
        {
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            byte b3 = _reader.ReadByte();
            return (b1 << 16) | (b2 << 8) | b3;
        }

        public int ReadInt32() => _reader.ReadInt32();
        public uint ReadUInt32() => _reader.ReadUInt32();

        public long ReadInt64() => _reader.ReadInt64();
        public ulong ReadUInt64() => _reader.ReadUInt64();

        public float ReadFloat() => _reader.ReadSingle();
        public double ReadDouble() => _reader.ReadDouble();

        public bool ReadBool() => _reader.ReadBoolean();
        public byte[] ReadBytes(int n) => _reader.ReadBytes(n);
      
        public string ReadString()
        {
            ushort length = _reader.ReadUInt16();
            if (length == 0) return string.Empty;
            var bytes = _reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        // === HELPERS ===

        public byte[] GetWrittenData()
        {
            _writer.Flush();
            return _buff.ToArray();
        }

        public void ResetWrite()
        {
            _buff.SetLength(0);
        }

        public void ResetRead()
        {
            if (_buff != null)
                _buff.Position = 0;
        }
        public sbyte GetCmd()
        {
            return _cmd;
        }
        public sbyte GetSubCmd()
        {
            return _subCmd;
        }
        public byte[] GetPayload()
        {
            if (_buff == null)
                throw new InvalidOperationException("No raw data available. This message was not initialized with raw data.");
            return _buff.ToArray();
        }
      

        public short GetPayloadSize()
        {
            if (_buff == null)
                throw new InvalidOperationException("No raw data available. This message was not initialized with raw data.");
            return (short)_buff.Length;
        }
        public void WriteToStreamBigEndian(BinaryWriter writer)
        {
            // Ghi CMD, SubCMD
            writer.Write((byte)_cmd);
            writer.Write((byte)_subCmd);

            byte[] payload = GetPayload();

            // 🔍 Chỉ ghi size nếu không phải CMD 7/8
            if (_cmd != 7 && _cmd != 8)
            {
                short size = (short)payload.Length;
                writer.Write((byte)(size >> 8)); // High byte
                writer.Write((byte)(size & 0xFF)); // Low byte
            }

            writer.Write(payload);
            writer.Flush();
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _buff?.Dispose();          
        }
    }

}
