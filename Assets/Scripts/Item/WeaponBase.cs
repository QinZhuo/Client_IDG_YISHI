using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
public enum WeaponStatus
{
    none,
    hand,
    bag,
}
public enum WeaponType
{
    空手,
    刀,
    剑,
}
public class WeaponBase:ComponentBase
{
    public WeaponId weaponId; 
    public WeaponType weaponType;
    public SkillId defalutSkillId=SkillId.none;
    public WeaponStatus status=WeaponStatus.none;


}

public class WeaponSystem:ComponentBase 
{
    public List<WeaponBase> weaponList;
    public Action<WeaponId> changeWeapon;
    public int currentId;
    public override void Init()
    {
        weaponList=new List<WeaponBase>();
    }
    public void AddWeapon(WeaponId weaponId){
        AddWeapon(WeaponManager.GetWeapon(weaponId));
    }
    public void AddWeapon(WeaponBase weapon)
    {
       weaponList.Add(weapon);
       weapon.InitNetData(data);
       changeWeapon(weapon.weaponId);
       (data as PlayerData).skillList.AddSkill(weapon.defalutSkillId);
        //UnityEngine.Debug.LogError("add "+Weapon.WeaponId);
    }
    public override void Update()
    {
       
    }

}

