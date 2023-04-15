using System;
using System.IO;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;


namespace ToolsLibrary
{
    public class ConfigFileCtrl
    {
        static JsonSerializerOptions options;

        static ConfigFileCtrl()
        {
            options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
        }

        /// <summary>
        /// 序列化操作  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        static public void Save<T>(T obj, string filename = "config")
        {
            string _filePath = Directory.GetCurrentDirectory() + $"\\Config\\{filename}.yaml";
            FileInfo fi = new FileInfo(_filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }


            //StreamWriter yamlWriter = File.CreateText(_filePath);


            string str = System.Text.Json.JsonSerializer.Serialize(obj, ConfigFileCtrl.options);
            str = JsonHelper.FormatJson(str);
            File.WriteAllText(_filePath, str);



            //Serializer yamlSerializer = new Serializer();
            //yamlSerializer.Serialize(yamlWriter, obj);
            //yamlWriter.Close();
        }

        /// <summary>
        /// 泛型反序列化操作  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        static public T Read<T>(string filename = "config")
        {
            string _filePath = Directory.GetCurrentDirectory() + $"\\Config\\{filename}.yaml";
            if (!File.Exists(_filePath))
            {
                return default;
            }


            //读取持久化对象  
            try
            {
                T info = JsonSerializer.Deserialize<T>(File.ReadAllText(_filePath));

                return info;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default;
            }
        }
    }
}
