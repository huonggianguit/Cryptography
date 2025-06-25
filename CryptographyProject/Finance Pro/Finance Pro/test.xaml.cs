using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Finance_Pro.Data;
using Microsoft.Win32;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
namespace Finance_Pro
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window
    {
        public test()
        {
            InitializeComponent();
        }
        private string selectedImagePath = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Tạo một hộp thoại mở file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

            // Hiển thị hộp thoại và kiểm tra nếu người dùng chọn file
            if (openFileDialog.ShowDialog() == true)
            {

                // Lấy đường dẫn file đã chọn
                 selectedImagePath = openFileDialog.FileName;


                // Hiển thị ảnh xem trước trong Image Control
                pic.Source = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string query = $"UPDATE {txtTable.Text} SET image = @data WHERE name = @name";

            try
            {
                using (var connection = new ConnectSQL()) // Tạo đối tượng ConnectSQL
                {
                    using (var cmd = new MySqlCommand(query, connection.GetConnection()))
                    {
                        // Đọc file ảnh thành mảng byte
                        byte[] imageBytes = File.ReadAllBytes(selectedImagePath);

                        // Thêm tham số vào câu truy vấn
                        cmd.Parameters.AddWithValue("@data", imageBytes);
                        // Thêm tham số vào câu truy vấn
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim()); // Sử dụng tham số hóa

                        // Thực hiện truy vấn
                        int rowsAffected = cmd.ExecuteNonQuery();

                        MessageBox.Show(rowsAffected > 0 ? "Cập nhật thành công!" : "Không có dữ liệu nào được cập nhật.", "Thông báo");
                    }
                }
                }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string query = $"SELECT image FROM {txtTable.Text} WHERE name = @name";

            try
            {
                using (var connection = new ConnectSQL()) // Tạo đối tượng ConnectSQL
                {
                    using (var cmd = new MySqlCommand(query, connection.GetConnection()))
                    {
                        // Thêm tham số vào câu truy vấn
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim()); // Sử dụng tham số hóa

                        // Thực thi câu truy vấn và lấy kết quả
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["image"] != DBNull.Value)
                                {
                                    byte[] imageBytes = (byte[])reader["image"];
                                    MessageBox.Show($"Kích thước mảng ảnh: {imageBytes.Length} byte."); // Kiểm tra kích thước ảnh

                                    if (imageBytes.Length > 0)
                                    {
                                        // Chuyển byte[] thành hình ảnh và gán vào control pic.Source
                                        using (var ms = new MemoryStream(imageBytes))
                                        {
                                            BitmapImage bitmap = new BitmapImage();
                                            bitmap.BeginInit();
                                            bitmap.StreamSource = ms;
                                            bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Chế độ lưu ảnh sau khi tải
                                            bitmap.EndInit();

                                            // Đảm bảo ảnh đã được xử lý xong trước khi gán vào control Image
                                            pic.Source = bitmap;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ảnh trống hoặc không hợp lệ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Không tìm thấy ảnh cho người dùng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy người dùng với tên này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
