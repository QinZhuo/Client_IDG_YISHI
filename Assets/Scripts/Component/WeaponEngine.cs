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
            return weaponData.fixedParams["shootRate"];
        }
    }
    public Fixed rotRange
    {
        get
        {
            return weaponData.fixedParams["rotRange"];
        }
    }
    public Fixed mainDamage
    {
        get
        {
            return weaponData.fixedParams["mainDamage"];
        }
    }
    public void Fire(SkillNode skinode,ISkillNodeRun run)
    {
        if (time - laskFireTime > shootRate)
        {
            ShootBullet(netData.transform.Position, netData.transform.Rotation + netData.client.random.Range(-rotRange.ToInt(), rotRange.ToInt()), skinode, run);
            laskFireTime = time;
        }
    }

    protected void ShootBullet(Fixed2 position, Fixed rotation,SkillNode skillnode,ISkillNodeRun run)
    {
        Bullet bullet = new Bullet();
      
        bullet.skillNode = skillnode;
        bullet.skill = run.skill;
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

    protected WeaponRuntime()
    {
    }
    public static WeaponRuntime Parse(WeaponData weaponData)
    {
        WeaponRuntime runtime;
        if (weaponData.weaponType == WeaponType.枪)
        {
            runtime = new Gun();
        }
        else
        {
            runtime = new WeaponRuntime();
        }
        runtime.weaponData = weaponData;
        return runtime;
    }
    public SkillId defalutSkillId { get { return weaponData.defalutSkillId; } }
    public WeaponStatus status=WeaponStatus.none;
    public WeaponData weaponData;

}

public class WeaponEngine:ComponentBase 
{
    protected List<WeaponRuntime> weaponList;
    public Action<WeaponId> changeWeapon;
    protected int currentId;
    public T curWeapon<T>() where T: WeaponRuntime
    {
        
        if(currentId < weaponList.Count)
        {
            return weaponList[currentId] as T;
        }
        else
        {
            return null;
        }
       
        
    }
    public override void Init()
    {
        weaponList=new List<WeaponRuntime>();
        currentId =-1;
    }
    public void AddWeapon(WeaponId weaponId){
        AddWeapon(WeaponRuntime.Parse(WeaponManager.Get(weaponId.ToString())));
    }
    public void AddWeapon(WeaponRuntime weapon)
    {
       weaponList.Add(weapon);
       currentId++;
       weapon.InitNetData(netData);
       changeWeapon(weapon.weaponId);
       (netData as PlayerData).skillList.AddSkill(weapon.defalutSkillId);
        //UnityEngine.Debug.LogError("add "+Weapon.WeaponId);
    }
    public override void Update()
    {
       
    }

}

