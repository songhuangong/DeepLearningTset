using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfConverter
{
    public class Bool2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (parameter == null) return DependencyProperty.UnsetValue; 
            if (value == null) return DependencyProperty.UnsetValue;
            
            //将参数字符分段 parray[0]为比较值，parray[1]为true返回值，parray[2]为false返回值
            string[] parray = parameter.ToString().ToLower().Split(':'); 
            if (value is bool && parray.Length >=2)
            {
                //value为bool，true返回parray[1]，false返回parray[2]
                return (bool)value? parray[0] : parray[1];
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return DependencyProperty.UnsetValue;
        }
    }
}
