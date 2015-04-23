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
        public virtual string getIcon() { return null; }
        public virtual AnimationProperty addProperty() { return null; }
        public virtual void removeProperty(AnimationProperty p) { }

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
        public AnimationObject m_animation_object = null;
        public virtual AnimationObject getAnimationObject()
        {
            return m_animation_object;
        }
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
        public virtual AnimationProperty copyFrom(AnimationProperty other)//属性的拷贝
        {
            return null;
        }

        public virtual void drawUI(Panel parent)
        {
            for(int i = 0; i<getPropertyCount(); ++i)
            {
                Type t = getPropertyType(i);
                string n = getPropertyName(i);
                if(t == typeof(int))
                {
                    int min;
                    int max;
                    getPropertyIntSlide(i, out min, out max);
                    var v = getPropertyInt(i);
                    if(min<max)
                    {
                        //draw slide
                        var s = new intSlider();
                        parent.Children.Add(s);
                    }
                    else
                    {
                        var intf = new intField();
                        parent.Children.Add(intf);
                    }
                }
                else if (t == typeof(string))
                {
                    var strf = new stringField();
                    parent.Children.Add(strf);
                }
            }
        }


        public TimeSheetControl.TimeCurve.TimeKey mKey;
    }


    public partial class AnimationItemsCtrl : UserControl
    {
        ContextMenu mContextMenu = null;
        public AnimationItemsCtrl()
        {
            InitializeComponent();
        }

        List<AnimationObject> mAnimationType = new List<AnimationObject>();

        //注册类型
        public void RegAnimationObjectType(AnimationObject proto)
        {
            reset();
            mAnimationType.Clear();
            mAnimationType.Add(proto);
            draw();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //弹出右键菜单
            mContextMenu = new ContextMenu();
            foreach(var proto in mAnimationType)
            {
                MenuItem mi = new MenuItem();
                mi.Header = proto.getName();

                mi.Click += new RoutedEventHandler( (obj, arg) =>
                    {
                        var item = proto.clone();
                        var tl = m_timesheet.addTimeLine(" ");
                        m_timesheet.timeCurveSelected = tl;
                        tl.attackObject = item;
                        draw();
                    });
                mContextMenu.Items.Add(mi);
            }

            mContextMenu.IsOpen = true;
        }

        TextBlock _ObjectTitle = null;
        TextBlock xm_object_name
        {
            get
            {
                if(_ObjectTitle == null)
                {
                    _ObjectTitle = UIHelper.FindChild<TextBlock>(this, "object_name");
                }
                return _ObjectTitle;
            }
        }

        StackPanel _ObjectPanel = null;
        StackPanel xm_objects
        {
            get
            {
                if (_ObjectPanel == null)
                {
                    _ObjectPanel = m_objects;
                }
                return _ObjectPanel;
            }
        }

        void reset()
        {
            m_timesheet.reset();
            m_objects.Children.Clear();
            m_object_name.Text = "no objects";

            TimeSheetControl.evtOnAddTimeKey += OnAddKey;
            TimeSheetControl.evtOnRemoveTimeKey += OnRemoveKey;
            TimeSheetControl.evtOnSelectChanged += OnSelectChange;
        }

        void OnAddKey(TimeSheetControl.TimeCurve tc, TimeSheetControl.TimeCurve.TimeKey tk)
        {

        }

        void OnRemoveKey(TimeSheetControl.TimeCurve tc, TimeSheetControl.TimeCurve.TimeKey tk)
        {

        }

        TimeSheetControl.TimeCurve oldCurve = null;
        TimeSheetControl.TimeCurve.TimeKey oldKey = null;
        AnimationProperty _editProperty = null;
        AnimationProperty editProperty
        {
            get
            {
                var tk = m_timesheet.timeKeySelected;
                if(tk != null)
                {
                    return tk.attachProperty as AnimationProperty;
                }
                
                var tc = m_timesheet.timeCurveSelected;
                if(tc != null)
                {
                    var ao = tc.attackObject as AnimationObject;
                    if(_editProperty == null || _editProperty.getAnimationObject() != ao)
                    {
                        _editProperty = ao.addProperty();
                    }
                }
                return _editProperty;
            }
            
        }

        void OnSelectChange(TimeSheetControl.TimeCurve oldc, TimeSheetControl.TimeCurve.TimeKey oldk, 
            TimeSheetControl.TimeCurve newc, TimeSheetControl.TimeCurve.TimeKey newk)
        {

        }

        void draw()
        {
            m_objects.Children.Clear();
            m_object_name.Text = "属性";
            m_propertys.Children.Clear();

            //draw property
            var choosenCurve = m_timesheet.timeCurveSelected;
            if(choosenCurve != null)
            {
                var item = choosenCurve.attackObject as AnimationObject;
                m_object_name.Text = item.getName()+"属性";
            }
            else
            {
                m_object_name.Text = "属性";
            }

            if(editProperty != null)
            {
                editProperty.drawUI(m_propertys);
            }

            //draw object list
            foreach(var curve in m_timesheet.timeCurves)
            {
                var uiItem = new AnimationtItem();
                var item = curve.attackObject as AnimationObject;
                uiItem.name = item.getName();
                uiItem.icon = item.getIcon();
                m_objects.Children.Add(uiItem);
            }

            m_timesheet.repaint();
        }
    }
}
