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
    枪
}
public class Gun : WeaponRuntime
{
    public Fixed laskFireTime;
    public Fixed time
    {
        get
        {
            return netData.client.inputCenter.Time;
        }
    }
    public Fixed shootRate
    {
        get
        {
            return weaponData.fixedParams[0];
        }
    }
    public Fixed rotRange
    {
        get
        {
            return weaponData.fixedParams[1];
        }
    }
    public void Fire()
    {
        if (time - laskFireTime > shootRate)
        {
            ShootBullet(netData.transform.Position, netData.transform.Rotation + netData.client.random.Range(rotRange.ToInt()));
            laskFireTime = time;
        }
    }

    protected void ShootBullet(Fixed2 position, Fixed rotation)
    {
        Bullet bullet = new Bullet();
        bullet.user = netData;
        bullet.Init(netData.client);
        bullet.Reset(position, rotation);
        netData.client.objectManager.Instantiate(bullet);
    }
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
       weapon.InitNetData(netData);
       changeWeapon(weapon.weaponId);
       (netData as PlayerData).skillList.AddSkill(weapon.defalutSkillId);
        //UnityEngine.Debug.LogError("add "+Weapon.WeaponId);
    }
    public override void Update()
    {
       
    }

}

