using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDG
{
    [System.Serializable]
    public class KeyValueProtocol
    {
        Dictionary<string, string> keyValuePairs=new Dictionary<string, string>();
        public int Count
        {
            get
            {
                return keyValuePairs.Count;
            }
        }
 
        static string Encode(string value)
        {
            StringBuilder strb = new StringBuilder(value);
            strb.Replace("@", "@@");
            strb.Replace(",", "@k-");
            strb.Replace(":", "@v-");

            return strb.ToString();
        }
        static string Decode(string value)
        {
            StringBuilder strb = new StringBuilder(value);
            strb.Replace("@@", "@");
            strb.Replace("@k-", ",");
            strb.Replace("@v-", ":");
            return strb.ToString();
        }
        public string this[string key]
        {
            get
            {
                if (keyValuePairs.ContainsKey(key))
                {
                    return Decode(keyValuePairs[key]);
                }
                else
                {
                    return "";
                }
            }
            set
            {

                if (keyValuePairs.ContainsKey(key))
                {
                    keyValuePairs[key] = Encode(value);
                }
                else
                {
                    keyValuePairs.Add(key, Encode(value));
                }


            }
        }
        public KeyValueProtocol()
        {

        }
        public KeyValueProtocol(string initInfo)
        {
            
            var kvs= initInfo.Split(new char[] {','});
         
            keyValuePairs.Clear();
            foreach (var kv in kvs)
            {
                if (kv == "") { continue; }
                var str = kv.Split(new char[] { ':' });
                keyValuePairs.Add(str[0], str[1]);
            }
        }
        public string GetString()
        {
            StringBuilder strb = new StringBuilder();
            foreach (var kv in keyValuePairs)
            {
                strb.Append(kv.Key + ":" + kv.Value);
                if (kv.Key != keyValuePairs.Last().Key)
                {
                    strb.Append(",");
                }
            }
            return strb.ToString();
        }
        public void Clear()
        {
            keyValuePairs.Clear();
        }
    }
}
