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
                if(_mUI == null)
                {
                    _mUI = new AnimationtItem();
                    (_mUI as AnimationtItem).m_choosen_button.Background = new SolidColorBrush(Colors.Black);
                    _mUI.name = getName();
                    _mUI.icon = getIcon();
                }
                return _mUI;
            }
        }

        public void resetUI()
        {
            _mUI = null;
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

    public abstract class AnimationProperty //automatic UI
    {
        public Panel mPanel = null;
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

        public abstract void copyFrom(AnimationProperty other);//属性的拷贝

        public abstract AnimationProperty clone();
        public virtual bool compare(AnimationProperty other)//属性的拷贝
        {
            return false;
        }

        public AnimationProperty undoListCurrent()
        {
            if(m_undoList.Count != 0)
            {
                return m_undoList.Last();
            }
            return null;
        }

        public bool isChange()
        {
            var undoAp = undoListCurrent();
            if (undoAp == null) return true;
            return !this.compare(undoAp);
        }
        
        const int maxRedo = 5;
        int undoIdx = -1;
        List<AnimationProperty> m_undoList = new List<AnimationProperty>();
        public void undo()
        {
            if (undoIdx <= 0) return;
            undoIdx--;
            this.copyFrom(m_undoList[undoIdx]);
        }

        public void redo()
        {
            if (undoIdx+1 == m_undoList.Count) return;
            undoIdx++;
            this.copyFrom(m_undoList[undoIdx]);
        }

        public bool record()
        {
            if (!isChange()) return false;
            AnimationProperty ap = this.clone();
            if(undoIdx + 1 != m_undoList.Count)
            {
                m_undoList.RemoveRange(undoIdx + 1, m_undoList.Count - (undoIdx + 1));
            }
            m_undoList.Add(ap);
            undoIdx++;
            return true;
        }

        public virtual void changeNotify()
        {
            if (record())
            {
                //drawUI(mPanel);
            }
        }

        Button m_record = null;
        Button m_redo = null;
        Button m_undo = null;
        public virtual void drawUI(Panel parent)
        {
            mPanel = parent;
            parent.Children.Clear();

            StackPanel sp = new StackPanel();
            parent.Children.Add(sp);
            sp.Orientation = Orientation.Horizontal;

            if(undoIdx == -1)
                record();
            //撤销
            Button m_undo = new Button();
            sp.Children.Add(m_undo);
            m_undo.Content = "撤销";
            m_undo.Width = 60;

            //if (undoIdx <= 0)
            //{
            //    m_undo.IsEnabled = false;
            //}
            //else
            {
                m_undo.Click += (s, arg) =>
                {
                    undo();
                    drawUI(mPanel);
                };
            }
                
            
            //重做
            m_redo = new Button();
            sp.Children.Add(m_redo);
            m_redo.Content = "重做";
            m_redo.Width = 60;

            //if (undoIdx+1 >= m_undoList.Count)
            //{
            //    m_undo.IsEnabled = false;
            //}
            //else
            {
                m_redo.Click += (s, arg) =>
                {
                    redo();
                    drawUI(mPanel);
                };
            }

            //记录
            m_record = new Button();
            sp.Children.Add(m_record);
            m_record.Content = "记录";
            m_record.Width = 60;
            //if(!isChange() )
            //{
            //    m_record.IsEnabled = false;
            //}
            //else
            {
                m_record.Click += (s, arg) =>
                {
                    record();
                    drawUI(parent);
                };
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
                        tl.attackObject = item;
                        m_timesheet.timeCurveSelected = tl;
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

        bool bKeyBind = false;
        void reset()
        {
            m_timesheet.reset();
            m_objects.Children.Clear();
            m_object_name.Text = "no objects";

            if(!bKeyBind)
            {
                var window = Window.GetWindow(this);
                window.PreviewKeyDown += window_KeyDown;
                bKeyBind = true;
            }
        }

        void OnAddKey(TimeSheetControl.TimeCurve tc, TimeSheetControl.TimeCurve.TimeKey tk)
        {
            //temly not use
        }

        void OnRemoveKey(TimeSheetControl.TimeCurve tc, TimeSheetControl.TimeCurve.TimeKey tk)
        {
            //temply not use
        }

        //TimeSheetControl.TimeCurve oldCurve = null;
        //TimeSheetControl.TimeCurve.TimeKey oldKey = null;
        AnimationProperty _editProperty = null;
        AnimationProperty editProperty
        {
            get
            {
                var tk = m_timesheet.timeKeySelected;
                var tc = m_timesheet.timeCurveSelected;
                if(tk != null)
                {
                    if (tk.attachProperty == null)
                    {
                        var ao = tc.attackObject as AnimationObject;
                        _editProperty = ao.addProperty(tk);
                    }
                    return tk.attachProperty as AnimationProperty;
                }

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

        void OnSelectChange(TimeSheetControl.TimeCurve oc, TimeSheetControl.TimeCurve.TimeKey ok, 
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
                }
                if (nc != null)
                {
                    var ao = (nc.attackObject as AnimationObject);
                    if (ao == null) return;
                    ao.mUI.m_choosen_button.Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255));
                }

                if(nk != ok)
                {
                    editProperty.drawUI(m_propertys);
                }
                
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
                editProperty.drawUI(m_propertys);//属性UI
            }

            //draw object list
            foreach(var curve in m_timesheet.timeCurves)
            {
                var tmpCurve = curve;//for closure use
                var ao = curve.attackObject as AnimationObject;
                m_objects.Children.Add(ao.mUI);
                ao.mUI.m_choosen_button.Click += delegate(object sender, RoutedEventArgs e)
                    {
                        var ao1 = m_timesheet.timeCurveSelected.attackObject as AnimationObject;
                        if(m_timesheet.timeCurveSelected != tmpCurve)
                        {
                            if (m_timesheet.timeKeySelected != null)
                            {
                                m_timesheet.timeKeySelected = null;
                            }
                            m_timesheet.timeCurveSelected = tmpCurve;
                        }
                        
                        //ao1 = m_timesheet.timeCurveSelected.attackObject as AnimationObject;
                        //ao1.mUI.m_choosen_button.Background = new SolidColorBrush(Colors.YellowGreen);
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
            //Console.WriteLine("onTime: " + t);
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
            var cv = m_timesheet.timeCurveSelected;
            if(cv != null)
            {
                var ao = (cv.attackObject as AnimationObject);
                ((ao.mUI.Parent) as Panel).Children.Remove(ao.mUI);

                m_timesheet.removeTimeLine(cv);
            }
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
            //这里只放绑定
            TimeSheetControl.evtOnAddTimeKey += OnAddKey;
            TimeSheetControl.evtOnRemoveTimeKey += OnRemoveKey;
            TimeSheetControl.evtOnSelectChanged += OnSelectChange;


            TimeSheetControl.evtOnPlaying += onPlaying;//时间轴播放
            TimeSheetControl.evtOnPickTime += t =>//鼠标点击事件
            {
                m_time_box.Text = t;
            };
        }

        private void window_KeyDown(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Z) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var ao = editProperty;
                if (ao != null)
                {
                    ao.undo();
                    ao.drawUI(ao.mPanel);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Y) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var ao = editProperty;
                if (ao != null)
                {
                    ao.redo();
                    ao.drawUI(ao.mPanel);
                }
            }
        }

    }
}
