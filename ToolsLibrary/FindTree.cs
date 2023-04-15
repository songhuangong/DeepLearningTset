using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ToolsLibrary
{
    public class FindTree
    {
        /// <summary>
        /// 根据类型查找子元素
        /// 调用形式：  List<StackPanel> initToolBarWeChatUserSp = GetChildObjects<StackPanel>(name, typeof(StackPanel));
        /// </summary>
        /// <typeparam name="T">查找类型</typeparam>
        /// <param name="obj">查询对象</param>
        /// <returns></returns>
        static public List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            Type typename = typeof(T);

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }


        static public T GetChildObjectFirst<T>(DependencyObject obj) where T : FrameworkElement
        {
            List<T> childList = new List<T>();

            childList = GetChildObjects<T>(obj);
            if (childList.Count > 0)
            {
                return childList[0];
            }
            else
            {
                return null;
            }

        }



        /// <summary>
        /// 获取父可视对象中第一个指定类型的子可视对象
        /// </summary>
        /// <typeparam name="T">可视对象类型</typeparam>
        /// <param name="parent">父可视对象</param>
        /// <returns>第一个指定类型的子可视对象</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }



        static public T GetChildObjectWithName<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            List<T> childList = new List<T>();

            childList = GetChildObjects<T>(obj);

            foreach (var item in childList)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
