using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
[CreateAssetMenu(fileName = "WeaponAssets",menuName = "游戏资源配置文件/武器资源配置文件")]
public class WeaponAssets : AssetsDataManager<WeaponAssetsData,WeaponData> {
    public override string TableName()
    {
        return "WeaponSetting";
    }
    [ContextMenu("Serialize")]
    public override void Serialize()
    {
        base.Serialize();
    }
    [ContextMenu("Deserialize")]
    public override void Deserialize()
    {
        base.Deserialize();
    }
}

[System.Serializable]
public class WeaponAssetsData:AssetsData<WeaponData>{
    public string Weaponname;
    public string WeaponInfo;
    public Sprite uiIcon;
    public AnimationClip idleAnim;
    public AnimationClip animation;
    public GameObject ItemPrefab;
    public override void Serialize(ByteProtocol protocol)
    {
        protocol.push(Weaponname);
        protocol.push(WeaponInfo);
        protocol.push(ObjToId(uiIcon));
        protocol.push(ObjToId(animation));
        protocol.push(ObjToId(ItemPrefab));
    }
    public override void Deserialize(ByteProtocol protocol)
    {
        Weaponname = protocol.getString();
        WeaponInfo = protocol.getString();
        uiIcon = IdToObj<Sprite>(protocol.getInt32());
        idleAnim = IdToObj<AnimationClip>(protocol.getInt32());
        ItemPrefab = IdToObj<GameObject>(protocol.getInt32());
    }
}