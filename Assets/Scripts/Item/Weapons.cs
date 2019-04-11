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

public class WeaponManager{
    public static Dictionary<WeaponId,Func<WeaponBase>> WeaponList;

    public static void Init(){
        WeaponList=new Dictionary<WeaponId, Func<WeaponBase>>();
        Add<Weapon_sword>(WeaponId.白刀);
        Add<Weapon_none>(WeaponId.无战斗);
        Add<Weapon_boxing>(WeaponId.拳击战斗);
    }
    public static WeaponBase GetWeapon(WeaponId WeaponId){
        if(WeaponList==null){
            Init();
        }
        return WeaponList[WeaponId]();
    }
    public static void Add<T>(WeaponId WeaponId)where T:WeaponBase,new()
    {
       WeaponList.Add(WeaponId,()=>{
           var Weapon=new T();
           Weapon.weaponId=WeaponId;
           return Weapon;
           });
    }
}
public class Weapon_sword:WeaponBase
{
    public override void Init(){
        base.Init();
        defalutSkillId=SkillId.swordAttack;
    }
}

public class Weapon_none:WeaponBase
{
    
}
public class Weapon_boxing:WeaponBase
{
    
}