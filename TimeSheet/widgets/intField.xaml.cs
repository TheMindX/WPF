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
    public partial class intField : UserControl
    {
        public intField()
        {
            InitializeComponent();
        }

        internal static bool sIsTextAllowed(string text)
        {
            if (text.Length > 6) return false;
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        // Use the DataObject.Pasting Handler 
        internal static void sTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!sIsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void textInput(object sender, TextCompositionEventArgs e)
        {
            if (intField.sIsTextAllowed(m_value.Text))
                e.Handled = false;
            else
                e.Handled = true;
        }

        public int Text
        {
            get
            {
                int val = 0;
                int.TryParse(m_value.Text, out val);
                return val;
            }
            set
            {
                m_value.Text = value.ToString();
            }
        }
    }
}
