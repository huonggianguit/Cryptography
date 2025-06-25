using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics; // dùng Debug.WriteLine

namespace Finance_Pro.TheImitationGame
{
    public class GenerateSearchKeywords
    {
        public static List<string> GetKeywords(string input)
        {
           // Debug.WriteLine("⏳ Bắt đầu xử lý từ khóa cho: " + input);

            // Bước 1: Chuẩn hóa chuỗi (lowercase, loại ký tự đặc biệt thành khoảng trắng)
            string cleaned = Regex.Replace(input.ToLower(), @"[^\p{L}\p{N}\s]", " ");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
          //  Debug.WriteLine("✅ Chuỗi sau chuẩn hóa: " + cleaned);

            // Bước 2: Tách thành các từ
            string[] words = cleaned.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
         //   Debug.WriteLine("✅ Tách được " + words.Length + " từ:");
            foreach (var w in words) Debug.WriteLine("   - " + w);

            // Sinh chuỗi prefix từ mảng words
            var prefixWithMark = SinhChuoiPrefix(words);
          //  Debug.WriteLine("✅ Tổng chuỗi prefix CÓ DẤU: " + prefixWithMark.Count);
            foreach (var p in prefixWithMark) Debug.WriteLine("   [+] " + p);

            // Sinh chuỗi bỏ dấu từ các chuỗi prefix
            var prefixNoMark = prefixWithMark
                .Select(RemoveDiacritics)
                .Where(x => x != null)
                .ToList();
         //   Debug.WriteLine("✅ Tổng chuỗi prefix KHÔNG DẤU: " + prefixNoMark.Count);
           // foreach (var p in prefixNoMark) Debug.WriteLine("   [-] " + p);

            // Gộp và loại trùng
            var final = prefixWithMark
                .Concat(prefixNoMark)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            //Debug.WriteLine("✅ Tổng kết quả cuối cùng (sau Distinct): " + final.Count);
            foreach (var k in final) Debug.WriteLine("   [*] " + k);

            return final;
        }

        // Hàm bỏ dấu tiếng Việt
        private static string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            string noDiacritics = new string(normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .Normalize(NormalizationForm.FormC);

            // Thêm dòng xử lý ký tự đặc biệt
            return noDiacritics.Replace('đ', 'd').Replace('Đ', 'D');
        }


        // Sinh chuỗi prefix từ danh sách từ
        public static List<string> SinhChuoiPrefix(string[] words)
        {
            var result = new HashSet<string>();
            for (int i = 0; i < words.Length; i++)
            {
                StringBuilder sb = new StringBuilder(words[i]);
                result.Add(sb.ToString());

                for (int j = i + 1; j < words.Length; j++)
                {
                    sb.Append(" ").Append(words[j]);
                    result.Add(sb.ToString());
                }
            }
            return result.ToList();
        }
    }
}
