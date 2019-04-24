using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;


[CreateAssetMenu(fileName = "AIAssets",menuName = "游戏资源配置文件/AI配置资源")]
public class AIAssets : AssetsDataManager<AIAssetsData,AiData> {

    public override string TableName()
    {
        return "AiSetting";
    }
    [ContextMenu("Serialize")]
    public override void Serialize(){
        base.Serialize();
    }
    [ContextMenu("Deserialize")]
    public override void Deserialize()
    {
        base.Deserialize();
    }
}
[System.Serializable]
public class AIAssetsData:AssetsData<AiData>{
    
}