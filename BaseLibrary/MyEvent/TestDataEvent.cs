using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.MyEvent
{
    public class TestDataEvent : PubSubEvent<string>
    {

    }


    public class CanReturn
    {
        public string data;
        public object session;
    }

    public class TestDataEventCanReturn : PubSubEvent<CanReturn>
    {

    }
}
