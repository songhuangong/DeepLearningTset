using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsLibrary
{
    public class OpenDialog
    {
        public static string? OpenPictureFileDialog()
        {
            // 实例化一个文件选择对象
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "打开图像文件";
            dialog.Multiselect = false; //能否多开
            dialog.RestoreDirectory = true;//设置对话框是否记忆之前打开的目录
            dialog.DefaultExt = ".png";  // 设置默认类型
                                         // 设置可选格式
            dialog.Filter = @"图像文件(*.jpg,*.png,*.tif,*.gif,*.bmp)|*jpeg;*.jpg;*.png;*.tif;*.tiff;*.bmp;*.gif
                                  |JPEG(*.jpeg, *.jpg)|*.jpeg;*.jpg|PNG(*.png)|*.png|GIF(*.gif)|*.gif
                                  |TIF(*.tif,*.tiff)|*.tif;*.tiff|All files (*.*)|*.*";
            // 打开选择框选择
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName; // 获取选择的文件名
            }
            else
            {
                return null;
            }
        }



        public static string[] OpenFileDialog(string filter_str = "txt")
        {
            // 实例化一个文件选择对象
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "打开文件";
            dialog.Multiselect = true; //能否多开
            dialog.RestoreDirectory = true;//设置对话框是否记忆之前打开的目录
            dialog.DefaultExt = $".{filter_str}";  // 设置默认类型
                                         // 设置可选格式
            dialog.Filter = $"文件 (*.{filter_str})|*.{filter_str}|All files (*.*)|*.*";


            // 打开选择框选择
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileNames; // 获取选择的文件名
            }
            else
            {
                return null;
            }
        }


        public static string SaveFileDialog(string filter_str="txt", string fileName="")
        {
            // 实例化一个文件选择对象
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "保存文件";           
            if (filter_str == "mb")
            {
                dialog.InitialDirectory = Directory.GetCurrentDirectory() + $"\\模板文件";
            }
            else
            {
                dialog.RestoreDirectory = true;//设置对话框是否记忆之前打开的目录
            }
            
            dialog.DefaultExt = $".{filter_str}";  // 设置默认类型
                                                   // 设置可选格式
            dialog.Filter = $"文件 (*.{filter_str})|*.{filter_str}|All files (*.*)|*.*";

            if (fileName != "")
            {
                dialog.FileName = fileName;
            }
            


            // 打开选择框选择
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName; // 获取选择的文件名
            }
            else
            {
                return null;
            }
        }
    }
}
