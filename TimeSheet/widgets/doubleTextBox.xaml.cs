using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


namespace TimeSheet
{
    /// <summary>
    /// intField.xaml 的交互逻辑
    /// </summary>
    public partial class doubleTextBox : UserControl
    {
        public doubleTextBox()
        {
            InitializeComponent();
        }

        private void textInput(object sender, TextCompositionEventArgs e)
        {
            if (doubleField.sIsTextAllowed(m_value.Text) && doubleField.sIsTextAllowed(e.Text))
                e.Handled = false;
            else
                e.Handled = true;
        }

        public double Text
        {
            get
            {
                int val = 0;
                int.TryParse(m_value.Text, out val); ;
                return val;
            }
            set
            {
                m_value.Text = value.ToString();
            }
        }
    }
}
