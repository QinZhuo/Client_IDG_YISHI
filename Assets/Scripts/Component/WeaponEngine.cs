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
public class WeaponRuntime:ComponentBase
{
    public WeaponId weaponId
    {
        get
        {
            return weaponData.weaponId;
        }
    }
    public WeaponType weaponType;
    public SkillId defalutSkillId { get { return weaponData.defalutSkillId; } }
    public WeaponStatus status=WeaponStatus.none;
    public WeaponData weaponData;

}

public class WeaponEngine:ComponentBase 
{
    public List<WeaponRuntime> weaponList;
    public Action<WeaponId> changeWeapon;
    public int currentId;
    public override void Init()
    {
        weaponList=new List<WeaponRuntime>();
    }
    public void AddWeapon(WeaponId weaponId){
        var weapon = new WeaponRuntime();
        weapon.weaponData = WeaponManager.Get(weaponId.ToString());
        AddWeapon(weapon);
    }
    public void AddWeapon(WeaponRuntime weapon)
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

