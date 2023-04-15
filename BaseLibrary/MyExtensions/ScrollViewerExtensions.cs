using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BaseLibrary.MyExtensions
{
    public class ScrollViewerExtensions
    {
        public static readonly DependencyProperty AlwaysScrollToEndProperty = DependencyProperty.RegisterAttached("AlwaysScrollToEnd", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(false, AlwaysScrollToEndChanged));
        private static bool _autoScroll;


        private static void AlwaysScrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (scroll != null)
            {
                bool alwaysScrollToEnd = (e.NewValue != null) && (bool)e.NewValue;
                if (alwaysScrollToEnd)
                {
                    scroll.ScrollToEnd();
                    scroll.ScrollChanged += ScrollChanged;
                    // scroll.SizeChanged += Scroll_SizeChanged;
                }
                else { scroll.ScrollChanged -= ScrollChanged; /*scroll.ScrollChanged -= ScrollChanged; */}
            }
            else { throw new InvalidOperationException("The attached AlwaysScrollToEnd property can only be applied to ScrollViewer instances."); }
        }


        public static bool GetAlwaysScrollToEnd(ScrollViewer scroll)
        {
            if (scroll == null) { throw new ArgumentNullException("scroll"); }
            return (bool)scroll.GetValue(AlwaysScrollToEndProperty);
        }


        public static void SetAlwaysScrollToEnd(ScrollViewer scroll, bool alwaysScrollToEnd)
        {
            if (scroll == null) { throw new ArgumentNullException("scroll"); }
            scroll.SetValue(AlwaysScrollToEndProperty, alwaysScrollToEnd);
        }


        private static void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (scroll == null) { throw new InvalidOperationException("The attached AlwaysScrollToEnd property can only be applied to ScrollViewer instances."); }

            //确认是否需要移动到最下方
            if (e.ExtentHeightChange == 0) { _autoScroll = scroll.VerticalOffset == scroll.ScrollableHeight; }
            if (_autoScroll && e.ExtentHeightChange != 0) { scroll.ScrollToVerticalOffset(scroll.ExtentHeight); }
        }
    }
}
