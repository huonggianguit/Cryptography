
using Finance_Pro.Data;
using Finance_Pro.Models;
using Finance_Pro.ServerConnect;
using Microsoft.WindowsAPICodePack.Shell;
using Mysqlx.Crud;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaBrushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;
using System;
using System.Collections.Generic;
namespace Finance_Pro.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        public static Dictionary<string, byte[]> fileIdMap = new Dictionary<string, byte[]>();
        public static List<string> listFileSelect = new List<string>();
        public DashboardWindow()
        {
            InitializeComponent();
            CalculateParametersScreen();
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
            if (dpiX > 1.25)
                dpiX = 1.25;

            this.Height /= dpiX;
            this.Width /= dpiX;

        }
        #endregion
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        string WindowsState = "Normal";
        double WidthNormal;
        double HeightNormal;

        double LeftNormal;
        double TopNormal;

        //private void btnMaximize_Click(object sender, RoutedEventArgs e)
        //{

        //    if (WindowsState == "Maximized")
        //    {
        //        WindowsState = "Normal";
        //        this.Width = WidthNormal;
        //        this.Height = HeightNormal;
        //        this.Left = LeftNormal;
        //        this.Top = TopNormal;
        //        TaskbarBorder.CornerRadius = new CornerRadius(15);
        //    }
        //    else
        //    {
        //        WindowsState = "Maximized";

        //        WidthNormal = this.Width;
        //        HeightNormal = this.Height;

        //        LeftNormal = this.Left;
        //        TopNormal = this.Top;

        //        // Kích thước vùng làm việc khả dụng (trừ Taskbar)
        //        double workAreaWidth = SystemParameters.WorkArea.Width;
        //        double workAreaHeight = SystemParameters.WorkArea.Height;

        //        // Chuyển về trạng thái bình thường để thay đổi kích thước
        //       // this.WindowState = WindowState.Normal;

        //        // Đặt kích thước và vị trí
        //        this.Left = SystemParameters.WorkArea.Left;
        //        this.Top = SystemParameters.WorkArea.Top;
        //        this.Width = workAreaWidth;
        //        this.Height = workAreaHeight;
        //        TaskbarBorder.CornerRadius = new CornerRadius(2);
        //    }
        //}

        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.Shutdown();

        //}

        private void Mainview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ngăn sự kiện lan truyền lên Border cha
            e.Handled = true;
        }

        private void TaskbarBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //#region resizeThumb
        //// Phương thức con để thay đổi Cursor theo tên của Thumb
        private void SetCursorBasedOnThumbName(Thumb thumb)
        {
            if (thumb == null) return;

            // Lấy tên của đối tượng thumb
            string objectName = thumb.Name;

            // Dùng switch để thay đổi Cursor theo từng tên của đối tượng
            switch (objectName)
            {
                case "SizeWindows_Left":
                    thumb.Cursor = Cursors.SizeWE;  // Đặt Cursor là SizeWE cho Thumb bên trái
                    break;

                case "SizeWindows_NWSE_L":
                    thumb.Cursor = Cursors.SizeNWSE;  // Đặt Cursor là SizeNWSE cho Thumb góc trái trên
                    break;

                case "SizeWindows_Top":
                    thumb.Cursor = Cursors.SizeNS;  // Đặt Cursor là SizeNS cho Thumb phía trên
                    break;

                case "SizeWindows_NESW_R":
                    thumb.Cursor = Cursors.SizeNESW;  // Đặt Cursor là SizeNESW cho Thumb góc phải trên
                    break;

                case "SizeWindows_Right":
                    thumb.Cursor = Cursors.SizeWE;  // Đặt Cursor là SizeWE cho Thumb bên phải
                    break;

                case "SizeWindows_NWSE_R":
                    thumb.Cursor = Cursors.SizeNWSE;  // Đặt Cursor là SizeNWSE cho Thumb góc phải dưới
                    break;

                case "SizeWindows_Bottom":
                    thumb.Cursor = Cursors.SizeNS;  // Đặt Cursor là SizeNS cho Thumb phía dưới
                    break;

                case "SizeWindows_NESW_L":
                    thumb.Cursor = Cursors.SizeNESW;  // Đặt Cursor là SizeNESW cho Thumb góc trái dưới
                    break;

                default:
                    thumb.Cursor = Cursors.Arrow;  // Nếu không phải các trường hợp trên, mặc định là mũi tên
                    break;
            }
        }
        private void SizeWindows_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //CompositionTarget.Rendering += OnRendering;
            var thumb = sender as Thumb; // Kiểm tra xem sender có phải là Thumb không
            if (thumb != null)
            {
                SetCursorBasedOnThumbName(thumb);
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == "Nhập tên file")
            {
                tb.Text = "";
                tb.Foreground = MediaBrushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Nhập tên file";
                tb.Foreground = MediaBrushes.Gray;
            }
        }

        private void SizeWindows_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null) return;

            string objectName = thumb.Name;

            // Đặt giá trị kích thước tối thiểu
            double minWidth = 500;  // Kích thước chiều rộng tối thiểu 780
            double minHeight = 660; // Kích thước chiều cao tối thiểu 600

            switch (objectName)
            {
                case "SizeWindows_Left":
                    // Kéo từ trái -> giảm chiều rộng và thay đổi vị trí của đối tượng

                    double newWidthLeft = this.Width - e.HorizontalChange;
                    if (newWidthLeft >= minWidth) // Kiểm tra chiều rộng không nhỏ hơn minWidth
                    {
                        this.Width = newWidthLeft;
                        this.Left += e.HorizontalChange;
                    }
                    break;

                case "SizeWindows_NWSE_L":
                    // Kéo góc trái trên -> thay đổi cả chiều rộng và chiều cao
                    double newWidthNWSE_L = this.Width - e.HorizontalChange;
                    double newHeightNWSE_L = this.Height - e.VerticalChange;
                    if (newWidthNWSE_L >= minWidth && newHeightNWSE_L >= minHeight)
                    {
                        this.Width = newWidthNWSE_L;
                        this.Height = newHeightNWSE_L;
                        this.Left += e.HorizontalChange;
                        this.Top += e.VerticalChange;
                    }
                    break;

                case "SizeWindows_Top":
                    // Kéo từ trên -> giảm chiều cao và thay đổi vị trí của đối tượng
                    double newHeightTop = this.Height - e.VerticalChange;
                    if (newHeightTop >= minHeight)
                    {
                        this.Height = newHeightTop;
                        this.Top += e.VerticalChange;
                    }
                    break;

                case "SizeWindows_NESW_R":
                    // Kéo góc phải trên -> thay đổi cả chiều rộng và chiều cao
                    double newWidthNESW_R = this.Width + e.HorizontalChange;
                    double newHeightNESW_R = this.Height - e.VerticalChange;
                    if (newWidthNESW_R >= minWidth && newHeightNESW_R >= minHeight)
                    {
                        this.Width = newWidthNESW_R;
                        this.Height = newHeightNESW_R;
                        this.Top += e.VerticalChange;
                    }
                    break;

                case "SizeWindows_Right":
                    // Kéo từ phải -> tăng chiều rộng và giữ nguyên vị trí
                    double newWidthRight = this.Width + e.HorizontalChange;
                    if (newWidthRight >= minWidth)
                    {
                        this.Width = newWidthRight;
                        this.Height += 0.001;
                    }
                    this.InvalidateMeasure();

                    break;

                case "SizeWindows_NWSE_R":
                    // Kéo góc phải dưới -> thay đổi cả chiều rộng và chiều cao
                    double newWidthNWSE_R = this.Width + e.HorizontalChange;
                    double newHeightNWSE_R = this.Height + e.VerticalChange;
                    if (newWidthNWSE_R >= minWidth && newHeightNWSE_R >= minHeight)
                    {
                        this.Width = newWidthNWSE_R;
                        this.Height = newHeightNWSE_R;
                    }
                    break;

                case "SizeWindows_Bottom":
                    // Kéo từ dưới -> tăng chiều cao và giữ nguyên vị trí
                    double newHeightBottom = this.Height + e.VerticalChange;
                    if (newHeightBottom >= minHeight)
                    {
                        this.Height = newHeightBottom;
                    }
                    break;

                case "SizeWindows_NESW_L":
                    // Kéo góc trái dưới -> thay đổi cả chiều rộng và chiều cao
                    double newWidthNESW_L = this.Width - e.HorizontalChange;
                    double newHeightNESW_L_2 = this.Height + e.VerticalChange;
                    if (newWidthNESW_L >= minWidth && newHeightNESW_L_2 >= minHeight)
                    {
                        this.Width = newWidthNESW_L;
                        this.Height = newHeightNESW_L_2;
                        this.Left += e.HorizontalChange;
                    }
                    break;

                default:
                    break;
            }
        }

        private void SizeWindows_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //  CompositionTarget.Rendering -= OnRendering;
            var thumb = sender as Thumb;
            SetCursorBasedOnThumbName(thumb);
        }
        // Xử lý khi cập nhật giao diện
        private void OnRendering(object sender, EventArgs e)
        {
            // Lệnh vẽ lại nếu cần thiết (hoặc chỉ cần tối ưu hóa logic)
            // Ví dụ: cập nhật kích thước cửa sổ

            // Thực hiện các cập nhật về kích thước hoặc vị trí chỉ khi cần thiết
            // ...

            // Dừng sự kiện để không chạy liên tục
            //CompositionTarget.Rendering -= OnRendering;
        }

        private void btnTaskbarLeft_Click(object sender, RoutedEventArgs e)
        {
            // Lấy độ rộng và chiều cao của màn hình hiện tại
            double screenWidth = this.Width;
            double screenHeight = this.Height;

            // Hiển thị độ rộng và chiều cao trong MessageBox
            MessageBox.Show($"Screen Width: {screenWidth}, Screen Height: {screenHeight}", "Screen Resolution");
        }

        private void btnMinimize_Click_1(object sender, RoutedEventArgs e)
        {
            ObservableCollection<CategoryExpenses> CollecData = new ObservableCollection<CategoryExpenses>();

            // Gọi hàm LoadCategoriesFromDatabase để tải dữ liệu
            DataProgram.LoadCategoriesFromDatabase(CollecData);

            // In danh sách CategoryExpenses ra (có thể là in ra ListBox, TextBox, Debug.WriteLine, v.v.)
            foreach (var category in CollecData)
            {

                Debug.WriteLine($"Category ID: {category.id}, Name: {category.Name}");

                foreach (var subCategory in category.CategoryExpensesProp)
                {
                    Debug.WriteLine($"    SubCategory ID: {subCategory.id}, Name: {subCategory.Name}, is_Details: {subCategory.is_Details}");

                    if (subCategory.is_Details == "1")
                    {
                        foreach (var detail in subCategory.SubSubCategoriesProp)
                        {
                            Debug.WriteLine($"        Detail ID: {detail.id}, Name: {detail.Name}");
                        }
                    }
                }
            }
        }

        //private void OptionMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    OptionMenu.Items.Clear();
        //    ObservableCollection<CategoryExpenses> CollecData = new ObservableCollection<CategoryExpenses>();

        //    // Gọi hàm LoadCategoriesFromDatabase để tải dữ liệu
        //    DataProgram.LoadCategoriesFromDatabase(CollecData);

        //    // In danh sách CategoryExpenses ra (có thể là in ra ListBox, TextBox, Debug.WriteLine, v.v.)
        //    foreach (var category in CollecData)
        //    {
        //        // Tạo một MenuItem mới
        //        var menuItem = new MenuItem
        //        {
        //            Header = $"ID: {category.Name}" // Hiển thị ID của Category
        //        };

        //        // Thêm sự kiện Click cho từng MenuItem
        //        menuItem.Click += (s, ev) =>
        //        {
        //            MessageBox.Show($"You selected Category ID: {category.Name}");
        //        };

        //        // Thêm MenuItem vào OptionMenu
        //        OptionMenu.Items.Add(menuItem);
        //    }
        //}



        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Đóng ứng dụng
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            if (txtFilePath.Text == "")
            {
                MessageBox.Show("Chưa load đường dẫn file", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Services.gI().sendFile(txtFilePath.Text))
            {
                MessageBox.Show("Update File thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Chỉ hổ trợ file <16MB", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyWordSearch = search.Text;
            Debug.WriteLine("keyWordSearch : " + search.Text);
            if (!string.IsNullOrWhiteSpace(keyWordSearch))
            {
                Services.gI().sendWordSearch(keyWordSearch);
            }
        }
        private void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        {
            string keyWordSearch = string.Empty;
            if (fileView.SelectedItem is FileItem selected)
            {
                keyWordSearch = selected.FileName;
            }
            Services.gI().downLoadFIle(keyWordSearch);
        }
        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                if(selectedFiles.Length == 1)
                {
                    txtFilePath.Text = selectedFiles[0];
                    txtFilePath.Foreground = MediaBrushes.Black;
                }
                else
                {
                    listFileSelect.Clear();
                    bool flag = true;
                    foreach (string path in selectedFiles)
                    {
                        listFileSelect.Add(path);
                        if (!Services.gI().sendFile(path)) {
                            flag = false;
                                }
                    }
                    if (flag)
                    {
                        MessageBox.Show("Update File thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Chỉ hổ trợ file <16MB", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

      
        ImageSource GetIconByExtension(string extension)
        {
            string fakePath = Path.Combine(Path.GetTempPath(), "dummy" + extension);
            if (!File.Exists(fakePath))
                File.WriteAllText(fakePath, "");

            try
            {
                var shellFile = ShellFile.FromFilePath(fakePath);
                return shellFile.Thumbnail.LargeBitmapSource; // Hoặc ExtraLargeBitmapSource
            }
            catch
            {
                return null; // fallback nếu Windows không có thumbnail
            }
        }


        public void LoadFiles(List<string> fileNames)
        {
            var list = new List<FileItem>();

            foreach (var name in fileNames)
            {
                var ext = Path.GetExtension(name);
                list.Add(new FileItem
                {
                    FileName = name,
                    Icon = GetIconByExtension(ext)
                });
            }

            fileView.ItemsSource = list;
        }
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void fileView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
        }
        //#endregion

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    // Lấy kích thước cửa sổ hiện tại
        //    double width = this.Width;
        //    double height = this.Height;
        //    // height =840
        //    var viewModel = this.DataContext as DashboardViewModel;


        //    // Hiển thị kích thước trong MessageBox
        //    MessageBox.Show($"Width: {viewModel.StatusMenu}, Height: {viewModel.checkHideMenu}", "Cửa sổ kích thước");
        //}
    }
}
