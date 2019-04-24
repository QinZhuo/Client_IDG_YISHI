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

    }
    public void Deserialize(ByteProtocol protocol)
    {
        weaponId = (WeaponId)protocol.getInt32();
    }
}