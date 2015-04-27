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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TestRegAnimation.tesstOfRegAnimObject();
            m_externalAppControl.ExeName = "notepad++.exe";
            m_externalAppControl.openApp();
            this.Unloaded += new RoutedEventHandler((s, e1) => { m_externalAppControl.Dispose(); });

            List<Item> items = new List<Item>();
            items.Add(new Item(){Title="动画1"});
            items.Add(new Item() { Title = "动画2" });
            items.Add(new Item() { Title = "动画3" });
            items.Add(new Item() { Title = "动画4" });
            items.Add(new Item() { Title = "动画5" });
            m_items.ItemsSource = items;
        }


        public class Item
        {
            public string Title { get; set; }
            public int Completion { get; set; }
        }
    }
}
