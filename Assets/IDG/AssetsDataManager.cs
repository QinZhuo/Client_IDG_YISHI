using UnityEngine;
using System.Collections.Generic;
namespace IDG
{

    public class AssetsDataManager<T, DataT> : ScriptableObject, ISerializable where T : AssetsData<DataT>, new() where DataT : IDataClass, new()
    {
        public List<T> assets;
        public T Get(string Id)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i].data.Id == Id)
                {
                    return assets[i];
                }
            }
            Debug.LogError("资源[" + Id + "]不存在");
            return default(T);
        }
        public virtual string TableName()
        {
            return "表名";
        }

        [ContextMenu("Serialize")]
        public virtual void Serialize()
        {
            DataFile.SerializeToFile(TableName() + ".idgd", this);
        }
        [ContextMenu("Deserialize")]
        public virtual void Deserialize()
        {
            DataFile.Instance.DeserializeToData(TableName() + ".idgd", this);
        }
        public void Serialize(ByteProtocol protocol)
        {
            protocol.push(assets.Count);
            foreach (var assetsData in assets)
            {
                assetsData.data.Serialize(protocol);
            }
            foreach (var assetsData in assets)
            {
                assetsData.Serialize(protocol);
            }
        }

        public void Deserialize(ByteProtocol protocol)
        {
            var len = protocol.getInt32();
            Debug.LogError("len " + len);
            if (assets == null)
            {
                assets = new List<T>();
            }
            for (int i = 0; i < len; i++)
            {
                if (i >= assets.Count)
                {
                    var assetsData = new T();
                    assetsData.data = new DataT();
                    assetsData.data.Deserialize(protocol);
                    assets.Add(assetsData);
                }
                else
                {
                    assets[i].data.Deserialize(protocol);
                }
            }
            for (int i = 0; i < len; i++)
            {

                assets[i].Deserialize(protocol);

            }

        }
    }


    [System.Serializable]
    public class AssetsData<T> : ISerializable where T : IDataClass
    {
        public T data;

        public virtual void Deserialize(ByteProtocol protocol)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Serialize(ByteProtocol protocol)
        {
            throw new System.NotImplementedException();
        }
        public int ObjToId(Object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.GetInstanceID();
            }
        }
        public OT IdToObj<OT>(int instanceID) where OT : Object
        {
            if (instanceID == 0)
            {
                return null;
            }
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.InstanceIDToObject(instanceID) as OT;
#endif
            return null;
        }
    }
}