using MySqlX.XDevAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Finance_Pro.ServerConnect
{
    public class MySession
    {
        private static MySession instance = new MySession();

        public static MySession gI()
        {
            if (instance == null)
            {
                instance = new MySession();
            }
            return instance;
        }
        private static TcpClient sc;
        private string host = "localhost";
        private int port = 12345;
        public static NetworkStream dataStream;

        public static bool connected;
        public static bool connecting;

        public static bool isMainSession = true;

        public static Thread initThread; // Loop Chính. 
        public static Thread collectorThread; // Loop Nhận tin nhắn từ server
        public static Thread sendThread; // Loop Gửi tin nhắn đến server

        public bool isCancel = false;

        private static int timeConnected;
        public static int count;

        public bool isConnected()
        {
            return connected;
        }
        public static IMessageHandler messageHandler;
        public void setHandler(IMessageHandler msgHandler)
        {
            messageHandler = msgHandler;
        }

        public void connect()
        {
            if (!connected && !connecting)
            {
                initThread = new Thread(NetworkInit);
                initThread.Start();
            }
        }

        private void NetworkInit()
        {
            isCancel = false;
            connecting = true;
            Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest; // Xét mức độ ưu tiên cao cho Theard (ko hiểu sao C# có trò này, cái này bửa thấy ngkhac xài ^^)
            connected = true;
            try
            {
                doConnect(host, port);
                messageHandler.onConnectOK(isMainSession);
            }
            catch (Exception)
            {
                if (messageHandler != null)
                {
                    close();
                    messageHandler.onConnectionFail(isMainSession);
                }
            }
        }

        public void doConnect(string host, int port)
        {
            sc = new TcpClient(); // Tạo socket TCP// Này Giống như IO của Java thôi, Dù có xài ngôn ngữ nào thì vẫn dùng cơ chế chơi vs OS như nhau, chỉ khác về Syntax, Lib
            sc.Connect(host, port); // Mở cộng tại host, port
            dataStream = sc.GetStream(); // Đoạn này hình như nó bọc Socket lại, nó trả về 1 thứ mà có thể tương tác vs socket như đọc, ghi....
            // Đoạn này tạo 2 buffer để đọc và ghi dữ liệu, giống như DataInputStream và DataOutputStream trong Java

            sendThread = new Thread(MessageSender.gI().Run);
            collectorThread = new Thread(MessageCollector.gI().Run);

            sendThread.Start(); // Theard gửi msg 
            collectorThread.Start(); //theard nhận msg
            timeConnected = currentTimeMillis();

            sendMessage(new Message(6, 0, IOUtils.StringToUTFBytes(App.clientKey)));

        }
        public void close()
        {
            cleanNetwork();
        }
        private static void cleanNetwork()
        {
            try
            {
                connected = false;
                connecting = false;
                if (sc != null)
                {
                    sc.Close();
                    sc = null;
                }
                if (dataStream != null)
                {
                    dataStream.Close();
                    dataStream = null;
                }
                MessageSender.gI().Dispose(); // Giải phóng bộ nhớ của MessageSender
                MessageSender.gI().Dispose(); // Giải phóng bộ nhớ của MessageCollector
                sendThread = null;
                collectorThread = null;
            }
            catch (Exception)
            {
            }
        }
        public void sendMessage(Message message)
        {
            MessageSender.gI().AddMessage(message);
        }


        public static int currentTimeMillis()
        {
            return Environment.TickCount;
        }
        public static bool IsSocketConnected()
        {
            TcpClient client = sc;
            try
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    // Kiểm tra socket có thể đọc mà không bị đóng
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // socket đã đóng bởi remote
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

    }

}


