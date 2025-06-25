using Finance_Pro.Dashboard;
using Finance_Pro.Login;
using Finance_Pro.TheImitationGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Finance_Pro.ServerConnect
{
    public class Controller : IMessageHandler
    {
        public static Controller instance = new Controller();
        public static Controller gI()
        {
            if (instance == null)
            {
                instance = new Controller();
            }
            return instance;
        }
        public void onConnectOK(bool isMain1)
        {

        }

        public void onConnectionFail(bool isMain1)
        {

        }

        public void onDisconnected(bool isMain1)
        {

        }
        public void onMessage(Message msg)
        {
            try
            {


                if (msg == null)
                {
                    return;
                }
                sbyte cmd = msg.GetCmd();
                sbyte subCmd = msg.GetSubCmd();
                Debug.WriteLine("oneMessage msg: cmd: " + cmd + " subCmd: " + subCmd);
                if (cmd == 6)
                {
                    switch (subCmd)
                    {
                        case 1:
                            {
                                bool flag = msg.ReadBool();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var loginWindow = Application.Current.Windows
                                        .OfType<LoginWindow>()
                                        .FirstOrDefault();

                                    if (loginWindow != null)
                                    {
                                        loginWindow.DoLoginWhenConnected(flag);
                                    }
                                });
                            }
                            break;
                        case 2:
                            {
                                bool flag = msg.ReadBool();
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var loginWindow = Application.Current.Windows
                                        .OfType<LoginWindow>()
                                        .FirstOrDefault();

                                    if (loginWindow != null)
                                    {
                                        loginWindow.DoRegister(flag);
                                    }
                                });
                            }
                            break;
                    }
                }
                // xử lý message
                if (cmd == 7)
                {
                    switch (subCmd)
                    {
                        case 0:
                            {

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dashboard = Application.Current.Windows
                                        .OfType<DashboardWindow>()
                                        .FirstOrDefault();

                                    if (dashboard != null)
                                    {
                                        int size = msg.ReadByte();
                                        Debug.WriteLine("Số file tìm thấy: " + size);
                                        DashboardWindow.fileIdMap.Clear();
                                        List<string> list = new List<string>();
                                        for (int i = 0; i < size; i++)
                                        {
                                            int sizeName = msg.ReadByte();
                                            byte[] nameB = msg.ReadBytes(sizeName);
                                            string name = Turing.DecryptAESString(nameB);
                                            DashboardWindow.fileIdMap[name]= nameB;
                                            Debug.WriteLine("name file : " + name);
                                            list.Add(name);
                                        }

                                        dashboard.LoadFiles(list);
                                    }
                                });
                            }
                            break;
                        case 1:
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dashboard = Application.Current.Windows
                                        .OfType<DashboardWindow>()
                                        .FirstOrDefault();

                                    if (dashboard != null)
                                    {
                                        dashboard.ShowMessageBox("Không tìm thấy File: " );
                                    }
                                });
                            }
                            break;
                    }
                }
                if (cmd == 9)
                {
                    switch (subCmd)
                    {
                        case 0:
                            {
                                int fileL = msg.ReadByte();
                                byte[] fileID = msg.ReadBytes(fileL);

                                ushort rawL = msg.ReadUInt16BE();
                                byte[] raw = msg.ReadBytes(rawL);
                                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                string outputFolder = Path.Combine(desktopPath, "OutputClientDoAn");
                                if (!Directory.Exists(outputFolder))
                                {
                                    Directory.CreateDirectory(outputFolder);
                                }
                                string fileName = Turing.DecryptAESString(fileID);
                                string fullPath = Path.Combine(outputFolder, fileName);
                                File.WriteAllBytes(fullPath, raw);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dashboard = Application.Current.Windows
                                        .OfType<DashboardWindow>()
                                        .FirstOrDefault();

                                    if (dashboard != null)
                                    {
                                        dashboard.ShowMessageBox("Nhận file thành công tại: " + outputFolder);
                                    }
                                });
                            }
                            break;
                    }
                }
                if (cmd == 10)
                {
                    switch (subCmd)
                    {
                        case 0:
                            {
                                int fileL = msg.ReadByte();
                                byte[] fileID = msg.ReadBytes(fileL);
                                int rawL = msg.ReadInt24BE();
                                byte[] raw = Turing.DecryptAES( msg.ReadBytes(rawL));
                                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                string outputFolder = Path.Combine(desktopPath, "OutputClientDoAn");
                                if (!Directory.Exists(outputFolder))
                                {
                                    Directory.CreateDirectory(outputFolder);
                                }
                                string fileName = Turing.DecryptAESString(fileID);
                                string fullPath = Path.Combine(outputFolder, fileName);
                                File.WriteAllBytes(fullPath, raw);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var dashboard = Application.Current.Windows
                                        .OfType<DashboardWindow>()
                                        .FirstOrDefault();

                                    if (dashboard != null)
                                    {
                                        dashboard.ShowMessageBox("Nhận file thành công tại: " + outputFolder);
                                    }
                                });
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ Lỗi: " + ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (msg != null)
                {
                    msg.Dispose();
                    msg = null;
                }
             
            }
        }

    }
}
