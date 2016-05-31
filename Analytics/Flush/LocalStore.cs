//-----------------------------------------------------------------------
// <copyright file="LocalStore.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_5_3_OR_NEWER

namespace Segment.Flush
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class LocalStore
    {
        private BaseActionConverter baseActionConverter;
        private LinkedList<BaseAction> dataList;
        private bool isDirty;

        public LocalStore()
        {
            this.baseActionConverter = new BaseActionConverter();
            this.FileName = Path.Combine(UnityEngine.Application.persistentDataPath, "analytics.json");
            this.dataList = this.Load();
        }

        public int Count
        {
            get { return this.dataList.Count; }
        }

        public string FileName
        {
            get; private set;
        }

        public List<BaseAction> PeakTop(int count)
        {
            List<BaseAction> results = new List<BaseAction>(count);

            foreach (BaseAction baseAction in this.dataList)
            {
                results.Add(baseAction);

                if (results.Count == count)
                {
                    break;
                }
            }

            return results;
        }

        public void Add(BaseAction baseAction)
        {
            this.dataList.AddLast(baseAction);
            this.isDirty = true;
        }

        public void Remove(BaseAction baseAction)
        {
            this.dataList.Remove(baseAction);
            this.isDirty = true;
        }

        public void Save()
        {
            if (this.isDirty == false)
            {
                return;
            }

            string json = JsonConvert.SerializeObject(this.dataList);
            File.WriteAllText(this.FileName, json);
            this.isDirty = false;
        }

        private LinkedList<BaseAction> Load()
        {
            LinkedList<BaseAction> result = null;

            if (File.Exists(this.FileName))
            {
                try
                {
                    string json = File.ReadAllText(this.FileName);
                    result = JsonConvert.DeserializeObject<LinkedList<BaseAction>>(json, this.baseActionConverter);
                }
                catch (System.Exception ex)
                {
                    var args = new Dict
                    {
                        { "file", this.FileName },
                        { "exception", ex.ToString() }
                    };

                    Segment.Logger.Error(string.Format("Error deserializing local store {0}", this.FileName), args);
                }
            }

            // if the file didn't exist, or there was an error, then return a new one
            return result ?? new LinkedList<BaseAction>();
        }
        
        public class BaseActionConverter : JsonConverter
        {
            public override bool CanWrite
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(BaseAction);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);

                string eventType = jo["type"].Value<string>();

                switch (eventType)
                {
                    case "track":
                    {
                        string userId = jo["userId"].Value<string>();
                        string eventName = jo["event"].Value<string>();
                        Properties properties = jo["properties"].ToObject<Properties>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Track(userId, eventName, properties, options);
                    }

                    case "screen":
                    {
                        string userId = jo["userId"].Value<string>();
                        string name = jo["name"].Value<string>();
                        string category = jo["category"].Value<string>();
                        Properties properties = jo["properties"].ToObject<Properties>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Screen(userId, name, category, properties, options);
                    }

                    case "page":
                    {
                        string userId = jo["userId"].Value<string>();
                        string name = jo["name"].Value<string>();
                        string category = jo["category"].Value<string>();
                        Properties properties = jo["properties"].ToObject<Properties>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Page(userId, name, category, properties, options);
                    }
                    
                    case "identify":
                    {
                        string userId = jo["userId"].Value<string>();
                        Traits traits = jo["traits"].ToObject<Traits>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Identify(userId, traits, options);
                    }

                    case "group":
                    {
                        string userId = jo["userId"].Value<string>();
                        string groupId = jo["groupId"].Value<string>();
                        Traits traits = jo["traits"].ToObject<Traits>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Group(userId, groupId, traits, options);
                    }

                    case "alias":
                    {
                        string previousId = jo["previousId"].Value<string>();
                        string userId = jo["userId"].Value<string>();
                        Options options = jo["properties"].ToObject<Options>();

                        return new Alias(previousId, userId, options);
                    }

                    default:
                        return null;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}

#endif
