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

        public TimeSheetControl.TimeCurve mCurve = null;
        private AnimationtItem _mUI = null;
        public AnimationtItem mUI
        {
            get
            {   
                return _mUI;
            }
        }

        public void resetUI()
        {
            _mUI = new AnimationtItem();
            (_mUI as AnimationtItem).Background = new SolidColorBrush(Colors.Black);
            _mUI.name = getName();
            _mUI.icon = getIcon();
        }

        public virtual AnimationProperty createProperty()
        {
            return null;
        }

        public virtual AnimationProperty addProperty(TimeSheetControl.TimeCurve.TimeKey key)
        {
            var prop = createProperty();
            prop.m_animation_object = this;
            if(key != null)
            {
                key.attachProperty = prop;
            }
            
            return prop;
        }

        public virtual Object toObject()
        {
            return this;
        }

        public virtual AnimationObject newInstance()
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
        public virtual void attackObject(Object obj) { }
        public virtual Object getObject()//转换接口
        {
            return this;
        }
        public virtual AnimationProperty copyFrom(AnimationProperty other)//属性的拷贝
        {
            return null;
        }

        public virtual void drawUI(Panel parent)
        {
            //for(int i = 0; i<getPropertyCount(); ++i)
            //{
            //    Type t = getPropertyType(i);
            //    string n = getPropertyName(i);
            //    if(t == typeof(int))
            //    {
            //        int min;
            //        int max;
            //        getPropertyIntSlide(i, out min, out max);
            //        var v = getPropertyInt(i);
            //        if(min<max)
            //        {
            //            //draw slide
            //            var s = new intSlider();
            //            parent.Children.Add(s);
            //        }
            //        else
            //        {
            //            var intf = new intField();
            //            parent.Children.Add(intf);
            //        }
            //    }
            //    else if (t == typeof(string))
            //    {
            //        var strf = new stringField();
            //        parent.Children.Add(strf);
            //    }
            //}
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

        private void BtnAddAnimation(object sender, RoutedEventArgs e)
        {
            //弹出右键菜单
            mContextMenu = new ContextMenu();
            foreach(var proto in mAnimationType)
            {
                MenuItem mi = new MenuItem();
                mi.Header = proto.getName();

                mi.Click += new RoutedEventHandler( (obj, arg) =>
                    {
                        var item = proto.newInstance();
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
                        _editProperty = ao.addProperty(tk);
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

            int idx = 0;
            //draw object list
            foreach(var curve in m_timesheet.timeCurves)
            {
                var tmpCurve = curve;//for closure use
                var ao = curve.attackObject as AnimationObject;
                ao.resetUI();
                m_objects.Children.Add(ao.mUI);
                ao.mUI.m_choosen_button.Click += delegate(object sender, RoutedEventArgs e)
                    {
                        var ao1 = m_timesheet.timeCurveSelected.attackObject as AnimationObject;
                        ao1.mUI.m_choosen_button.Background = new SolidColorBrush(Colors.Black);
                        m_timesheet.timeCurveSelected = tmpCurve;
                        ao1 = m_timesheet.timeCurveSelected.attackObject as AnimationObject;
                        ao1.mUI.m_choosen_button.Background = new SolidColorBrush(Colors.YellowGreen);
                        m_timesheet.repaint();
                    };
            }

            m_timesheet.repaint();
        }

        private void onPlaying(bool pause, double t)
        {
            if(pause)
            {
                if ((string)m_play.Content != "播")
                    m_play.Content = "播";
            }
            else
            {
                if ((string)m_play.Content != "暂")
                    m_play.Content = "暂";
            }
            Console.WriteLine("onTime: " + t);
        }

        private void BtnOnPlay(object sender, RoutedEventArgs e)
        {
            //TimeSheetControl.evtOnPlaying -= onPlaying;
            //TimeSheetControl.evtOnPlaying += onPlaying;
            if (m_timesheet.m_stopwatch.isPause())
                m_timesheet.timePlay();
            else
                m_timesheet.timePause();
        }

        private void BtnRemoveAnimation(object sender, RoutedEventArgs e)
        {

        }

        private void m_reset_Click(object sender, RoutedEventArgs e)
        {
            m_timesheet.timeReset();
        }

        private void m_nextkey_Click(object sender, RoutedEventArgs e)
        {
            m_timesheet.selectNextKey();
        }

        private void m_prekey_Click(object sender, RoutedEventArgs e)
        {
            m_timesheet.selectPreviewKey();
        }

        private void m_scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            m_timesheet.scrollY = e.VerticalOffset;
            m_timesheet.repaint();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSheetControl.evtOnPlaying += onPlaying;
            TimeSheetControl.evtOnPickTime += t =>
                {
                    m_time_box.Text = t.ToString();
                };

            TimeSheetControl.evtOnSelectChanged += delegate(TimeSheetControl.TimeCurve oc, TimeSheetControl.TimeCurve.TimeKey ok, 
                TimeSheetControl.TimeCurve nc, TimeSheetControl.TimeCurve.TimeKey nk)
            {
                if(nc != oc)
                {
                    if (oc != null)
                    {
                        var ao = (oc.attackObject as AnimationObject);
                        if (ao == null) return;
                        if (ao.mUI == null) return;
                        ao.mUI.m_choosen_button.Background = new SolidColorBrush(Colors.Black);
                    }
                    if(nc != null)
                    {
                        var ao = (nc.attackObject as AnimationObject);
                        if (ao == null) return;
                        if (ao.mUI == null) return;
                        ao.mUI.m_choosen_button.Background = new SolidColorBrush(Colors.YellowGreen);
                    }
                }
            };
        }

    }
}
