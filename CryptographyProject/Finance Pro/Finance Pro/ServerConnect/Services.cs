using Finance_Pro.Dashboard;
using Finance_Pro.TheImitationGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Finance_Pro.ServerConnect
{
    public class Services
    {
        private static Services _instance = null;

        public static Services gI()
        {
            return _instance ?? (_instance = new Services());
            //g toán tử null-coalescing (??) kết hợp với gán. C# có trò này khá hay tương đương vs return _instance != null ? _instance : (_instance = new Services());
        }

        private Services()
        {
            // Constructor private để chặn khởi tạo ngoài, cũng chưa đụng tới trường hợp nào khởi tạo ngoài kiểu này, chắc do dự án lớn hay gì đấy quy chuẩn lập trình :)))
        }
      

        public bool sendFile(string Name)
        {
          
            Debug.WriteLine($"📂 Bắt đầu gửi file: {Name}");

            FileInfo file = new FileInfo(Name);
            string filename = file.Name;
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            Debug.WriteLine($"📄 Tên file gốc: {filename}");
            Debug.WriteLine($"📄 Không đuôi: {filenameWithoutExtension}");

            List<string> keywords = GenerateSearchKeywords.GetKeywords(filenameWithoutExtension);
            Debug.WriteLine($"🔑 Keywords: {string.Join(", ", keywords)}");

            if (keywords.Count <= 0)
            {
                Debug.WriteLine("❌ Không có từ khóa nào được tạo");
                return false;
            }

            string firstKeyword = keywords[0];
            keywords.RemoveAt(0);
            Debug.WriteLine($"🔑 First keyword: {firstKeyword}");

            byte[]? tokenFrist = Turing.ComputeHmac(firstKeyword);
            if (tokenFrist == null || tokenFrist.Length != 32)
            {
                Debug.WriteLine("❌ Lỗi tính toán HMAC đầu tiên");
                return false;
            }
            Debug.WriteLine($"✅ TokenFirst (hex): {Turing.ToHexString(tokenFrist)}");

            byte[] rawFileID = Turing.EncryptAES(IOUtils.StringToUTFBytes(filename));
            Debug.WriteLine($"🔐 rawFileID length: {rawFileID.Length}");

            if (rawFileID.Length >= 255)
            {
                Debug.WriteLine("❌ Tên file quá dài sau khi mã hóa");
                return false;
            }

            byte[] raw = File.ReadAllBytes(file.FullName);
            Debug.WriteLine($"📥 raw size: {raw.Length}");

            byte[] rawEnc = Turing.EncryptAES(raw);
            if (rawEnc == null)
            {
                Debug.WriteLine("❌ rawEnc bị null");
                return false;
            }
            Debug.WriteLine($"🔒 rawEnc size: {rawEnc.Length}");

            byte subCmd = 0;
            if (rawEnc.Length <= ushort.MaxValue)
                subCmd = 0;
            else if (rawEnc.Length <= 0xFFFFFF)
                subCmd = 1;
            else
            {
                Debug.WriteLine("❌ Payload quá lớn, không hỗ trợ");
                return false;
            }

            Message msg = new Message(7, subCmd);
            try
            {
                Debug.WriteLine("✍️ Bắt đầu ghi message...");
                msg.WriteBytes(tokenFrist);
                msg.WriteByte((byte)rawFileID.Length);
                msg.WriteBytes(rawFileID);

                if (subCmd == 0)
                {
                    msg.WriteUInt16BigEndian((ushort)rawEnc.Length);
                }
                else
                {
                    msg.WriteUInt24BigEndian(rawEnc.Length);
                }

                msg.WriteBytes(rawEnc);
                Debug.WriteLine($"📤 Gửi message CMD 7, subCmd {subCmd}");
                MySession.gI().sendMessage(msg);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"❗ Exception khi gửi message file: {e.Message}");
                return false;
            }

            if (keywords.Count <= 0)
            {
                Debug.WriteLine("📦 Không còn từ khóa nào để gửi tiếp");
                return true;
            }

            List<byte[]> listToken = new List<byte[]>();
            for (int i = 0; i < keywords.Count; i++)
            {
                byte[]? token = Turing.ComputeHmac(keywords[i]);
                if (token == null)
                {
                    Debug.WriteLine($"⚠️ Token null tại keyword: {keywords[i]}");
                    continue;
                }
                listToken.Add(token);
            }

            int countToken = listToken.Count;
            Debug.WriteLine($"📊 Số lượng token còn lại: {countToken}");

            if (countToken > ushort.MaxValue)
            {
                Debug.WriteLine("❗ Token quá nhiều, không gửi tiếp");
                return true;
            }

            Message msg1 = new Message(8, 0);
            try
            {
                msg1.WriteByte((byte)rawFileID.Length);
                msg1.WriteBytes(rawFileID);
                msg1.WriteUInt16BigEndian((ushort)countToken);

                List<string> list = new List<string>();
                for (int i = 0; i < countToken; i++)
                {
                    byte[] token = listToken[i];
                    msg1.WriteBytes(token);
                    list.Add(Turing.ToHexString(token));
                }

                Debug.WriteLine("📤 Gửi token còn lại:");
                Debug.WriteLine(string.Join("\n", list));
                MySession.gI().sendMessage(msg1);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"❗ Exception khi gửi list token: {e.Message}");
                return true;
            }

            Debug.WriteLine("✅ Gửi file và token thành công");
            return true;
        }

        public void sendWordSearch(string wordSearch)
        {
            byte[]? token = Turing.ComputeHmac(wordSearch);
            if (token == null || token.Length != 32)
            {
                Debug.WriteLine("❌ Lỗi tính toán HMAC cho wordSearch");
                return ;
            }

            Message msg = new Message(9, 0);
            try
            {
                msg.WriteBytes(token);            
                Debug.WriteLine($"Search Token {Turing.ToHexString(token)}");
                MySession.gI().sendMessage(msg);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"❗ Exception khi gửi message file: {e.Message}");
                return ;
            }

        }
        public void downLoadFIle(string filename)
        {
          
            int dotIndex = filename.LastIndexOf('.');
            string nameOnly = (dotIndex > 0) ? filename.Substring(0, dotIndex) : filename;

            List<string> keywords = GenerateSearchKeywords.GetKeywords(nameOnly);
            List<byte[]> listToken = new List<byte[]>();
            for (int i = 0; i < keywords.Count; i++)
            {
                byte[]? token = Turing.ComputeHmac(keywords[i]);
                if (token == null)
                {
                    Debug.WriteLine($"⚠️ Token null tại keyword: {keywords[i]}");
                    continue;
                }
                listToken.Add(token);
            }
            int countToken = listToken.Count;   
            if (countToken > ushort.MaxValue)
            {
                Debug.WriteLine("❗ Token quá nhiều, không gửi tiếp");
                return ;
            }
            if (DashboardWindow.fileIdMap.TryGetValue(filename, out byte[] rawFileID))
            {
                Message msg = new Message(8, 1);
                try
                {
                    msg.WriteByte((byte)rawFileID.Length);
                    msg.WriteBytes(rawFileID);
                    msg.WriteUInt16BigEndian((ushort)countToken);
                    for (int i = 0; i < countToken; i++)
                    {
                        byte[] token = listToken[i];
                        msg.WriteBytes(token);
                    }
                    MySession.gI().sendMessage(msg);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"❗ Exception khi gửi file Down: {e.Message}");

                }
            }
            else
            {
                Debug.WriteLine("❌ Không tìm thấy file ID: " + filename);
            }
           
        }
    }
}
