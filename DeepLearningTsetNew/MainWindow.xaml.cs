using DeepLearningTset;
using HalconDotNet;
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

namespace DeepLearningTsetNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Local iconic variables 

        HObject ho_Image = null;

        // Local control variables 

        HTuple hv_ImageFiles = new HTuple(), hv_DLModelHandle = new HTuple();
        HTuple hv_DLDeviceHandles = new HTuple(), hv_DLPreprocessParam = new HTuple();
        HTuple hv_MaxNumInferenceImages = new HTuple(), hv_MetaData = new HTuple();
        HTuple hv_ClassificationThreshold = new HTuple(), hv_SegmentationThreshold = new HTuple();
        HTuple hv_DLDatasetInfo = new HTuple(), hv_WindowDict = new HTuple();
        HTuple hv_IndexInference = new HTuple(), hv_DLSample = new HTuple();
        HTuple hv_DLResult = new HTuple();


        public MainWindow()
        {
            InitializeComponent();
        }


        public void Disp_Text(int Row, int Column, string text, string color = "red", int font_size = 30)
        {
            try
            {
                HTuple hv_Font, hv_FontWithSize;

                //设置字体颜色
                hSmart.HalconWindow.SetColor(color);

                //获取系统字体列表
                HOperatorSet.QueryFont(hSmart.HalconWindow, out hv_Font);
                //Specify font name and size
                string font = $"-Bold-{font_size}";
                hv_FontWithSize = (hv_Font.TupleSelect(0)) + font;
                //设置文字大小
                hSmart.HalconWindow.SetFont(hv_FontWithSize);
                //设置显示的位置（坐标）
                hSmart.HalconWindow.SetTposition(Row, Column);
                //设置显示的内容
                hSmart.HalconWindow.WriteString(text);
            }
            catch
            {
                MessageBox.Show(text);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            HOperatorSet.SetDraw(hSmart.HalconWindow, "margin");
            //获取图像
            hv_ImageFiles.Dispose();
            HOperatorSet.ListFiles("D:/test_pro/halcon深度学习/果汁测试/混合图像",
                (new HTuple("files")).TupleConcat("follow_links"), out hv_ImageFiles);
            {
                HTuple ExpTmpOutVar_0;
                HOperatorSet.TupleRegexpSelect(hv_ImageFiles, (new HTuple("\\.(tif|tiff|gif|bmp|jpg|jpeg|jp2|png|pcx|pgm|ppm|pbm|xwd|ima|hobj)$")).TupleConcat(
                    "ignore_case"), out ExpTmpOutVar_0);
                hv_ImageFiles.Dispose();
                hv_ImageFiles = ExpTmpOutVar_0;
            }

            //读取模型
            hv_DLModelHandle.Dispose();
            //HOperatorSet.ReadDlModel("C:/Users/dell/AppData/Local/Programs/MVTec/HALCON-22.11-Steady/dl/pretrained_dl_classifier_compact.hdl",
            //   out hv_DLModelHandle);

            HOperatorSet.ReadDlModel("D:/test_pro/halcon深度学习/果汁测试/test.hdl",
                out hv_DLModelHandle);




            //Use either a GPU or CPU for evaluation and inference later.
            hv_DLDeviceHandles.Dispose();
            HOperatorSet.QueryAvailableDlDevices(((new HTuple("runtime")).TupleConcat("runtime")).TupleConcat(
                "id"), ((new HTuple("gpu")).TupleConcat("cpu")).TupleConcat(0), out hv_DLDeviceHandles);
            HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", 1);

            //设置预处理参数 Set preprocessing parameters and preprocess.

            ComFunc.create_dl_preprocess_param_from_model(hv_DLModelHandle, "none", "full_domain",
                new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);
            //
            hv_MaxNumInferenceImages.Dispose();
            hv_MaxNumInferenceImages = 10;

            //
            //Get thresholds for inference. These have been stored along with
            //the model in the meta data.
            hv_MetaData.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "meta_data", out hv_MetaData);
            //异常值分类阈值
            hv_ClassificationThreshold.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_ClassificationThreshold = ((hv_MetaData.TupleGetDictTuple(
                    "anomaly_classification_threshold"))).TupleNumber();
            }
            //分割阈值
            hv_SegmentationThreshold.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_SegmentationThreshold = ((hv_MetaData.TupleGetDictTuple(
                    "anomaly_segmentation_threshold"))).TupleNumber();
            }
            //
            //Create a dictionary with dataset parameters used for display.
            //分类字典
            hv_DLDatasetInfo.Dispose();
            HOperatorSet.CreateDict(out hv_DLDatasetInfo);
            HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_names", (new HTuple("ok")).TupleConcat(
                "nok"));
            HOperatorSet.SetDictTuple(hv_DLDatasetInfo, "class_ids", (new HTuple(0)).TupleConcat(
                1));
            //
            //Apply the model to test images.
            //窗体字典
            hv_WindowDict.Dispose();
            HOperatorSet.CreateDict(out hv_WindowDict);
            //循环读取并检测
            for (hv_IndexInference = 0; (int)hv_IndexInference <= (int)((new HTuple(hv_ImageFiles.TupleLength()
                )) - 1); hv_IndexInference = (int)hv_IndexInference + 1)
            {
                //
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    //ho_Image.Dispose();
                    HOperatorSet.ReadImage(out ho_Image, hv_ImageFiles.TupleSelect(hv_IndexInference));
                    hSmart.HalconWindow.ClearWindow();
                    
                    
                }
                //把图像放入字典
                ComFunc.gen_dl_samples_from_images(ho_Image, out hv_DLSample);
                //把模型放入字典
                ComFunc.preprocess_dl_samples(hv_DLSample, hv_DLPreprocessParam);
                //分类检测，输出结果
                hv_DLResult.Dispose();
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSample, new HTuple(), out hv_DLResult);
                //
                //Apply thresholds to classify regions and the entire image.
                //阈值判断
                ComFunc.threshold_dl_anomaly_results(hv_SegmentationThreshold, hv_ClassificationThreshold,
                    hv_DLResult);

                HTuple hv_ResultKeys;
                HOperatorSet.GetDictParam(hv_DLResult, "keys", new HTuple(), out hv_ResultKeys);
                HObject AnomalyRegion;
                HOperatorSet.GetDictObject(out AnomalyRegion, hv_DLResult, "anomaly_region");
                HObject AnomalyImage;
                //HOperatorSet.GetDictObject(out AnomalyImage, hv_DLResult, "anomaly_image");
                HOperatorSet.GetDictObject(out AnomalyImage, hv_DLSample, "image");
                hSmart.SetFullImagePart();
                hSmart.HalconWindow.DispObj(AnomalyImage);

                HTuple anomalyClass;     
                HOperatorSet.GetDictTuple(hv_DLResult, "anomaly_class", out anomalyClass);
                if (anomalyClass.S == "ok")
                {
                    Disp_Text(100, 100, "PASS", "green");
                }
                else
                {
                    Disp_Text(100, 100, "NG", "red");
                    
                    HOperatorSet.DispRegion(AnomalyRegion, hSmart.HalconWindow);
                }

                if (anomalyClass.S != "ok")
                {
                    MessageBox.Show("");
                }
                
                HTuple area; HTuple row; HTuple col;
                HOperatorSet.AreaCenter(AnomalyRegion, out area, out row, out col);
                Trace.WriteLine($"{area},{row},{col},{anomalyClass}");
                
            }
        }
    }
}
