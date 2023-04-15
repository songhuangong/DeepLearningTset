using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ToolsLibrary
{
    public class Mac
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
    }
}
