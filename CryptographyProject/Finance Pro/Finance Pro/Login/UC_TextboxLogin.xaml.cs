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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Finance_Pro.Login
{
    /// <summary>
    /// Interaction logic for UC_TexboxLogin.xaml
    /// </summary>
    public partial class UC_TextboxLogin : UserControl
    {
        public UC_TextboxLogin()
        {
            InitializeComponent();
        }



        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(UC_TextboxLogin));


        public bool isPassword
        {
            get { return (bool)GetValue(isPasswordProperty); }
            set { SetValue(isPasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isPasswordProperty =
            DependencyProperty.Register("isPassword", typeof(bool), typeof(UC_TextboxLogin));


    }
}
