using System;
using System.IO;
using YamlDotNet.Serialization;

namespace BaseLibrary.Tools
{
    public class ConfigCtrl
    {
        /// <summary>
        /// 序列化操作  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        static public void Save<T>(T obj, string filename="config")
        {
            string _filePath = Directory.GetCurrentDirectory() + $"\\Config\\{filename}.yaml";
            FileInfo fi = new FileInfo(_filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            StreamWriter yamlWriter = File.CreateText(_filePath);
            Serializer yamlSerializer = new Serializer();
            yamlSerializer.Serialize(yamlWriter, obj);
            yamlWriter.Close();
        }

        /// <summary>
        /// 泛型反序列化操作  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        static public T Read<T>(string filename="config")
        {
            string _filePath = Directory.GetCurrentDirectory() + $"\\Config\\{filename}.yaml";
            if (!File.Exists(_filePath))
            {
                return default;
            }
            StreamReader yamlReader = File.OpenText(_filePath);
            Deserializer yamlDeserializer = new Deserializer();

            //读取持久化对象  
            try
            {
                T info = yamlDeserializer.Deserialize<T>(yamlReader);
                yamlReader.Close();
                return info;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return default;
            }
        }



        static public void Save(object obj)
        {

        }

        static public void Read()
        {

        }




    }

}
