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

namespace CommControlLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RangeSelectionControl : UserControl
    {
        public RangeSelectionControl()
        {
            InitializeComponent();
        }

        #region 定义路由事件

        //public event Action<double> LeftValueChange;
        //public event Action<double> RightValueChange;


        //声明和注册路由事件
        public static readonly RoutedEvent LeftValueChangeEvent =
            EventManager.RegisterRoutedEvent("LeftValueChange", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(RangeSelectionControl));
        //CLR事件包装
        public event RoutedEventHandler LeftValueChange
        {
            add { this.AddHandler(LeftValueChangeEvent, value); }
            remove { this.RemoveHandler(LeftValueChangeEvent, value); }
        }


        //声明和注册路由事件
        public static readonly RoutedEvent RightValueChangeEvent =
            EventManager.RegisterRoutedEvent("RightValueChange", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(RangeSelectionControl));
        //CLR事件包装
        public event RoutedEventHandler RightValueChange
        {
            add { this.AddHandler(RightValueChangeEvent, value); }
            remove { this.RemoveHandler(RightValueChangeEvent, value); }
        }



        #endregion


        #region 定义依赖属性


        public string RangeTitle
        {
            get { return (string)GetValue(RangeTitleProperty); }
            set { SetValue(RangeTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Titel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeTitleProperty =
            DependencyProperty.Register("RangeTitle", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("无名"));





        public string LeftMax
        {
            get { return (string)GetValue(LeftMaxProperty); }
            set { SetValue(LeftMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftMaxProperty =
            DependencyProperty.Register("LeftMax", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("100"));




        public string LeftMin
        {
            get { return (string)GetValue(LeftMinProperty); }
            set { SetValue(LeftMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftMinProperty =
            DependencyProperty.Register("LeftMin", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("0"));




        public string LeftValue
        {
            get { return (string)GetValue(LeftValueProperty); }
            set { SetValue(LeftValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftValueProperty =
            DependencyProperty.Register("LeftValue", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("0"));




        public string RightValue
        {
            get { return (string)GetValue(RightValueProperty); }
            set { SetValue(RightValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightValueProperty =
            DependencyProperty.Register("RightValue", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("0"));




        public string RightMax
        {
            get { return (string)GetValue(RightMaxProperty); }
            set { SetValue(RightMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightMaxProperty =
            DependencyProperty.Register("RightMax", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("100"));




        public string RightMin
        {
            get { return (string)GetValue(RightMinProperty); }
            set { SetValue(RightMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightMinProperty =
            DependencyProperty.Register("RightMin", typeof(string), typeof(RangeSelectionControl), new PropertyMetadata("0"));




        public bool IntBool
        {
            get { return (bool)GetValue(IntBoolProperty); }
            set { SetValue(IntBoolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntBool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntBoolProperty =
            DependencyProperty.Register("IntBool", typeof(bool), typeof(RangeSelectionControl), new PropertyMetadata(true));






        /// <summary>
        /// 决定两个进度条是否同步
        /// </summary>
        public bool Sync
        {
            get { return (bool)GetValue(SyncProperty); }
            set { SetValue(SyncProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sync.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SyncProperty =
            DependencyProperty.Register("Sync", typeof(bool), typeof(RangeSelectionControl), new PropertyMetadata(true));








        #endregion


        private void left_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double v;
            if (IntBool)
            {
                v = Math.Round(e.NewValue, 0);
            }
            else
            {
                v = Math.Round(e.NewValue, 3);

            }

            LeftValue = v.ToString();

            if (Sync)
            {
                if (v >= double.Parse(RightValue))
                {
                    RightValue = LeftValue;
                }
            }


            //事件触发
            RoutedEventArgs args = new RoutedEventArgs(LeftValueChangeEvent, LeftValue);
            this.RaiseEvent(args);
        }

        private void right_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double v;
            if (IntBool)
            {
                v = Math.Round(e.NewValue, 0);
            }
            else
            {
                v = Math.Round(e.NewValue, 3);

            }
            RightValue = v.ToString();

            if (Sync)
            {
                if (v <= double.Parse(LeftValue))
                {
                    LeftValue = RightValue;
                }
            }


            //事件触发
            RoutedEventArgs args = new RoutedEventArgs(RightValueChangeEvent, RightValue);
            this.RaiseEvent(args);
        }
    }
}