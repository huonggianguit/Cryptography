using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace Finance_Pro.Pages.Reality
{
    /// <summary>
    /// Interaction logic for btnMainGroup.xaml
    /// </summary>
    public partial class btnMainGroup : UserControl
    {
        public btnMainGroup()
        {
            InitializeComponent();
        }

       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           

          //  MessageBox.Show("Check giá trị IsDropDownOpen: " + ComboBoxX.IsDropDownOpen.ToString());

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
        //    MessageBox.Show("Check giá trị IsDropDownOpen: " + ComboBoxX.IsDropDownOpen.ToString());

        }

        private void Toggle_T_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CHECK ");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FUCK ");

        }

        private void ToggleButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FUCK ");
        }
    }
    public class ScrollLimitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double && values[1] is double)
            {
                return (double)values[0] == (double)values[1];
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
