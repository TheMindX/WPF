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
    /// AnimationItems.xaml 的交互逻辑
    /// </summary>
    /// 

    public class AnimationObject
    {
        public virtual string getName() { return null; }
        public virtual AnimationProperty addProperty() { return null; }
        public virtual void removeProperty() { }

        public virtual IEnumerable<AnimationProperty> getAllPropertys() { return new List<AnimationProperty>(); }
        public virtual Object toObject()
        {
            return this;
        }

        public virtual AnimationObject clone()
        {
            return null;
        }
    }

    public class AnimationProperty //automatic UI
    {
        public virtual int getPropertyCount(){return 0;}
        public virtual Type getPropertyType(int fidx){return typeof(int);}
        public virtual string getPropertyName(int fidx){return "";}
        public virtual string getPropertyString(int fidx) { return ""; }
        public virtual void setPropertyString(int fidx, string str) { }
        public virtual int getPropertyInt(int fidx) { return 1; }
        public virtual void setPropertyInt(int fidx, int v) { }
        public virtual void getPropertyIntSlide(int fidx, out int min, out int max) { min = 0; max = 0; }
        public virtual void attackObject(Object obj) { }
        public virtual Object getObject()//转换接口
        {
            return null;
        }

        public TimeSheetControl.TimeCurve.TimeKey mKey;
    }


    public partial class AnimationItemsCtrl : UserControl
    {
        public AnimationItemsCtrl()
        {
            InitializeComponent();
        }

        Dictionary<string, AnimationObject> mAnimationType = new Dictionary<string, AnimationObject>();
        //注册类型
        public void RegAnimationObjectType(string name, AnimationObject proto)
        {
            mAnimationType.Add(name, proto);
        }
    }
}
