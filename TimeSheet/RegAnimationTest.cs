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
    class TestRegAnimation
    {
        public class AnimationTest1Prop : AnimationProperty
        {
            string path = "Audio/test.ac";
            int vol = 80;


            public override void drawUI(Panel parent)
            {
                var p = new stringField();
                parent.Children.Add(p);
                var s = new intSlider();
                parent.Children.Add(s);
            }
        }

        public class AnimationTest1 : AnimationObject
        {
            public override string getName() { return "声音"; }
            public override string getIcon() { return "声"; }

            public override Object toObject()
            {
                return this;
            }

            public override AnimationProperty createProperty()
            {
                return new AnimationTest1Prop();
            }

            public override AnimationObject newInstance()
            {
                return new AnimationTest1();
            }
        }

        static public void tesstOfRegAnimObject()
        {
            var win = App.Current.MainWindow as MainWindow;
            win.m_animator.RegAnimationObjectType(new AnimationTest1());
        }
    }
}
