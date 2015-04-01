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

namespace layout1
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class NoteItem : Grid
    {
        public NoteItem()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.textBox.Visibility = Visibility.Visible;
            this.textBlock.Visibility = Visibility.Hidden;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.textBox.Visibility = Visibility.Hidden;
                this.textBlock.Visibility = Visibility.Visible;
            }
        }


        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.textBox.Visibility = Visibility.Hidden;
            this.textBlock.Visibility = Visibility.Visible;
        }

        private void textBox_MouseLeave(object sender, MouseEventArgs e)
        {
            this.textBox.Visibility = Visibility.Hidden;
            this.textBlock.Visibility = Visibility.Visible;
        }



    }
}
