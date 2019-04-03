using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
[CreateAssetMenu(fileName = "WeaponAssets",menuName = "游戏资源配置文件/武器资源配置文件")]
public class WeaponAssets : ScriptableObject {
    public WeaponAssetsData[] Weapons;
    public WeaponAssetsData  GetWeaponAssets(WeaponId id){
        for (int i = 0; i < Weapons.Length; i++)
        {
            if(Weapons[i].WeaponId==id){
                return Weapons[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class WeaponAssetsData{
    public WeaponId WeaponId;
    public string Weaponname;
    public string WeaponInfo;
    public Sprite uiIcon;
    public AnimationClip idleAnim;
    public AnimationClip animation;
    public GameObject ItemPrefab;
}