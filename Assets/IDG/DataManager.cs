using System;
using System.Collections.Generic;

namespace IDG
{

    public class DataManager<TManager, T> : ISerializable where T : IDataClass, new() where TManager : DataManager<TManager, T>, new()
    {
        public virtual string TableName()
        {
            return "表名";
        }
        protected static TManager instance;

        public static Dictionary<string, T> table;
        public static void Init()
        {
            instance = new TManager();
            instance.LoadTable();
        }
        public void LoadTable()
        {
            Debug.Log("初始化表[" + TableName() + "]中 ...");
            table = new Dictionary<string, T>();
            DataFile.Instance.DeserializeToData(TableName() + ".idgd", this);
        }

        public virtual void Serialize(ByteProtocol protocol)
        {
            throw new NotImplementedException();
        }

        public virtual void Deserialize(ByteProtocol protocol)
        {
            table.Clear();
            var len = protocol.getInt32();
            for (int i = 0; i < len; i++)
            {
                var data = new T();
                data.Deserialize(protocol);

                table.Add(data.Id, data);

            }
            Debug.Log("AI初始化完毕 AI数" + table.Count);
        }

        public static T Get(string Id)
        {

            if (table.ContainsKey(Id))
            {
                return table[Id];
            }
            else
            {
                Debug.LogError("不存在" + instance.TableName() + "【" + Id + "】");
                return default(T);
            }

        }
    }
    public interface IDataClass : ISerializable
    {
        string Id
        {
            get;
        }
    }
}