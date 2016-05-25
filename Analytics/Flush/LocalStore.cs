//-----------------------------------------------------------------------
// <copyright file="LocalStore.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public class LocalStore<T>
    {
        private LinkedList<T> dataList;
        private bool isDirty;

        public LocalStore(string storeName)
        {
            this.FileName = string.Format(@"C:\Users\brgishy\Desktop\{0}.json", storeName);
            this.dataList = this.Load();
        }

        public int Count
        {
            get { return dataList.Count; }
        }

        public string FileName
        {
            get; private set;
        }

        public List<T> PeakTop(int count)
        {
            List<T> results = new List<T>(count);

            foreach (T t in this.dataList)
            {
                results.Add(t);

                if (results.Count == count)
                {
                    break;
                }
            }

            return results;
        }

        public void Add(T data)
        {
            this.dataList.AddLast(data);
            this.isDirty = true;
        }

        public void Remove(T data)
        {
            this.dataList.Remove(data);
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

        private LinkedList<T> Load()
        {
            LinkedList<T> result = null;

            if (File.Exists(this.FileName))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<LinkedList<T>>(this.FileName);
                }
                catch
                {
                    Segment.Logger.Error(string.Format("Error deserializing local store {0}", this.FileName));
                }
            }

            // if the file didn't exist, or there was an error, then return a new one
            return result ?? new LinkedList<T>();
        }
    }
}
