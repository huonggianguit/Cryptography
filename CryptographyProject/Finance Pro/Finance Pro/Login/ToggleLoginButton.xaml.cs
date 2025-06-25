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
using Finance_Pro.Login;

namespace Finance_Pro.Login
{
    /// <summary>
    /// Interaction logic for ToggleLoginButton.xaml
    /// </summary>
    public partial class ToggleLoginButton : UserControl
    {
        public ToggleLoginButton()
        {
            InitializeComponent();
        }



        public bool isCheckRememBerMe
        {
            get { return (bool)GetValue(isCheckRememBerMeProperty); }
            set { SetValue(isCheckRememBerMeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isCheckRememBerMe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isCheckRememBerMeProperty =
            DependencyProperty.Register("isCheckRememBerMe", typeof(bool), typeof(ToggleLoginButton), new PropertyMetadata(false));

       
        public void btnCheckRememberMe_Click(object sender, RoutedEventArgs e)
        {
            isCheckRememBerMe = !isCheckRememBerMe;
            if (isCheckRememBerMe == true)
            {
                toggleImage.Source = (ImageSource)FindResource("ToggleBut2");
                toggleBackImage.ImageSource = (ImageSource)FindResource("BackBut2");
            }
            else
            {
                toggleImage.Source = (ImageSource)FindResource("ToggleBut1");
                toggleBackImage.ImageSource = (ImageSource)FindResource("BackBut1");
            }
        }
    }
}
