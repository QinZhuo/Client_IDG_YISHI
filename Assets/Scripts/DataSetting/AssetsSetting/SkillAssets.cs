using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
[CreateAssetMenu(fileName = "SkillAssets",menuName = "游戏资源配置文件/技能资源配置文件")]
public class SkillAssets : ScriptableObject {
    public SkillAssetsData[] skills;
    public SkillAssetsData  GetSkillAssets(SkillId id){
        for (int i = 0; i < skills.Length; i++)
        {
            if(skills[i].skillId==id){
                return skills[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class SkillAssetsData{
    public SkillId skillId;
    public string skillname;
    public string skillInfo;
    public Sprite uiIcon;
    public AnimationClip animation;
    public GameObject ItemPrefab;
}