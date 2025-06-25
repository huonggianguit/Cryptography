using Finance_Pro.Dashboard;
using Finance_Pro.Data;
using Finance_Pro.ServerConnect;
using Finance_Pro.TheImitationGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace Finance_Pro.Login
{
    public partial class LoginWindow : Window
    {
        private bool isRegisterMode = false;
        public LoginWindow()
        {
            InitializeComponent();
            CalculateParametersScreen();
            LoadSavedCredentials();
            this.DataContext = new LoginViewModel();
        }
        #region CalculateParametersScreen

        public double originalWidth;
        public double originalHeight;



        private void CalculateParametersScreen()
        {
            var dpiScale = VisualTreeHelper.GetDpi(this); // Trả về DpiScale cho cửa sổ

            // Lấy giá trị DPI scale
            double dpiX = dpiScale.DpiScaleX; // Tỷ lệ DPI theo chiều ngang
            double dpiY = dpiScale.DpiScaleY; // Tỷ lệ DPI theo chiều dọc

            this.Width /= dpiX;



        }
        #endregion

        private void LoadSavedCredentials()
        {
            // Kiểm tra nếu người dùng đã lưu tài khoản
            if (Properties.Settings.Default.RememberMe)
            {
                userName.Text = Properties.Settings.Default.userName;
                passWord.Password = Properties.Settings.Default.passWord;
                toogleBut.isCheckRememBerMe = true;

                txtBeforeUsername.FontSize = 18;
                txtBeforeUsername.Opacity = 1;
                txtBeforeUsername.Margin = new System.Windows.Thickness(0);
                txtBeforeUsername.Foreground = System.Windows.Media.Brushes.Red;

                txtBeforePass.Foreground = System.Windows.Media.Brushes.Red;
                txtBeforePass.FontSize = 18;
                txtBeforePass.Opacity = 1;
                txtBeforePass.Margin = new System.Windows.Thickness(0);

                toogleBut.toggleImage.Source = (ImageSource)FindResource("ToggleBut2");
                toogleBut.toggleBackImage.ImageSource = (ImageSource)FindResource("BackBut2");

            }
        }
        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!isRegisterMode)
            {
                btnLogin.Content = "REGISTER";
                btnRegister.Content = "Login";
                isRegisterMode = true;
            }
            else
            {
                btnLogin.Content = "LOGIN";
                btnRegister.Content = "Register";
                isRegisterMode = false;
            }
        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private bool isShowPass = false;
        private void txtPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isShowPass)
            {
                passWord.Password = txtPass.Text;
            }
        }

        private void passWord_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPass.Text.Length == 0 && !isShowPass)
            {
                txtPass.Text = "SslClientHelloInfo";
            }
            if (string.IsNullOrEmpty(passWord.Password))
            {
                txtPass.Text = string.Empty;
            }
        }
        private void btnShowPass_Click(object sender, RoutedEventArgs e)
        {
            isShowPass = !isShowPass;
            if (isShowPass)
            {
                btnShowPass.Content = FindResource("eyeIcon");
                txtPass.Text = passWord.Password;
                passWord.Visibility = Visibility.Collapsed;
                txtPass.Visibility = Visibility.Visible;

            }
            else
            {
                btnShowPass.Content = FindResource("eyeSlashedIcon");
                passWord.Visibility = Visibility.Visible;
                txtPass.Visibility = Visibility.Collapsed;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            byte subCmd = 1;
            if (isRegisterMode)
            {
                subCmd = 2;
            }
            Message msg = new Message(6, subCmd);
            try
            {
                msg.WriteUTF(userName.Text);
                msg.WriteUTF(passWord.Password);
                MySession.gI().sendMessage(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
               // msg.Dispose(); // Giải phóng tài nguyên của Message sau khi gửi
            }
        }

        public void DoLoginWhenConnected(bool flag)
        {
            Debug.WriteLine("DoLoginWhenConnected");
            if (flag)
            {
                if (toogleBut.isCheckRememBerMe)
                {
                    //Đoạn này nên thêm lưu dạng mã hóa userName và Password, Sau đó khi mở chương trình thì sẽ qua bước convert lại. 
                    Properties.Settings.Default.userName = userName.Text;
                    Properties.Settings.Default.passWord = passWord.Password;
                    Properties.Settings.Default.RememberMe = true;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.Reset();
                }
                // Chuyển password sang Byte[] dùng để Hmac
                Turing.gI().SetSearchKey(passWord.Password);
                passWord.Password = "";
                userName.Text = "";
                // Mở cửa sổ Dashboard
                var dashboard = new Dashboard.DashboardWindow(); // Điều chỉnh namespace nếu cần
                dashboard.Show();

                // Đóng cửa sổ hiện tại
                this.Close();
            }
            else
            {
                // Đăng nhập thất bại
                Properties.Settings.Default.Reset();
                MessageBox.Show("Nhập sai mật khẩu hoặc tài khoản", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void DoRegister(bool flag)
        {
            if (flag)
            {
                btnLogin.Content = "LOGIN";
                btnRegister.Content = "Register";
                isRegisterMode = false;
                MessageBox.Show("Đăng ký thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
