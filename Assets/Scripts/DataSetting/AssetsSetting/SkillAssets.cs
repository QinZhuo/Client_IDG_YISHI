using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
[CreateAssetMenu(fileName = "SkillAssets",menuName = "游戏资源配置文件/技能资源配置文件")]
public class SkillAssets : ScriptableObject,ISerializable {
    public SkillAssetsData[] skills;
    public SkillAssetsData  GetSkillAssets(SkillId id){
        for (int i = 0; i < skills.Length; i++)
        {
            if(skills[i].skillId==id){
                return skills[i];
            }
        }
        Debug.LogError("技能【"+id+"】不存在");
        return null;
    }
    public void Compute(){
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].skillData.coolDownTime= 1.ToFixed();
            skills[i].skillData.animTime= skills[i].animation.length.ToFixed();
        }
    }
    [ContextMenu("Serialize")]
    public void Serialize(){
         Compute();
        DataFile.SerializeToFile("SkillsSetting.isk",this);
    }
    public void Serialize(ByteProtocol protocol){
        protocol.push(skills.Length);
        foreach (var skill in skills)
        {
            skill.skillData.Serialize(protocol);
        }
    }
    
    public void Deserialize(ByteProtocol protocol){
        
    }
}
[System.Serializable]
public class SkillAssetsData{
    public SkillId skillId{
        get{
            return skillData.skillId;
        }
    }
    public string skillname;
    public string skillInfo;
    public Sprite uiIcon;
    public AnimationClip animation;
    public GameObject ItemPrefab;
    public SkillData skillData;
   
}