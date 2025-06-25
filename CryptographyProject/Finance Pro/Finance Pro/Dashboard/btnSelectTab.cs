using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Finance_Pro.Dashboard
{
    public class btnSelectTab : RadioButton
    {
        // Tạo một thuộc tính tùy chỉnh (ví dụ: MyCustomProperty)
        public static readonly DependencyProperty StatusMenuProperty =
       DependencyProperty.Register("StatusMenu", typeof(string), typeof(btnSelectTab), new PropertyMetadata("MaxMenu"));

        public string StatusMenu
        {
            get { return (string)GetValue(StatusMenuProperty); }
            set { SetValue(StatusMenuProperty, value); }
        }

         

        public string Icon { get; set; }
    

        public btnSelectTab()
        {

        }
    }
}
