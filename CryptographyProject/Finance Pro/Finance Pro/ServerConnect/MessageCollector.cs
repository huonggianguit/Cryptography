using K4os.Compression.LZ4.Streams.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Finance_Pro.ServerConnect
{
    public class MessageCollector : IDisposable
    {
        private static MessageCollector instance;
        private static BinaryReader streamRead;
        public static MessageCollector gI()
        {
            if (instance == null)
            {
                instance = new MessageCollector();
            }
            return instance;
        }
        private MessageCollector()
        {
            streamRead = new BinaryReader(MySession.dataStream, new UTF8Encoding());
        }

        public void Run()
        {
            try
            {
                while (MySession.connected)
                {
                    Debug.WriteLine("Vào theard Reader");
                    Message message = readMessage();
                    if (message == null)
                    {
                        break;
                    }
                    try
                    {
                        onRecieveMsg(message);
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        Thread.Sleep(5);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception ex3)
            {
            }         
        }
        private Message readMessage()
        {
            try
            {
                if (!MySession.IsSocketConnected())
                {                   
                    MessageBox.Show("Mất kết nối với máy chủ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);   
                }
                
                Debug.WriteLine("🟡 [Recv] Bắt đầu nhận message từ server...");

                sbyte cmd = streamRead.ReadSByte();
                Debug.WriteLine($"🟢 [Recv] CMD: {cmd}");

                sbyte subcmd = streamRead.ReadSByte();
                Debug.WriteLine($"🟢 [Recv] SubCMD: {subcmd}");


                if (cmd == 9)
                {
                    byte filenameL = streamRead.ReadByte();
                    byte[] filename = streamRead.ReadBytes(filenameL);
                    ushort rawL = IOUtils.ReadUInt16BE(streamRead);
                    byte[] raw = streamRead.ReadBytes(rawL);
                    List<byte> merged = new List<byte>();
                    merged.Add(filenameL);         // độ dài filename
                    merged.AddRange(filename);     // nội dung filename
                    merged.Add((byte)(rawL >> 8));      // byte cao
                    merged.Add((byte)(rawL & 0xFF));    // byte thấp
                    merged.AddRange(raw);          // nội dung raw
                    // Chuyển sang mảng
                    byte[] mergedArray = merged.ToArray();
                    return new Message(cmd, subcmd, mergedArray);
                }

                if (cmd == 10)
                {
                    byte filenameL = streamRead.ReadByte();
                    byte[] filename = streamRead.ReadBytes(filenameL);
                    int rawL = IOUtils.ReadUInt24BE(streamRead);
                    byte[] raw = streamRead.ReadBytes(rawL);
                    List<byte> merged = new List<byte>();
                    // Gộp theo thứ tự bạn đọc:
                    merged.Add(filenameL);         // độ dài filename
                    merged.AddRange(filename);     // nội dung filename
                    merged.Add((byte)((rawL >> 16) & 0xFF)); // byte cao nhất
                    merged.Add((byte)((rawL >> 8) & 0xFF));  // byte giữa
                    merged.Add((byte)(rawL & 0xFF));         // byte thấp nhất
                    merged.AddRange(raw);          // nội dung raw
                    // Chuyển sang mảng
                    byte[] mergedArray = merged.ToArray();
                    Debug.WriteLine($"NHẬN MSG 10 THÀNH CÔNG :");
                    return new Message(cmd, subcmd, mergedArray);
                }

                short payloadSize = IOUtils.ReadInt16BE(streamRead);
                Debug.WriteLine($"🟢 [Recv] Payload Size: {payloadSize}");
                byte[] payload = streamRead.ReadBytes(payloadSize);
                Debug.WriteLine($"🟢 [Recv] Payload Raw: [{string.Join(", ", payload)}]");
                Debug.WriteLine("✅ [Recv] Message nhận hoàn tất.\n");
                return new Message(cmd, subcmd, payload);

            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public static void onRecieveMsg(Message msg)
        {
            MySession.messageHandler.onMessage(msg);
        }

        public void Dispose()
        {
            if (streamRead != null)
            {
                streamRead.Close();
                streamRead = null;
            }
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }
    }
}
