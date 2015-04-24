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
    class TestTimePlaying
    {
        static public void play()
        {
            var win = (App.Current.MainWindow as MainWindow);
            win.m_animator.m_timesheet.timePlay();
        }
    }
}
