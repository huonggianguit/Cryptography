using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Finance_Pro.TheImitationGame
{
    public class Turing
    {
        private static Turing _instance = null;
        public static Turing gI()
        {
            if (_instance == null)
            {
                _instance = new Turing();
            }
            return _instance;
        }
        public Turing()
        {
            SetSecretKey(SECRET_KEY);
        }

        public void SetSearchKey(string key)
        {
            _searchKey = Encoding.UTF8.GetBytes(key);
            //Encoding.UTF8.GetBytes(key) sẽ chuyển string → byte[], vì HMAC yêu cầu key ở dạng byte.
        }
        public static string SECRET_KEY { get; private set; } = "helloworld";
        private static byte[] _aesKey = null;
        private static byte[] _searchKey = null;

        //AES-128 → 16 byte (128 bit)
        // AES-192 → 24 byte (192 bit)
        // AES-256 → 32 byte (256 bit)
        //SHA-256 băm ra 32 byte (256 bit), nên ta sẽ lấy 16 byte đầu tiên để làm khóa AES-128.
        //_aesKey = SHA256("my key") → Take(16) → key AES-128

        // Cần thêm các bước sau để bảo vệ key khỏi brute-force
        //  salt,vòng lặp (iteration), chống dictionary attack, Thư viện tham khảo  PBKDF2, bcrypt, hoặc Argon2

        public static void SetSecretKey(string secretKey)
        {
            // Chỉ cập nhật nếu key thay đổi
            if (SECRET_KEY != secretKey || _aesKey==null)
            {
                SECRET_KEY = secretKey;
                using (SHA256 sha = SHA256.Create())
                {
                    _aesKey = sha.ComputeHash(Encoding.UTF8.GetBytes(SECRET_KEY)).Take(16).ToArray();
                }
            }
        }
        // ==== GIẢI MÃ ====
        //Bọc lại đề chuyển thành sbyte[] chuyển sang Message Stream.
        public static sbyte[] DecryptAES(sbyte[] processedData)
        {
            if (processedData == null || processedData.Length < 16)
                throw new ArgumentException("ABC");

            try
            {
                // Chuyển đổi sbyte[] sang byte[] bằng Buffer.BlockCopy
                byte[] encryptedData = new byte[processedData.Length];
                Buffer.BlockCopy(processedData, 0, encryptedData, 0, processedData.Length);

                byte[] decryptedData = DecryptAES(encryptedData);

                // Chuyển byte[] về sbyte[]
                sbyte[] result = new sbyte[decryptedData.Length];
                Buffer.BlockCopy(decryptedData, 0, result, 0, decryptedData.Length);
                return result;
            }
            catch (Exception)
            {
                throw new InvalidDataException("ABC");
            }
        }

        //Hàm giải mã AES với byte[] đầu vào
        public static byte[] DecryptAES(byte[] encryptedData)
        {
            if (encryptedData == null || encryptedData.Length < 16)
                throw new ArgumentException("Dữ liệu không hợp lệ");

            if (_aesKey == null)
                throw new InvalidOperationException("SECRET_KEY chưa được thiết lập");

            try
            {
                // Lấy IV từ 16 byte đầu tiên
                byte[] iv = new byte[16];
                Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);

                int cipherTextLength = encryptedData.Length - 16;
                byte[] cipherText = new byte[cipherTextLength];
                Buffer.BlockCopy(encryptedData, 16, cipherText, 0, cipherTextLength);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _aesKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        return decryptor.TransformFinalBlock(cipherText, 0, cipherTextLength);
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException("Giải mã thất bại");
            }
        }

        public static string DecryptAESString(byte[] encryptedData)
        {
            try
            {
               
                byte[] decryptedBytes = DecryptAES(encryptedData);

                // Bỏ 2 byte đầu (độ dài)
                if (decryptedBytes.Length >= 2)
                {
                    decryptedBytes = decryptedBytes.Skip(2).ToArray();
                }

                string result = Encoding.UTF8.GetString(decryptedBytes);
         
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ [AES] Lỗi giải mã: " + ex.Message);
                return "";
            }
        }



        // ==== MÃ HÓA ====
        public static byte[] EncryptAES(byte[] plainData)
        {
            if (plainData == null || plainData.Length == 0)
                throw new ArgumentException("Dữ liệu không hợp lệ");

            if (_aesKey == null)
                throw new InvalidOperationException("SECRET_KEY chưa được thiết lập");
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _aesKey;
                    aes.GenerateIV(); // Sinh IV ngẫu nhiên
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] cipherText = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
                        byte[] output = new byte[aes.IV.Length + cipherText.Length];
                        Buffer.BlockCopy(aes.IV, 0, output, 0, aes.IV.Length);
                        Buffer.BlockCopy(cipherText, 0, output, aes.IV.Length, cipherText.Length);
                        return output;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException("Mã hóa thất bại");
            }
        }

        public static sbyte[] EncryptAES(sbyte[] plainData)
        {
            if (plainData == null || plainData.Length == 0)
                throw new ArgumentException("Dữ liệu không hợp lệ");

            byte[] input = new byte[plainData.Length];
            Buffer.BlockCopy(plainData, 0, input, 0, plainData.Length);

            byte[] encrypted = EncryptAES(input);

            sbyte[] result = new sbyte[encrypted.Length];
            Buffer.BlockCopy(encrypted, 0, result, 0, encrypted.Length);
            return result;
        }


        // ==== MÃ HÓA HMac Keyword ====

        public static byte[]? ComputeHmac(string keyword)
        {
            if (string.IsNullOrEmpty(keyword) || _searchKey == null)
                return null;
            //Tưởng: Đổi vs Hash, A băm thành X (X ko phải là A, X chỉ định danh cho A, ko thể từ X mà lấy ra A dc(đối vs file lớn), còn đối vs string là phải Brute-force.. ),
            //Đối với mã hóa, A thành X (X lúc này chính là A, Nhưng là A ở thế giới khác, Chỉ cần có key chuyển đổi thế giới là sẽ xem được
            // HMac nó có Key, không thể đạo ngược như Hash, Nhưng mà thay vì A qua Hash thành X, thì giờ A sẽ thành X', X' là X trộn vs key. Làm tăng độ khó khi Brute-force
            //HMAC(key, data) = hash(key + data + key). Nói chung là Brute-force nó mò từ từ cho ra String nào đó băm ra dc X. nó suy ra String gốc, giờ trộn vs Key nữa để nó mò ko ra dc luôn,
            //Vì định danh là định danh cả string + key, nó phải có cả key trộn vào ms dò dc. Mà mình bảo mật key, nên giảm khả năng khi Brute-force
            // Bản chất HMAC nó là 1 hàm băm, Nó biến A thành X (X định danh A, X ko phải là A ở thế giới khác). Nhưng cực khó để mò ngẫu nhiên 1 value để ra X, rồi suy ra A, Vì Hmac khi nó định danh nó trộn cả key vào nữa.
            byte[] messageBytes = Encoding.UTF8.GetBytes(keyword);

            using (var hmac = new HMACSHA256(_searchKey))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return hashBytes;
            }
            //  tạo một đối tượng HMAC (Hash-based Message Authentication Code) sử dụng thuật toán băm là SHA-256,_searchKey là key bí mật (secret key) dùng trong thuật toán HMAC.
            // Công thức chuẩn HMAC(K, m) = H((K' ⊕ opad) || H((K' ⊕ ipad) || m)),H: là SHA-256 trong trường hợp này,K': khóa được padding thành đúng độ dài,m: là dữ liệu (message)
            // ,⊕: XOR,opad, ipad: padding hằng số,Nói ngắn: HMAC = SHA256( key + SHA256(key + message) )
            // dùng Using để sau quá trình này nó tự động giải phóng bộ nhớ, tránh rò rỉ bộ nhớ (memory leak).
            // BitConverter.ToString(...): chuyển từng byte thành dạng hex (AB-3C-1F-...).
            // .Replace("-", ""): bỏ dấu - giữa các byte.
            //.ToLower(): chuyển về chữ thường.
            //Kết quả là một chuỗi hexadecimal 64 ký tự
        }
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2")); // "x2" giữ đúng định dạng 2 chữ số, chữ thường
            }
            return sb.ToString();
        }

    }
}
