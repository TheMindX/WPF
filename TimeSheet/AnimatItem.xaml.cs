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

namespace TimeSheet
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationtItem : UserControl
    {
        public AnimationtItem()
        {
            InitializeComponent();
        }

        public string name
        {
            set
            {
                m_text.Text = value;
            }
        }

        public string icon
        {
            set
            {
                m_icon.Text = value;
            }
        }
    }
}
