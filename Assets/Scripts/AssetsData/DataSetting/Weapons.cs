using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using IDG;
public enum WeaponId
{
    白刀,
    无战斗,
    拳击战斗,
    F57,
    F41,
    丧尸,
}


public class WeaponManager : DataManager<WeaponManager, WeaponData>
{
    public override string TableName()
    {
        return "WeaponSetting";
    }
}

[System.Serializable]
public class WeaponData : IDataClass
{
    public WeaponId weaponId;
    public WeaponType weaponType;
    public SkillId defalutSkillId = SkillId.none;
    //public List<Fixed> fixedParams = new List<Fixed>();
    public FixedDictionary fixedParams = new FixedDictionary();
    public string Id
    {
        get
        {
            return weaponId.ToString();
        }
    }

    public void Serialize(ByteProtocol protocol)
    {
        protocol.push((int)weaponId);
        protocol.push((int)weaponType);
        protocol.push((int)defalutSkillId);
        fixedParams.Serialize(protocol);
    }
    public void Deserialize(ByteProtocol protocol)
    {
        weaponId = (WeaponId)protocol.getInt32();
        weaponType = (WeaponType)protocol.getInt32();
        defalutSkillId = (SkillId)protocol.getInt32();
        fixedParams.Deserialize(protocol);
    }
}