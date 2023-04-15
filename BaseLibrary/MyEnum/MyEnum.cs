using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.MyEnum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum ControlType
    {
        input = 0,
        date,
        select,
        judge,
        shift,
        machine,
        testMachine,
    }
}
