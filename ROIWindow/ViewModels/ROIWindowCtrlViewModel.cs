using HalconDotNet;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ROIWindow.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToolsLibrary;

namespace ROIWindow.ViewModels
{
    public class ROIWindowCtrlViewModel : BindableBase
    {
        public HSmartWindowControlWPF? hSmart;

        public List<HDrawingObject> drawingObjList = new List<HDrawingObject>();


        private HTuple imageWidth = new HTuple();
        private HTuple imageHeight = new HTuple();

        //鼠标当前位置
        public double mouseCurRow=0;
        public double mouseCurCol=0;
        //记录位置
        public double FixRow = 0;
        public double FixCol = 0;

        private HImage curImg = new HImage();

        public HImage CurImg
        {
            get { return curImg; }
            //仅内部可以设置！
            private set
            {
                if (curImg != null)
                {
                    curImg.Dispose(); //这里是解决内存泄露的关键
                }
                SetProperty(ref curImg, value);

            }
        }

        private string curImgPath;

        public string CurImgPath
        {
            get { return curImgPath; }
            set { SetProperty(ref curImgPath, value); }
        }


        /// <summary>
        /// 当前选中的区域
        /// </summary>
        public HDrawingObject curDrawingObj = null;

        /// <summary>
        /// ROI类型
        /// </summary>
        public enum DrawingType
        {
            [Description("直线")]
            LINE,
            [Description("矩形")]
            RECTANGLE1,
            [Description("仿射矩形")]
            RECTANGLE2,
            [Description("圆")]
            CIRCLE,
            [Description("椭圆")]
            ELLIPSE,
            [Description("圆弧")]
            Arc,
        }

        private bool canMove = true;

        public bool CanMove
        {
            get { return canMove; }
            set { SetProperty(ref canMove, value); }

        }

        private bool canShowImageMessage = true;

        public bool CanShowImageMessage
        {
            get { return canShowImageMessage; }
            set { SetProperty(ref canShowImageMessage, value); }
        }


        private string imageMessage = "";

        public string ImageMessage
        {
            get { return imageMessage; }
            set { SetProperty(ref imageMessage, value); }
        }

        #region 命令声明
        public DelegateCommand AddPictrueCmd { get; set; }
        public DelegateCommand ClearCmd { get; set; }
        public DelegateCommand ClearROICmd { get; set; }
        public DelegateCommand<string> SaveImgCmd { get; set; }
        public DelegateCommand<string> SaveScreenshotCmd { get; set; }
        public DelegateCommand ProprecessCmd { get; set; }
        public DelegateCommand PreviewMouseRightButtonDownCmd { get; set; }
        public DelegateCommand<string> ROICmd { get; set; }
        public DelegateCommand<RoutedEventArgs> EventLoadedCommand { get; set; }
        #endregion


        #region 事件定义
        public event HSmartWindowControlWPF.HMouseEventHandlerWPF HMouseMove;
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event MouseButtonEventHandler MouseRightButtonDown;
        #endregion

        public ROIWindowCtrlViewModel()
        {
            //窗口加载事件
            EventLoadedCommand = new DelegateCommand<RoutedEventArgs>((e) =>
            {
                DependencyObject? obj = e.OriginalSource as DependencyObject;
                if (obj != null)
                {
                    hSmart = FindTree.GetChildObjectFirst<HSmartWindowControlWPF>(obj);
                    //相关事件订阅
                    hSmart.HMouseMove -= HSmart_HMouseMove;
                    hSmart.MouseLeftButtonDown -= HSmart_MouseLeftButtonDown;
                    hSmart.MouseRightButtonUp -= HSmart_MouseRightButtonUp;


                    hSmart.MouseRightButtonUp += HSmart_MouseRightButtonUp;
                    hSmart.HMouseMove += HSmart_HMouseMove;
                    //右键菜单和右键事件，居然不会冲突，挺好的！
                    hSmart.MouseLeftButtonDown += HSmart_MouseLeftButtonDown;
                }

            });

            //界面清除
            ClearCmd = new DelegateCommand(() => {
                hSmart?.HalconWindow.ClearWindow();
            });

            ClearROICmd = new DelegateCommand(() => {
                foreach (var item in drawingObjList)
                {
                    item?.ClearDrawingObject();
                }
                drawingObjList.Clear();
            });

            //保存原图
            SaveImgCmd = new DelegateCommand<string> ((str) =>
            {
                try
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(str);
                    var fullPath = OpenDialog.SaveFileDialog("tiff", fileName);
            
                    HOperatorSet.WriteImage(this.CurImg, "tiff", 0, fullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            //保存窗口截图
            SaveScreenshotCmd = new DelegateCommand<string>((str) =>
            {
                try
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(str);
                    var fullPath = OpenDialog.SaveFileDialog("png", fileName);
                    //截取窗口图
                    HOperatorSet.DumpWindow(hSmart.HalconWindow, "png best", fullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            //预处理命令
            ProprecessCmd = new DelegateCommand(() => {
                ProprecessImageWindow proprecessImageWindow = new ProprecessImageWindow();
                proprecessImageWindow.hSmart = hSmart;
                proprecessImageWindow.ShowDialog();
            });

            //图片加载
            AddPictrueCmd = new DelegateCommand(() =>
            {
                CurImgPath = OpenDialog.OpenPictureFileDialog();
                if (CurImgPath != null)
                {
                    ShowImage(CurImgPath);

                    //双击控制自适应
                    hSmart.HDoubleClickToFitContent = true;
                    hSmart.SetFullImagePart();
                }

            });


            PreviewMouseRightButtonDownCmd = new DelegateCommand(() => {
                FixRow = mouseCurRow;
                FixCol = mouseCurCol;
            });

            //绘图操作~~
            ROICmd = new DelegateCommand<string>((str) =>
            {
                HDrawingObject.HDrawingObjectType hDrawingObjectType = HDrawingObject.HDrawingObjectType.CIRCLE;

                var h_tuple = new HTuple[] { 100, 100, 50 };
                //画圆形
                if (str == DrawingType.CIRCLE.ToString())
                {
                    //起点x，起点y, 半径
                    h_tuple = new HTuple[] { FixRow, FixCol, 50 };
                    hDrawingObjectType = HDrawingObject.HDrawingObjectType.CIRCLE;
                }
                //画矩形
                else if (str == DrawingType.RECTANGLE1.ToString())
                {
                    //起点x，起点y, 半径
                    h_tuple = new HTuple[] { FixRow, FixCol, FixRow + 100, FixCol + 100};
                    hDrawingObjectType = HDrawingObject.HDrawingObjectType.RECTANGLE1;
                }

                //画仿射矩形
                else if (str == DrawingType.RECTANGLE2.ToString())
                {
                    //起点x，起点y, 半径
                    h_tuple = new HTuple[] { FixRow, FixCol, 0, 100, 50 };
                    hDrawingObjectType = HDrawingObject.HDrawingObjectType.RECTANGLE2;
                }
                //画椭圆
                else if (str == DrawingType.ELLIPSE.ToString())
                {
                    //起点x，起点y, 旋转弧度， 半径a， 半径b
                    h_tuple = new HTuple[] { FixRow, FixCol, 0, 25, 50 };
                    hDrawingObjectType = HDrawingObject.HDrawingObjectType.ELLIPSE;
                }
                //画直线
                else if (str == DrawingType.LINE.ToString())
                {
                    //起点x，起点y, 终点x，终点y, 
                    h_tuple = new HTuple[] { FixRow, FixCol, FixRow, FixCol + 100 };
                    hDrawingObjectType = HDrawingObject.HDrawingObjectType.LINE;
                }
                else
                {
                    return;
                }

                var drawingObj = HDrawingObject.CreateDrawingObject(hDrawingObjectType, h_tuple);
                drawingObjList.Add(drawingObj);//不用list装起来的话，这个变量会被释放，圆形就会消失。
                //将对象关联到窗口
                hSmart?.HalconWindow.AttachDrawingObjectToWindow(drawingObj);

                //----设置，回调
                drawingObj.OnSelect(OnSelectCallbackClass);
                drawingObj.OnDrag(OnDragCallbackClass);
                drawingObj.OnResize(OnResizeCallbackClass);

                foreach (var item in h_tuple)
                {
                    item.Dispose();
                }

            });
        }

        

        private void HSmart_HMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            //事件触发
            HMouseMove?.Invoke(sender, e);
            //
            mouseCurCol = e.Column;
            mouseCurRow = e.Row;
            ShowImageInfo(e.Row, e.Column);
        }

        

        private void HSmart_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //事件触发
            MouseLeftButtonDown?.Invoke(sender, e);
        }

        private void HSmart_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseRightButtonDown?.Invoke(sender, e);    
        }


        #region ROI 相关交互操作
        public void OnSelectCallbackClass(HDrawingObject drawid, HWindow window, string type)
        {
            curDrawingObj = drawingObjList.Find(obj => obj.ID.Equals(drawid.ID));
        }


        public void OnDragCallbackClass(HDrawingObject drawid, HWindow window, string type)
        {
            curDrawingObj = drawingObjList.Find(obj => obj.ID.Equals(drawid.ID));
        }


        public void OnResizeCallbackClass(HDrawingObject drawid, HWindow window, string type)
        {
            curDrawingObj = drawingObjList.Find(obj => obj.ID.Equals(drawid.ID));
        }
        #endregion
        

        public void ShowImageInfo(double row, double column)
        {
            try
            {
                double positionX, positionY;
                bool _isXOut = true, _isYOut = true;
                HTuple channel_count = new HTuple();
                if (CurImg == null) return;
                HOperatorSet.CountChannels(CurImg, out channel_count);
                positionY = row;
                positionX = column;
                _isXOut = (positionX < 0 || positionX >= imageWidth.I);
                _isYOut = (positionY < 0 || positionY >= imageHeight.I);
                ImageMessage = $" X:{positionX.ToString("f2")},Y:{positionY.ToString("f2")}";
                if (!_isXOut && !_isYOut)
                {
                    if ((int)channel_count == 1)
                    {
                        HOperatorSet.GetGrayval(CurImg, positionY, positionX, out HTuple grayVal);
                        ImageMessage += $" | Gray:{grayVal.D:000}";
                        grayVal.Dispose();
                    }
                    else if ((int)channel_count == 3)
                    {
                        HObject redChannel = null, greenChannel = null, blueChannel = null;
                        HOperatorSet.GenEmptyObj(out redChannel);
                        HOperatorSet.GenEmptyObj(out greenChannel);
                        HOperatorSet.GenEmptyObj(out blueChannel);
                        redChannel.Dispose();
                        greenChannel.Dispose();
                        blueChannel.Dispose();

                        HOperatorSet.AccessChannel(CurImg, out redChannel, 1);
                        HOperatorSet.AccessChannel(CurImg, out greenChannel, 2);
                        HOperatorSet.AccessChannel(CurImg, out blueChannel, 3);

                        HOperatorSet.GetGrayval(redChannel, positionY, positionX, out HTuple grayValRed);
                        HOperatorSet.GetGrayval(greenChannel, positionY, positionX, out HTuple grayValGreen);
                        HOperatorSet.GetGrayval(blueChannel, positionY, positionX, out HTuple grayValBlue);

                        redChannel.Dispose();
                        greenChannel.Dispose();
                        blueChannel.Dispose();
                        ImageMessage += $" | R:{grayValRed.D:000},G:{grayValGreen.D:000},B:{grayValBlue.D:000}";
                        ImageMessage += $" | W:{imageWidth},H:{imageHeight}";
                        grayValRed.Dispose();
                        grayValGreen.Dispose();
                        grayValBlue.Dispose();
                    }
                }
                channel_count.Dispose();
            }
            catch
            {
                return;
            }
        }

        

        public void ShowImage(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new Exception($"不存在路径 {path}");
            }
            hSmart?.HalconWindow.ClearWindow(); //必须清理掉不然会显示多张图片
            CurImg = new HImage(path);
            CurImg.GetImageSize(out imageWidth, out imageHeight);
            //这样显示的才是彩图 | hSmart?.HalconWindow.DispImage(CurImg); 这样显示的黑白图
            hSmart?.HalconWindow.DispObj(CurImg);
            //hSmart?.HalconWindow.AttachBackgroundToWindow(CurImg);
        }


        public void ShowImage(HObject obj)
        {
            CurImg = new HImage(obj);
            hSmart?.HalconWindow.DispObj(CurImg);
            obj.Dispose();
        }


    }
}
