using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    class TestTimeSheet
    {
        static public void testOfTimeSheet()
        {
            var win = App.Current.MainWindow as MainWindow;
            var ts = win.m_animator.m_timesheet;
            ts.reset();
            ts.addTimeLine("abc");
            ts.addTimeLine("cde");
            ts.addTimeLine("cdeasdfasfd");
            ts.addTimeLine("asdfasdfasdfasfd");
        }
    }
}
