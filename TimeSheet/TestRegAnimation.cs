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
            public string path = "Audio/test.ac";
            public int vol = 1;
            static int svol = 1;

            public AnimationTest1Prop()
            {
                svol++;
                vol = svol;
            }
            public override void drawUI(Panel parent)
            {
                base.drawUI(parent);

                var p = new stringField();
                parent.Children.Add(p);
                p.m_label.Text = "路径";
                p.m_value.Text = path;
                p.m_value.TextChanged += (send, e) =>
                {
                    path = p.m_value.Text;
                };
                p.LostFocus += (send, e) =>
                {
                    changeNotify();
                };

                var s = new intSlider();
                parent.Children.Add(s);
                s.m_label.Text = "音量";
                s.m_value.Text = vol.ToString();
                s.m_value.TextChanged += (send, e) =>
                    {
                        int.TryParse(s.m_value.Text, out vol);
                    };
                s.LostFocus += (sender, e) =>
                    {
                        changeNotify();
                    };
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);
                s = new intSlider();
                parent.Children.Add(s);

            }

            public override bool compare(AnimationProperty other)
            {
                return this.path == (other as AnimationTest1Prop).path
                    && this.vol == (other as AnimationTest1Prop).vol;
            }

            public override void copyFrom(AnimationProperty other)
            {
                this.path = (other as AnimationTest1Prop).path;
                this.vol = (other as AnimationTest1Prop).vol;
            }

            public override AnimationProperty clone()
            {
                var ret = new AnimationTest1Prop();
                ret.copyFrom(this);
                return ret;
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
