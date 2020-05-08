using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Segment.Model;

namespace Segment.Flush
{
    class ActionSizeCalculator
    {
        public static int Calculate(BaseAction action)
        {
            return JsonConvert.SerializeObject(action).Length;
        }
    }
}
