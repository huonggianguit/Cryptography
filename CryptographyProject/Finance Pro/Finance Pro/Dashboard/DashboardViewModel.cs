using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Finance_Pro.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public DashboardViewModel()
        {
            WidthMainGrid = 200;
            WidthLogo = 80;
            SizetxtLogo = 30;
            StatusMenu = "MaxMenu";
            lastSizeMenu = StatusMenu;
            _TabChoosed1 = true;

            ChangeSizeMenu = new RelayCommand<object>(ChangeMenu);

            ChooseTab = new RelayCommand<object>(SelectTab);
            Dashboard_Desktop_Layout = new DataTemplate();


            thumbResize = new RelayCommand<object>(ResizeMenu);

        }

        public DataTemplate Dashboard_Desktop_Layout { get; set; }

        bool _TabChoosed1;
        public bool TabChoosed1
        {
            get { return _TabChoosed1; }
            set
            {
                _TabChoosed1 = value;
                OnPropertyChanged("TabChoosed1");
            }
        }

        bool _TabChoosed2;
        public bool TabChoosed2
        {
            get { return _TabChoosed2; }
            set
            {
                _TabChoosed2 = value;
                OnPropertyChanged("TabChoosed2");
            }
        }

        bool _TabChoosed3;
        public bool TabChoosed3
        {
            get { return _TabChoosed3; }
            set
            {
                _TabChoosed3 = value;
                OnPropertyChanged("TabChoosed3");
            }
        }
        bool _TabChoosed4;
        public bool TabChoosed4
        {
            get { return _TabChoosed4; }
            set
            {
                _TabChoosed4 = value;
                OnPropertyChanged("TabChoosed4");
            }
        }

        bool _TabChoosed5;
        public bool TabChoosed5
        {
            get { return _TabChoosed5; }
            set
            {
                _TabChoosed5 = value;
                OnPropertyChanged("TabChoosed5");
            }
        }

        int _WidthMainGrid;
        public int WidthMainGrid
        {
            get { return _WidthMainGrid; }
            set
            {
                _WidthMainGrid = value;
                OnPropertyChanged("WidthMainGrid");
            }
        }
        string _StatusMenu;
        public string StatusMenu
        {
            get { return _StatusMenu; }
            set
            {
                _StatusMenu = value;
                OnPropertyChanged("StatusMenu");
            }
        }

        int _WidthLogo;
        public int WidthLogo
        {
            get { return _WidthLogo; }
            set
            {
                _WidthLogo = value;
                OnPropertyChanged("WidthLogo");
            }
        }
        int _SizetxtLogo;
        public int SizetxtLogo
        {
            get { return _SizetxtLogo; }
            set
            {
                _SizetxtLogo = value;
                OnPropertyChanged("SizetxtLogo");
            }
        }

        public ICommand ChangeSizeMenu { get; private set; }
        public ICommand ChooseTab { get; private set; }
        public ICommand ChangeStatusMenu { get; private set; }
        public ICommand thumbResize { get; private set; }
        public void ResizeMenu(object obj)
        {
            string ModeScreen = "LaptopMode";

            if (obj is double width)
            {

                if (width < 800)
                {
                    ModeScreen = "MobieMode";
                    checkHideMenuInMobie = true;
                }
                else if (width < 1000)
                {
                    ModeScreen = "LaptopMode";
                    checkHideMenuInMobie = false;
                }
                else if (width > 1000)
                {
                    ModeScreen = "DesktopMode";
                    checkHideMenuInMobie = false;
                }
                ChangeMenu(ModeScreen);
            }
        }
        public bool checkHideMenu;
        public bool checkHideMenuInMobie;
        string lastSizeMenu;
        public bool ResponsiveMenuSize;
        public void ChangeStatus(object obj)
        {
            // Debug.WriteLine("Check With " + (double)obj);

            string ModeScreen = "LaptopMode";

            if (obj is double width)
            {

                if (width < 800)
                {
                    ModeScreen = "MobieMode";
                    checkHideMenuInMobie = true;
                }
                else if (width < 1000)
                {
                    ModeScreen = "LaptopMode";
                    checkHideMenuInMobie = false;
                }
                else if (width > 1000)
                {
                    ModeScreen = "DesktopMode";
                    checkHideMenuInMobie = false;
                }
                ChangeMenu(ModeScreen);
            }
        }


        private void ChangeMenu(object obj)
        {
            if (obj is string cmd)
            {
                switch (cmd)
                {
                    case "MobieMode":
                        StatusMenu = "HideInMobie";
                        break;

                    case "LaptopMode":
                        StatusMenu = "MiniMenu";
                        lastSizeMenu = "MiniMenu";
                        break;

                    case "DesktopMode":
                        StatusMenu = "MaxMenu";
                        lastSizeMenu = "MaxMenu";
                        break;

                    case "btnMaxMenu":
                        StatusMenu = "MaxMenu";
                        lastSizeMenu = "MaxMenu";
                        checkHideMenu = false;
                        break;

                    case "btnMiniMenu":
                        StatusMenu = "MiniMenu";
                        lastSizeMenu = "MiniMenu";
                        checkHideMenu = false;
                        break;

                    case "btnHideMenu":
                        StatusMenu = "HideMenu";
                        checkHideMenu = true;
                        break;

                    case "Taskbar_Left":
                        ResponsiveMenuSize = false;
                        if (checkHideMenu == true || checkHideMenuInMobie)
                        {
                            StatusMenu = lastSizeMenu;

                        }
                        break;

                    case "Border_Navigate":
                        ResponsiveMenuSize = false;

                        if (checkHideMenu || checkHideMenuInMobie)
                        {
                            StatusMenu = "HideMenu";
                        }
                        break;

                    default:
                        StatusMenu = "MaxMenu";
                        break;
                }
            }
            switch (StatusMenu)
            {
                case "MaxMenu":
                    WidthMainGrid = 200;
                    WidthLogo = 80;
                    SizetxtLogo = 30;
                    break;

                case "MiniMenu":
                    WidthMainGrid = 80;
                    WidthLogo = 50;
                    SizetxtLogo = 20;
                    break;

                case "HideMenu":
                    WidthMainGrid = 0;
                    break;

                case "HideInMobie":
                    WidthMainGrid = 0;
                    break;
            }
        }
        public void SelectTab(object obj)
        {
            if (obj is ToggleButton button)
            {
                TabChoosed1 = false;
                TabChoosed2 = false;
                TabChoosed3 = false;
                TabChoosed4 = false;
                TabChoosed5 = false;

                switch (button.Content)
                {
                    case "Dashboard":
                        TabChoosed1 = true;
                        break;

                    case "Budget":
                        TabChoosed2 = true;
                        break;

                    case "Reality":
                        TabChoosed3 = true;
                        break;

                    case "Statistics":
                        TabChoosed4 = true;
                        break;

                    case "Utilities":
                        TabChoosed5 = true;
                        break;

                    default:
                        // Nếu cần, thêm xử lý mặc định ở đây
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
