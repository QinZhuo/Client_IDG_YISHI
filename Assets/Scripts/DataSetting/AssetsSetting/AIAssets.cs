using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
[CreateAssetMenu(fileName = "AIAssets",menuName = "游戏资源配置文件/AI配置资源")]
public class AIAssets : ScriptableObject,ISerializable {
    
    public AIAssetsData[] aIAssets;
    [ContextMenu("Serialize")]
    public void Serialize(){
        DataFile.SerializeToFile("AIsSetting.iai",this);
    }
    public void Serialize(ByteProtocol protocol){
          protocol.push(aIAssets.Length);
        foreach (var ai in aIAssets)
        {
            ai.data.Serialize(protocol);
        }
    }
    
    public void Deserialize(ByteProtocol protocol){
        
    }
}
[System.Serializable]
public class AIAssetsData{
    public string name{
        get{
            return data.name;
        }
    }
    public AiData data;
}