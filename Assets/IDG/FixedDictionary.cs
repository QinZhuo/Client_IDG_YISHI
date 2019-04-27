using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace IDG
{
    [System.Serializable]
    public struct KeyFixed{
        public string key;
        public Fixed value;
        public KeyFixed(string key, Fixed value)
        {
            this.key = key;
            this.value = value;
        }
    }
  
    [System.Serializable]
    public class FixedDictionary:ISerializable
    {
        public List<KeyFixed> dic = new List<KeyFixed>();

        public Fixed this[string key]
        {
            get
            {

                foreach (var kv in dic)
                {
                    if (kv.key == key)
                    {
                        return kv.value;
                    }
                }
                {
                    Debug.LogWarning("不存在字典数据[" + key + "]");
                    return 0.ToFixed();
                }
            }
        }

        public void Deserialize(ByteProtocol protocol)
        {
            dic.Clear();
            var len = protocol.getInt32();
            for (int i = 0; i < len; i++)
            {
                dic.Add(new KeyFixed( protocol.getString(), protocol.getRatio()));
            }
        }

        public void Serialize(ByteProtocol protocol)
        {
            protocol.push(dic.Count);
            foreach (var kv in dic)
            {
                protocol.push(kv.key);
                protocol.push(kv.value);
            }
            
        }
    }
}
