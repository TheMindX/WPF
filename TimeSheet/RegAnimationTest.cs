using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    class TestRegAnimation
    {
        public class AnimationTest1Prop : AnimationProperty
        {
            string path = "Audio/test.ac";
            int vol = 80;
            public override int getPropertyCount() { return 2; }
            public override Type getPropertyType(int fidx)
            {
                if (fidx == 0)
                {
                    return typeof(string);
                }
                else if (fidx == 1)
                {
                    return typeof(int);
                }
                return null;
            }

            public override string getPropertyName(int fidx)
            {
                if (fidx == 0)
                {
                    return "path";
                }
                else if (fidx == 1)
                {
                    return "vol";
                }
                return null;
            }

            public override string getPropertyString(int fidx)
            {
                if (fidx == 0)
                {
                    return path;
                }
                else if (fidx == 1)
                {
                    throw new Exception("type error");
                }
                throw new Exception("type error");
            }
            public override void setPropertyString(int fidx, string str)
            {
                if (fidx == 0)
                {
                    path = str;
                }
                else if (fidx == 1)
                {
                    throw new Exception("type error");
                }
            }
            public override int getPropertyInt(int fidx)
            {
                if (fidx == 0)
                {
                    throw new Exception("type error");
                }
                else if (fidx == 1)
                {
                    return vol;
                }
                throw new Exception("type error");
            }
            public override void setPropertyInt(int fidx, int v)
            {
                if (fidx == 0)
                {
                    throw new Exception("type error");
                }
                else if (fidx == 1)
                {
                    vol = v;
                }
                throw new Exception("type error");
            }
            public override void getPropertyIntSlide(int fidx, out int min, out int max)
            {
                if (fidx == 0)
                {
                    throw new Exception("type error");
                }
                else if (fidx == 1)
                {
                    min = 0;
                    max = 100;
                    return;
                }
                throw new Exception("type error");
            }

            public override void attackObject(Object obj)
            {
                return;
            }
            public override Object getObject()//转换接口
            {
                return this;
            }
        }

        public class AnimationTest1 : AnimationObject
        {
            public override string getName() { return "声音"; }
            public override string getIcon() { return "声"; }
            List<AnimationTest1Prop> mProps = new List<AnimationTest1Prop>();
            public override AnimationProperty addProperty()
            {
                var prop = new AnimationTest1Prop();
                prop.m_animation_object = this;
                mProps.Add(prop);
                return prop;
            }
            public override void removeProperty(AnimationProperty p)
            {
                mProps.Remove(p as AnimationTest1Prop);
            }

            public override IEnumerable<AnimationProperty> getAllPropertys() { return mProps; }
            public override Object toObject()
            {
                return this;
            }

            public override AnimationObject clone()
            {
                var ret = new AnimationTest1();
                ret.mProps = new List<AnimationTest1Prop>(mProps);
                return ret;
            }
        }

        static public void tesstOfRegAnimObject()
        {
            var win = App.Current.MainWindow as MainWindow;
            win.m_animator.RegAnimationObjectType(new AnimationTest1());
        }
    }
}
