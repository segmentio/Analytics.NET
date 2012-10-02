using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [Serializable]
    public class ApiDictionary : ISerializable, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {

        public Dictionary<string, object> dict;

        public ApiDictionary()
        {
            dict = new Dictionary<string, object>();
        }

        protected ApiDictionary(SerializationInfo info, StreamingContext context)  
        {  
            dict = new Dictionary<string, object>();  
            foreach (var entry in info)  
            {  
                dict.Add(entry.Name, entry.Value);  
            }  
        }  

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (string key in dict.Keys)
            {
                info.AddValue(key, this[key]);
            }
        }

        public void Add(string key, object value)
        {
            dict.Add(key, value);
        }

        public void Remove(string key)
        {
            dict.Remove(key);
        }


        public object this[string index]
        {
            set { dict[index] = value; }
            get { return dict[index]; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }


    }
}
