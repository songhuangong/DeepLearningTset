using HalconDotNet;
using ROIWindow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ROIWindow.Views
{
    /// <summary>
    /// ProprecessImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProprecessImageWindow : Window
    {

        ProprecessImageViewModel vm = new ProprecessImageViewModel();

        public HSmartWindowControlWPF hSmart;

        public HImage CurImg;
        HObject NowRegions;

        public ProprecessImageWindow()
        {
            InitializeComponent();
            this.DataContext = vm;
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurImg = hSmart.HalconWindow.DumpWindowImage();
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            hSmart.HalconWindow.DispObj(NowRegions);
            hSmart.HalconWindow.AttachBackgroundToWindow(CurImg);
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            hSmart.HalconWindow.DispObj(CurImg);
        }





        private void OffsetChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.OffsetLightDark = e.NewValue;
            MeanDynThreshold();
        }

        private void LeftThresholdChange(object sender, RoutedEventArgs e)
        {
            vm.ThresholdLeft = int.Parse(e.OriginalSource.ToString());
            UpdataThreshold();
        }

        private void RightThresholdChange(object sender, RoutedEventArgs e)
        {
            vm.ThresholdRight = int.Parse(e.OriginalSource.ToString());
            UpdataThreshold();
        }




        /// <summary>
        /// 均值滤波 + 动态阈值
        /// </summary>
        public void MeanDynThreshold()
        {
            if (CurImg == null || !CurImg.IsInitialized())
            {
                return;
            }

            hSmart.HalconWindow.ClearWindow();
            hSmart.HalconWindow.AttachBackgroundToWindow(CurImg);

            HObject ho_ImageMean;
            HOperatorSet.MeanImage(CurImg, out ho_ImageMean, vm.MeanWith, vm.MeanHigh);
            HOperatorSet.DynThreshold(CurImg, ho_ImageMean, out NowRegions, vm.OffsetLightDark, vm.LightDark);

            ho_ImageMean.Dispose();

            hSmart.HalconWindow.SetColor("green");
            hSmart.HalconWindow.DispObj(NowRegions);
            hSmart.HalconWindow.AttachBackgroundToWindow(CurImg);
            
        }


        /// <summary>
        /// 均值滤波 + 动态阈值
        /// </summary>
        static public HObject MeanDynThreshold(HObject img, int meanWith, int meanHigh, double offsetLightDark, string lightDark)
        {
            if (img == null || !img.IsInitialized())
            {
                return null;
            }

            HObject NowRegions;



            HObject ho_ImageMean;
            HOperatorSet.MeanImage(img, out ho_ImageMean, meanWith, meanHigh);
            HOperatorSet.DynThreshold(img, ho_ImageMean, out NowRegions, offsetLightDark, lightDark);

            ho_ImageMean.Dispose();

            return NowRegions;
        }


        void UpdataThreshold()
        {
            HObject ho_ImageChannel;




            if (CurImg == null || !CurImg.IsInitialized())
            {
                return;
            }

            hSmart.HalconWindow.ClearWindow();
            hSmart.HalconWindow.AttachBackgroundToWindow(CurImg);

            var cnt = CurImg.CountChannels().I;

            if (cnt > 1)
            {
                //访问通道3
                HOperatorSet.AccessChannel(CurImg, out ho_ImageChannel, vm.ColorChannel);
                HOperatorSet.Threshold(ho_ImageChannel, out NowRegions, vm.ThresholdLeft, vm.ThresholdRight);
            }
            else
            {
                HOperatorSet.Threshold(CurImg, out NowRegions, vm.ThresholdLeft, vm.ThresholdRight);
            }



            //二值化区域
            hSmart?.HalconWindow.SetColor("red");
            hSmart?.HalconWindow.DispObj(NowRegions);
        }

        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectGlobal)
            {
                UpdataThreshold();
            }
            else
            {
                //均值滤波 + 动态阈值
                MeanDynThreshold();
            }
        }
    }
}
