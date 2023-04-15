using BaseLibrary.MyEnum;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.MyEvent
{
    /// <summary>
    /// 一个弹窗事件
    /// </summary>
    public class GrowlEvent : PubSubEvent<GrowMsg>
    {
    }



    public class GrowMsg
    {
        public EnumAlarmType enumAlarmType;
        public string message;
        public string token;
    }

}
