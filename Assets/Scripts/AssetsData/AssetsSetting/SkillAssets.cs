using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;

[CreateAssetMenu(fileName = "SkillAssets",menuName = "游戏资源配置文件/技能资源配置文件")]
public class SkillAssets : AssetsDataManager<SkillAssetsData,SkillData>
{

    public override string TableName()
    {
        return "SkillSetting";
    }
    public void Compute(){
        for (int i = 0; i < assets.Count; i++)
        {
            assets[i].data.animTime=assets[i].useOverAnim!=null?( assets[i].useOverAnim.length.ToFixed()*(1 / assets[i].useOverAnimSspeed)):0.ToFixed();
        }
    }

    [ContextMenu("Serialize")]
    public override void Serialize()
    {
        Compute();
        base.Serialize();
    }
    [ContextMenu("Deserialize")]
    public override void Deserialize()
    {
        base.Deserialize();
    }
}

[System.Serializable]
public class SkillAssetsData:AssetsData<SkillData>{
    public string skillname;
    public string skillInfo;
    public Sprite uiIcon;
    public AnimationClip useOverAnim;
    public float useOverAnimSspeed=1;
    public GameObject ItemPrefab;
   
    public override void Serialize(ByteProtocol protocol)
    {
        protocol.push(skillname);
        protocol.push(skillInfo);
        protocol.push(ObjToId(uiIcon));
        protocol.push(ObjToId(useOverAnim));
        protocol.push(useOverAnimSspeed.ToFixed());
        protocol.push(ObjToId(ItemPrefab));
        
    }
    public override void Deserialize(ByteProtocol protocol)
    {
        skillname = protocol.getString();
        skillInfo = protocol.getString();
        uiIcon = IdToObj<Sprite>(protocol.getInt32());
        useOverAnim = IdToObj<AnimationClip>(protocol.getInt32());
        useOverAnimSspeed= protocol.getRatio().ToFloat();
        ItemPrefab = IdToObj<GameObject>(protocol.getInt32());
       
    }
}