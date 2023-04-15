using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ROIWindow.ViewModels
{
    internal class ProprecessImageViewModel : BasePropertyChanged
    {
       
        private int channel = 3;
        /// <summary>
        /// 颜色通道
        /// </summary>
        public int ColorChannel
        {
            get { return channel; }
            set {
                channel = value;
                OnPropertyChanged(); }
        }


        private int thresholdLeft;
        /// <summary>
        /// 二值化左值
        /// </summary>
        public int ThresholdLeft
        {
            get { return thresholdLeft; }
            set { thresholdLeft = value; OnPropertyChanged(); }
        }

        private int thresholdRight;
        /// <summary>
        /// 二值化右值
        /// </summary>
        public int ThresholdRight
        {
            get { return thresholdRight; }
            set { thresholdRight = value; OnPropertyChanged(); }
        }

        private int meanWith = 13;
        /// <summary>
        /// 均值滤波的宽度
        /// </summary>
        public int MeanWith
        {
            get { return meanWith; }
            set { meanWith = value; OnPropertyChanged(); }
        }


        private int meanHigh = 13;
        /// <summary>
        /// 均值滤波的高度
        /// </summary>
        public int MeanHigh
        {
            get { return meanHigh; }
            set { meanHigh = value; OnPropertyChanged(); }
        }


        private string lightDark = "light";
        /// <summary>
        /// 动态阈值函数的明暗选择
        /// </summary>
        public string LightDark
        {
            get { return lightDark; }
            set { lightDark = value; OnPropertyChanged(); }
        }

        private double offsetLightDark = 5;

        public double OffsetLightDark
        {
            get { return offsetLightDark; }
            set { offsetLightDark = value; OnPropertyChanged(); }
        }




        /// <summary>
        /// 选择全局阈值
        /// </summary>
        public bool SelectGlobal { get; set; } = false;
    }
}
