using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToolsLibrary
{
    /// <summary>
    /// 正则匹配工具
    /// </summary>
    public class RegexTool
    {
        //获取标签中的内容
        public static string? GetP(string str, string p_name)
        {
            if (!str.Contains(p_name))
            {
                return null;
            }
            return Regex.Match(str, $"(?<=<{p_name}>)(.|\n)*(?=</{p_name}>)").Value;
        }

        //json根据key获取value的值
        public static string? GetJValue(string str_json, string key_name)
        {
            if (!str_json.Contains(key_name))
            {
                return null;
            }
            Match match = Regex.Match(str_json, $"(?<=\"{key_name}\"\\s*:\\s*\")((.|\n)*?(?=\"))");
            return match.Groups.Values.ElementAtOrDefault(1)?.Value;
        }

        //json替换key对应的value的值
        public static string? ReplaceJValue(string str_json, string key_name, string replacement)
        {
            if (!str_json.Contains(key_name))
            {
                return null;
            }
            return Regex.Replace(str_json, $"(?<=\"{key_name}\"\\s*:\\s*\")((.|\n)*?(?=\"))", replacement);
        }
    }
}
