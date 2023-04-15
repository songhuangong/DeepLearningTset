using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.CodeDom;

namespace BaseLibrary.Tools
{
    public static class TypeExtension
    {
        /// <summary>
        /// 对象的深拷贝对象。
        /// 注意“T”及其里面的引用类型必须标记为可序列化。[Serializable]
        /// </summary>
        public static T Copy<T>(this T obj)
            where T : class
        {

            var CloneObject = System.Text.Json.JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));

            return CloneObject;
            
        }
    }
}
