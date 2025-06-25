using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Finance_Pro.ServerConnect
{
    public class MessageSender : IDisposable
    {
        private static MessageSender instance ;
        private static BinaryWriter streamWrite;
        public static MessageSender gI()
        {
            if (instance == null)
            {
                instance = new MessageSender();
            }
            return instance;
        }
        public static ConcurrentQueue<Message> sendingMessage;
        public MessageSender()
        {
            sendingMessage = new ConcurrentQueue<Message>();
            streamWrite = new BinaryWriter(MySession.dataStream, new UTF8Encoding());
        }

        public void Run()
        {
            while (MySession.connected)
            {
                try
                {
                    while (sendingMessage.TryDequeue(out Message m))
                    {
                        if (m != null)
                        {
                            DoSendMessage(m);
                        }
                    }
                    try
                    {
                        Thread.Sleep(5);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception)
                {
                }
            }
        }
        public void AddMessage(Message message)
        {
            sendingMessage.Enqueue(message);
        }

        public void DoSendMessage(Message message)
        {
            if (message == null)
            {
                return;
            }
            try
            {
                if (!MySession.IsSocketConnected())
                {
                    MessageBox.Show("Mất kết nối với máy chủ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Debug.WriteLine("Domsg");
                message.WriteToStreamBigEndian(streamWrite);
           
                //MySession.dataStream.Flush();
                //Debug.WriteLine("Message sent successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending message: " + ex.Message);
            }
        }

        public void Dispose()
        {
            if (streamWrite != null)
            {
                streamWrite.Close();
                streamWrite = null;
            }
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }
    }
}
