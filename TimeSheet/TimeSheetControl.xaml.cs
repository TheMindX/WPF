using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// TimeSheetControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSheetControl : UserControl
    {
        public TimeSheetControl()
        {
            InitializeComponent();
            mPickStaus = new pickStaus(this);
        }


        #region event
        public static event System.Action<TimeCurve, TimeCurve.TimeKey, TimeCurve, TimeCurve.TimeKey> evtOnSelectChanged;//选择曲线
        public static event System.Action<TimeCurve, TimeCurve.TimeKey> evtOnAddTimeKey;//添加key
        public static event System.Action<TimeCurve, TimeCurve.TimeKey> evtOnEditTimeKey;//编辑key
        public static event System.Action<TimeCurve, TimeCurve.TimeKey> evtOnRemoveTimeKey;//移除key
        public static event System.Action<int> evtOnMovieTime;//goto time位置
        #endregion


        #region config
        const double pickKeyPixelWidth = 5;
        const double pickBias = pickKeyPixelWidth;//5个象素偏离

        //const double timeCurveBeginX = 0;
        //const double timeCurveBeginY = 0;
        //const double timeCurveEndX = 0;
        //const double timeCurveEndY = 0;
        //const double timeLineItemHeight = 32;
        //const double minTimeUnitPixels = 15;//最大



        const double timeCurveBeginX = 64;
        const double timeCurveBeginY = 112;
        const double timeCurveEndX = 8;
        const double timeCurveEndY = 0;
        const double timeLineItemHeight = 32;
        const double minTimeUnitPixels = 15;//最大
        //time bar 缩放的scaletor
        int[] timeUnits = 
        {
            20,//20 ms
            50, 
            100,
            500,
            1000,
            5000,
        };
        double time2Pixel = 0.08f;//time*viewScale = pixel //时间轴与象素的缩放比
        #endregion



        //捡选状态
        class pickStaus
        {
            public pickStaus(TimeSheetControl p)
            {
                paresent = p;
            }

            TimeSheetControl paresent = null;

            public enum EStat
            {
                noPick,
                selecting,
                draging,
            }
            private EStat _mStat = EStat.noPick;//当前的捡选状态
            public EStat mStat
            {
                get
                {
                    return _mStat;
                }
                set
                {
                    if (mStat != EStat.noPick && value == EStat.noPick)
                    {
                        int i = 0;
                    }
                    _mStat = value;

                }
            }

            public enum EAction
            {
                mouseDownPicked,//点击选中
                mouseDownNoPicked,//点击但没选中
                mouseUp,//鼠标弹起
            }

            #region transition table
            class Transitions
            {
                public EStat mStIn = EStat.noPick;
                public EAction mAct = EAction.mouseDownNoPicked;
                public EStat mStOut = EStat.noPick;

                public Transitions(EStat inSt, EAction act, EStat outSt)
                {
                    mStIn = inSt;
                    mAct = act;
                    mStOut = outSt;
                }
            }

            List<Transitions> mTransitions = new List<Transitions>()
        {
            new Transitions(EStat.noPick, EAction.mouseDownPicked, EStat.draging),
            new Transitions(EStat.noPick, EAction.mouseDownNoPicked, EStat.selecting),
            new Transitions(EStat.noPick, EAction.mouseUp, EStat.noPick),

            new Transitions(EStat.selecting, EAction.mouseDownPicked, EStat.draging),
            new Transitions(EStat.selecting, EAction.mouseDownNoPicked, EStat.selecting),
            new Transitions(EStat.selecting, EAction.mouseUp, EStat.noPick),

            new Transitions(EStat.draging, EAction.mouseDownPicked, EStat.draging),
            new Transitions(EStat.draging, EAction.mouseDownNoPicked, EStat.selecting),
            new Transitions(EStat.draging, EAction.mouseUp, EStat.selecting),
        };

            #endregion

            public TimeSheetControl.TimeCurve.TimeKey currentKey()
            {
                if (mKeysPicked.Count == 0)
                {
                    return null;
                }
                return mKeysPicked[0];
            }

            public void addPickedKey(TimeSheetControl.TimeCurve.TimeKey tk)
            {
                //if (tk == null)
                //{
                //    log("addPickedKey:null");
                //}
                //else
                //{
                //    log("addPickedKey:", tk.time);
                //}

                var oldk = currentKey();
                if (tk != null)
                {
                    if (!mKeysPicked.Contains(tk))
                    {
                        mKeysPicked.Add(tk);
                    }
                }
                var newk = currentKey();
                if (newk != oldk)
                {
                    if (TimeSheetControl.evtOnSelectChanged != null)
                    {
                        TimeSheetControl.evtOnSelectChanged(
                            oldk == null ? null : oldk.mTimeCurve, oldk,
                            newk == null ? null : newk.mTimeCurve, newk);
                    }
                }
            }

            public void setPickedKey(TimeSheetControl.TimeCurve cv, TimeSheetControl.TimeCurve.TimeKey tk)
            {
                //if (tk == null)
                //{
                //    log("#########setPickedKey: null");
                //}
                //else
                //{
                //    log("#########setPickedKey:", tk.time);
                //}

                var oldk = currentKey();
                mKeysPicked.Clear();
                if (tk != null)
                {
                    if (!mKeysPicked.Contains(tk))
                    {
                        mKeysPicked.Add(tk);
                    }
                }
                var newk = currentKey();
                if (newk != oldk)
                {
                    if (TimeSheetControl.evtOnSelectChanged != null)
                    {
                        TimeSheetControl.evtOnSelectChanged(
                            oldk == null ? null : oldk.mTimeCurve, oldk,
                            newk == null ? null : newk.mTimeCurve, newk);
                    }
                }
            }

            public void reset()
            {
                mKeysPicked.Clear();
                mStat = EStat.noPick;
            }

            public EStat TransitState(EAction act)
            {
                //shift 下不响应
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    mStat = EStat.noPick;
                    return mStat;
                }

                foreach (var tran in mTransitions)
                {
                    if (tran.mStIn == mStat && act == tran.mAct)
                    {
                        mStat = tran.mStOut;
                        return mStat;
                    }
                }
                mStat = EStat.noPick;
                return EStat.noPick;
            }

            public IEnumerable<TimeSheetControl.TimeCurve.TimeKey> getAllPicked()
            {
                return mKeysPicked;
            }
            //选中的祯值
            List<TimeSheetControl.TimeCurve.TimeKey> mKeysPicked = new List<TimeSheetControl.TimeCurve.TimeKey>();

            Point mPickPos;
            public Point pickPos
            {
                get
                {
                    return mPickPos;
                }
                set
                {
                    mPickPos = value;
                    dragPos = value;
                }
            }

            public Point dragPos
            {
                get;
                set;
            }
        }

        public class TimeCurve //时间曲线
        {
            public string name;
            public Color color;

            List<TimeKey> timeKeys = new List<TimeKey>();

            public IEnumerable<TimeKey> getTimeKeys()
            {
                for (int i = 0; i < timeKeys.Count; ++i)
                {
                    yield return timeKeys[i];
                }
            }

            public void sortByTime()
            {
                timeKeys.Sort((t1, t2) =>
                {
                    if (t1.time > t2.time) return 1;
                    else if (t1.time == t2.time) return 0;
                    else return -1;
                });

                for (int i = 0; i < timeKeys.Count; ++i)
                {
                    timeKeys[i].idx = i;
                }
            }

            public List<TimeKey> selfAndAfterKeys(TimeKey key)
            {
                List<TimeKey> ret = new List<TimeKey>();
                bool find = false;
                for (int i = key.idx; i < timeKeys.Count; ++i)
                {
                    ret.Add(timeKeys[i]);
                }
                return ret;
            }

            public TimeKey addKey(double time, bool sync = true)
            {
                var tkey = new TimeKey(this, time);
                timeKeys.Add(tkey);
                sortByTime();
                //TimeSheetControl.timeKeySelected = tkey;
                if (sync && TimeSheetControl.evtOnAddTimeKey != null) TimeSheetControl.evtOnAddTimeKey(this, tkey);
                return tkey;
            }

            public void removeKey(TimeKey tkey, bool sync = true)
            {
                timeKeys.Remove(tkey);
                sortByTime();
                if (sync && TimeSheetControl.evtOnRemoveTimeKey != null) TimeSheetControl.evtOnRemoveTimeKey(this, tkey);
            }

            public class TimeKey
            {
                public TimeCurve mTimeCurve;
                double mTime;
                public int idx = 0;

                public double time
                {
                    get
                    {
                        return mTime;
                    }
                    set
                    {
                        mTime = value;
                        if (TimeSheetControl.evtOnEditTimeKey != null)
                            TimeSheetControl.evtOnEditTimeKey(mTimeCurve, this);
                    }
                }
                public TimeKey(TimeCurve tcurve, double t)
                {
                    mTimeCurve = tcurve;
                    mTime = t;
                }
            }
        }


        pickStaus mPickStaus = null;


        #region member
        double pixelStart = 0;//start象素位置
        public double timeStart//start时间位置
        {
            get
            {
                return (double)(pixelStart / time2Pixel);
            }
        }
        public int timePick
        {
            get
            {
                return (int)pos2time(mPickStaus.pickPos.X);
            }
            set
            {
                mPickStaus.pickPos = new Point(time2pos(value), mPickStaus.pickPos.Y);
                //TODO repaint
            }
        }
        #endregion

        #region helper
        int getTimeUnitIdx()
        {
            var t = minTimeUnitPixels / time2Pixel;
            int i = 0;
            for (; i < timeUnits.Length; ++i)
            {
                if (t < timeUnits[i]) return i;
            }
            return timeUnits.Length - 1;
        }

        double timeNow()
        {
            return System.DateTime.Now.ToUniversalTime().Subtract(
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;
        }


        //判断一个curve 是否和 rect相交
        bool isTimeLineInRect(TimeCurve tc, Point p1, Point p2)
        {
            double top = p1.Y < p2.Y ? p1.Y : p2.Y;
            double bottom = p1.Y < p2.Y ? p2.Y : p1.Y;
            for (int i = 0; i < mTimeCurves.Count; ++i)
            {
                if (mTimeCurves[i] == tc)//确定位置
                {
                    double minY = timeCurveBeginY + timeLineItemHeight * i;
                    double maxY = minY + timeLineItemHeight;

                    if (minY > bottom) return false;
                    if (maxY < top) return false;
                    return true;
                }
            }
            return false;
        }

        Point? testInArea(Point epos)
        {
            if (!isInTimeSheet(epos))
            {
                return null;
            }
            return epos;

        }

        bool isInTimeSheet(Point pos)
        {
            if (pos.X < timeCurveBeginX) return false;
            if (pos.X > m_canvas.Width - timeCurveEndX) return false;
            if (pos.Y < timeCurveBeginY) return false;
            if (pos.Y > m_canvas.Height - timeCurveEndY) return false;
            return true;
        }

        double time2pos(double time)//pixel
        {
            return (time - timeStart) * time2Pixel + timeCurveBeginX;
        }

        //
        double pos2time(double x)
        {
            var t = (int)((x - timeCurveBeginX) / time2Pixel);
            var ret = t + timeStart;
            return ret;
        }

        void SetLocation(UIElement ui, double px, double py)
        {
            Canvas.SetLeft(ui, px);
            Canvas.SetTop(ui, py);
        }

        void drawText(Point pt, string text, Color col)
        {
            var title = new TextBlock();
            title.IsEnabled = false;
            this.m_canvas.Children.Add(title);
            title.Text = text;
            title.Foreground = new SolidColorBrush(col);
            SetLocation(title, pt.X, pt.Y);
        }

        void drawRect(Point pt, Point pt2, Color col, bool fillOrStroke = true)
        {
            var rect = new Rectangle();
            rect.IsEnabled = false;
            this.m_canvas.Children.Add(rect);
            SetLocation(rect, Math.Min(pt.X, pt2.X), Math.Min(pt.Y, pt2.Y));

            rect.Width = Math.Abs(pt2.X - pt.X);
            rect.Height = Math.Abs(pt2.Y - pt.Y);
            if (fillOrStroke)
                rect.Fill = new SolidColorBrush(col);
            else
                rect.Stroke = new SolidColorBrush(col);
        }

        void drawLine(Point pt1, Point pt2, Color col)
        {
            var l = new Line();
            l.IsEnabled = false;
            this.m_canvas.Children.Add(l);
            l.X1 = pt1.X;
            l.Y1 = pt1.Y;
            l.X2 = pt2.X;
            l.Y2 = pt2.Y;
            l.Stroke = new SolidColorBrush(col);
        }
        #endregion

        #region method
        private TimeCurve mCurrentTimeCurve = null;
        public TimeCurve timeCurveSelected
        {
            get
            {
                if (timeKeySelected != null)
                {
                    mCurrentTimeCurve = timeKeySelected.mTimeCurve;
                }
                return mCurrentTimeCurve;
            }
            set
            {
                if (value != mCurrentTimeCurve)
                {
                    if (evtOnSelectChanged != null)
                    {
                        evtOnSelectChanged(mCurrentTimeCurve, timeKeySelected, value, timeKeySelected);
                    }
                    mCurrentTimeCurve = value;
                }

            }
        }


        List<TimeCurve> mTimeCurves = new List<TimeCurve>();//key frame source
        public TimeCurve.TimeKey timeKeySelected //selected 属性变化发生通知， 
        {
            get
            {
                return mPickStaus.currentKey();
            }
            set
            {
                mPickStaus.setPickedKey(timeKeySelected == null ? null : timeKeySelected.mTimeCurve, value);
            }
        }


        public TimeCurve addTimeLine(string name, Color? c = null)
        {
            var tcurve = new TimeCurve();
            tcurve.name = name;
            if (c.HasValue)
            {
                tcurve.color = c.Value;
            }
            else
            {
                tcurve.color = Color.FromArgb(75, 255, 255, 255);
            }
            mTimeCurves.Add(tcurve);
            return tcurve;
        }

        public void reset()
        {
            mPickStaus.reset();
            mTimeCurves.Clear();
            mCurrentTimeCurve = null;
        }

        //这里得到点选数据, 设置mCurrentPickPos
        bool pickTimeRecord(Point pos, out TimeCurve tcurve, out TimeCurve.TimeKey tkey)
        {
            bool res = false;
            if (!isInTimeSheet(pos))
            {
                tcurve = null;
                tkey = null;
                goto ret;
            }
            mPickStaus.pickPos = pos;

            int tlIdx = (int)((pos.Y - timeCurveBeginY) / timeLineItemHeight);
            if (tlIdx < 0 || tlIdx >= mTimeCurves.Count)
            {
                tcurve = null;
                tkey = null;
                goto ret;
            }
            tcurve = mTimeCurves[tlIdx];

            double t = pos2time(pos.X);
            var keys = tcurve.getTimeKeys();
            foreach (var ki in keys)
            {
                if (Math.Abs(ki.time - t) < 10 / time2Pixel)
                {
                    tkey = ki;
                    res = true;
                    goto ret;
                }
            }
            tkey = null;
        ret:

            return res;
        }

        public void setKey(Point apos)
        {
            TimeCurve tcurve = null;
            TimeCurve.TimeKey tkey = null;
            pickTimeRecord(apos, out tcurve, out tkey);
            if (tcurve == null)
            {
                return;
            }
            var time = pos2time(apos.X);
            timeKeySelected = tcurve.addKey(time);
            draw();
        }
        #endregion

        #region draw
        public void draw()
        {
            m_canvas.Children.Clear();
            drawTimeTicks();
            drawTimeLines();
            drawSelection();
        }

        //框架刻度线绘制
        void drawTimeTicks()
        {
            int unitIdx = getTimeUnitIdx();
            int unitTime = timeUnits[unitIdx];

            double unitPixel = (time2Pixel * unitTime);

            int timePos = (int)(pixelStart / time2Pixel);
            int startTime = (timePos / unitTime) * unitTime;
            if (startTime < timePos) startTime = startTime + unitTime;
            var detPixel = (int)((startTime - timePos) * time2Pixel + timeCurveBeginX);

            int currentTime = startTime;
            double currentPixel = detPixel;
            //EditorUtils.drawRect(new Point(timeLineBeginX, timeLineBeginY), new Point(this.Width - timeLineEndX, this.Height), new Rect(), Color.FromArgb(0.2f, 0.2f, 0.2f, 1), false);

            //绘制边框...
            drawRect(new Point(timeCurveBeginX, timeCurveBeginY - 20), new Point(this.Width - timeCurveEndX, timeCurveBeginY), Color.FromArgb(255, (byte)(0.1 * 255), (byte)(0.1 * 255), (byte)(0.1 * 255)));
            drawRect(new Point(timeCurveBeginX, timeCurveBeginY - 40), new Point(this.Width - timeCurveEndX, timeCurveBeginY), Color.FromArgb(255, (byte)(0.4 * 255), (byte)(0.4 * 255), (byte)(0.4 * 255)), false);
            drawRect(new Point(timeCurveBeginX, timeCurveBeginY - 20), new Point(this.Width - timeCurveEndX, this.Height), Color.FromArgb(255, (byte)(0.4 * 255), (byte)(0.4 * 255), (byte)(0.4 * 255)), false);

            //绘制刻度线....
            while (currentPixel < this.Width - timeCurveEndX)
            {
                if (currentTime >= 0)
                {
                    if ((int)currentTime % (int)(10 * unitTime) == 0)
                    {
                        drawText(new Point(currentPixel, timeCurveBeginY - 40)
                            , (currentTime / 1000.0).ToString() + "s", Colors.White);
                        drawLine(new Point(currentPixel, timeCurveBeginY - 40),
                            new Point(currentPixel, this.Height), Color.FromArgb((byte)(0.8*255), 255, 255, 255));
                    }
                    else if ((int)currentTime % (int)(5 * unitTime) == 0)
                    {
                        drawText(new Point(currentPixel, timeCurveBeginY - 35),
                            (currentTime / 1000.0).ToString() + "s", Colors.White);
                        drawLine(new Point(currentPixel, timeCurveBeginY - 35),
                            new Point(currentPixel, this.Height), Color.FromArgb((byte)(0.6 * 255), 255, 255, 255));
                    }
                    else
                    {
                        drawLine(new Point(currentPixel, timeCurveBeginY - 25),
                            new Point(currentPixel, this.Height), Color.FromArgb((byte)(0.2 * 255), 255, 255, 255));
                    }
                }
                currentTime += unitTime;
                currentPixel += unitPixel;
            }
            //绘制
            //如果是在拖矩形状态， //
            if (isInTimeSheet(new Point(mPickStaus.dragPos.X, timeCurveBeginY + 1)))
                drawLine(new Point(mPickStaus.dragPos.X, timeCurveBeginY - 35), new Point(mPickStaus.dragPos.X, this.Height), Colors.Red);
        }

        void drawTimeLines()
        {
            double currentPosX = timeCurveBeginX;
            double currentPosY = timeCurveBeginY;

            for (int i = 0; i < mTimeCurves.Count; ++i)
            {
                var line = mTimeCurves[i];
                if (line.name == "") continue;
                if (line == timeCurveSelected)
                {
                    drawText(new Point(0, currentPosY), line.name, Colors.Yellow);
                    drawRect(new Point(currentPosX, currentPosY),
                        new Point(this.Width - timeCurveEndX, currentPosY + timeLineItemHeight - pickKeyPixelWidth), Color.FromArgb(127, 255, 255, 255), true);
                }
                else
                {
                    drawText(new Point(0, currentPosY), line.name, Colors.White);
                    drawRect(new Point(currentPosX, currentPosY),
                        new Point(this.Width - timeCurveEndX, currentPosY + timeLineItemHeight - pickKeyPixelWidth), line.color, true);
                }

                var keys = line.getTimeKeys();
                foreach (var t in keys)
                {
                    if (isInTimeSheet(new Point(time2pos(t.time), currentPosY)))
                    {
                        if (mPickStaus.getAllPicked().ToArray().Contains(t))
                        {
                            drawRect(new Point(time2pos(t.time) - 3, currentPosY),
                                new Point(time2pos(t.time) + 3, currentPosY + timeLineItemHeight - 5),
                                Colors.Red);
                        }
                        else
                        {
                            drawRect(new Point(time2pos(t.time) - 3, currentPosY),
                            new Point(time2pos(t.time) + 3, currentPosY + timeLineItemHeight - 5),
                            Colors.White);
                        }
                    }
                }
                currentPosY += timeLineItemHeight;
            }
        }

        //框选绘制,
        void drawSelection()
        {
            if (mPickStaus.mStat == pickStaus.EStat.selecting)
            {
                drawRect(mPickStaus.pickPos, mPickStaus.dragPos,
                        Color.FromArgb((byte)(0.2 * 255), (byte)(0.5 * 255), 0, (byte)(0.3 * 255)), true);
            }

        }
        #endregion

        #region input
        Point initMousePos = new Point();

        private void onLeftMouseDown(double x, double y)
        {
            Point? apos = testInArea(new Point(x, y));
            if (apos == null)
            {
                return;
            }

            TimeCurve tcurve = null;
            TimeCurve.TimeKey tkey = null;

            bool picked = pickTimeRecord(apos.Value, out tcurve, out tkey);
            if (picked)
            {
                if (mPickStaus.getAllPicked().ToArray().Contains(tkey))
                {
                    mPickStaus.TransitState(pickStaus.EAction.mouseDownPicked);
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))//control 捡选该key之后的所有key
                    {
                        var keys = tkey.mTimeCurve.selfAndAfterKeys(tkey);
                        foreach (var key in keys)
                        {
                            mPickStaus.addPickedKey(key);
                        }
                    }
                    else
                    {
                        timeKeySelected = tkey;
                    }
                    mPickStaus.TransitState(pickStaus.EAction.mouseDownPicked);
                }
            }
            else
            {
                if (tcurve != null)
                {
                    if (tcurve != timeCurveSelected)
                    {
                        timeKeySelected = null;
                        timeCurveSelected = tcurve;
                    }
                }
                mPickStaus.TransitState(pickStaus.EAction.mouseDownNoPicked);
            }

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                evtOnMovieTime((int)pos2time(mPickStaus.dragPos.X));
            }

            //EditorWindow.focusedWindow.Repaint();//TODO reapaint
            draw();
        }



        public void onLeftMouseUp(double x, double y)
        {
            for (int i = 0; i < mTimeCurves.Count; ++i)
            {
                mTimeCurves[i].sortByTime();
            }

            //算出所有选择的key
            if (mPickStaus.mStat == pickStaus.EStat.selecting)
            {
                double left = 0;
                double right = 0;
                if (mPickStaus.pickPos.X < mPickStaus.dragPos.X)
                {
                    left = mPickStaus.pickPos.X - pickKeyPixelWidth;
                    right = mPickStaus.dragPos.X + pickKeyPixelWidth;
                }
                else
                {
                    right = mPickStaus.pickPos.X - pickKeyPixelWidth;
                    left = mPickStaus.dragPos.X + pickKeyPixelWidth;
                }
                if (mPickStaus.pickPos != mPickStaus.dragPos)
                {
                    timeKeySelected = null;

                    foreach (var tc in mTimeCurves)
                    {
                        if (isTimeLineInRect(tc, mPickStaus.pickPos, mPickStaus.dragPos))
                        {
                            foreach (TimeCurve.TimeKey tk in tc.getTimeKeys())
                            {
                                double pos = time2pos(tk.time);
                                if (left < pos && right > pos)//包含
                                {
                                    mPickStaus.addPickedKey(tk);
                                    //Debug.Log("" + pos + " is add to picklist");
                                }
                            }
                        }
                    }
                    //EditorWindow.focusedWindow.Repaint();//TODO repaint
                    draw();
                }
            }
            mPickStaus.TransitState(pickStaus.EAction.mouseUp);
        }

        double timeCount = 0;
        double timeLast = 0;
        double triggerTime = 200;
        public void onDrag(int whickKey, double dx, double dy)
        {
            if (mPickStaus.mStat == pickStaus.EStat.selecting)
            {
                mPickStaus.dragPos = testInArea(initMousePos) ?? mPickStaus.dragPos;
                draw();
            }

            Point? apos = testInArea(initMousePos) ?? new Point(0, 0);
            if (apos == null)
            {
                return;
            }
            mPickStaus.dragPos = apos.Value;
            timeCount += timeNow() - timeLast;
            //Debug.Log("timeCount: " + timeCount);
            if (timeCount > triggerTime)
            {
                timeCount = 0;
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (evtOnMovieTime != null)
                    {
                        evtOnMovieTime((int)pos2time(mPickStaus.dragPos.X));
                    }
                }
            }

            timeLast = timeNow();

            if (whickKey == 0)//left key
            {
                if (timeKeySelected != null && mPickStaus.mStat == pickStaus.EStat.draging)
                {
                    foreach (var key in mPickStaus.getAllPicked())
                    {
                        key.time += (dx / time2Pixel);
                    }
                }
            }
            else if (whickKey == 2) //middle key
            {
                pixelStart -= dx;
                //if (pixelStart < 0) pixelStart = 0;
            }
            else
            {
                if (mPickStaus.mStat != pickStaus.EStat.selecting)
                {
                    mPickStaus.pickPos = apos.Value;
                }
            }
            draw();
        }


        private void m_canvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        public void onDoubleClick(double x, double y)
        {
            Point? apos = testInArea(new Point(x, y));
            if (apos == null)
            {
                return;
            }
            setKey(apos.Value);
        }

        public void onScroll(double dx, double dy, double x, double y)
        {
            var apos = testInArea(new Point(x, y));
            if (apos == null) return;

            var t = pos2time(apos.Value.X);
            if (dy < 0)
            {
                time2Pixel = time2Pixel * 1.1f;
            }
            else if (dy > 0)
            {
                time2Pixel = time2Pixel / 1.1f;
            }
            //trans
            var px = time2pos(t);
            pixelStart -= apos.Value.X - px;

            draw();
        }
        #endregion

        private void m_canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition(m_canvas);
            onScroll(0, e.Delta, pos.X, pos.Y);
        }

        private void m_canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(m_canvas);

            onLeftMouseUp(pt.X, pt.Y);
            draw();
        }

        //private void m_canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{

        //}

        //private void m_canvas_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //}

        private void m_canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine(e.OriginalSource);
            initMousePos = e.GetPosition(m_canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    onLeftMouseDown(initMousePos.X, initMousePos.Y);
                }
                else if (e.ClickCount == 2)
                {
                    onDoubleClick(initMousePos.X, initMousePos.Y);
                }
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
            }
            e.Handled = false;
        }

        //private void m_canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    var pt = e.GetPosition(m_canvas);

        //    onLeftMouseUp(pt.X, pt.Y);
        //    draw();
        //}

        private void m_canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(m_canvas);
            var deltaPos = new Point(pos.X - initMousePos.X, pos.Y - initMousePos.Y);
            initMousePos = pos;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                onDrag(0, deltaPos.X, deltaPos.Y);
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                onDrag(2, deltaPos.X, deltaPos.Y);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                onDrag(1, deltaPos.X, deltaPos.Y);
            }
            e.Handled = false;
        }

        private void m_canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine(e.OriginalSource);
            var pt = e.GetPosition(m_canvas);

            onLeftMouseUp(pt.X, pt.Y);
            draw();
            e.Handled = false;
        }

    }
}
