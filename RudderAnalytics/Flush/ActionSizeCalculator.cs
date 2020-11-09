using Newtonsoft.Json;
using RudderStack.Model;

namespace RudderStack.Flush
{
    class ActionSizeCalculator
    {
        public static int Calculate(BaseAction action)
        {
            return JsonConvert.SerializeObject(action).Length;
        }
    }
}
