using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BaseLibrary.Tools
{
    public class Tool
    {
        static public List<string> GetMacsByWMI()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        macs.Add(mac);
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
            }

            return macs;
        }

        /// <summary>
        /// 获取首个MAC地址
        /// </summary>
        /// <returns></returns>
        static public string GetMac()
        {
            var list = GetMacsByWMI();
            if (list != null)
            {
                if (list.Count > 0)
                {
                    return list[0].Replace(":", "-");
                }
                return null;
            }
            else
            {
                return null;
            }
        }



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
