using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
public enum SkillId
{
    none=-1,
    拳击左直=0,
    拳击右直=1,
    拳击左上勾=2,
    拳击右上勾=3,
    挥砍=4,
}
public class SkillManager{

    public static Dictionary<SkillId,Func<SkillBase>> skillList;

    public static void Init(){
        skillList=new Dictionary<SkillId, Func<SkillBase>>();
        Add<Skill_Box>(SkillId.拳击右上勾);
        Add<Skill_Box>(SkillId.拳击左上勾);
        Add<Skill_Box>(SkillId.拳击右直);
        Add<Skill_Box>(SkillId.拳击左直);
        Add<Skill_Box>(SkillId.挥砍);
    }
    public static SkillBase GetSkill(SkillId skillId){
        if(skillList==null){
            Init();
        }
        return skillList[skillId]();
    }
    public static void Add<T>(SkillId skillId)where T:SkillBase,new()
    {
       skillList.Add(skillId,()=>{
           var skill=new T();
           skill.skillId=skillId;
           return skill;
           });
    }
}

class Skill_CloseCombat:SkillBase
{

    Fixed2 rot;
    public override void Init()
    {
        key = KeyNum.Skill1;
        time = new Fixed(0.7f);
        timer = new Fixed(0);
        overTime=0.5f.ToFixed();
    }
    public override void StayUse()
    {       
       if(player!=null){
            rot=data.Input.GetJoyStickDirection(key);
            if(rot.magnitude<0.1f){
                rot=data.transform.forward;
            }
            player.transform.Rotation=rot.ToRotation();
       }
    }
    public override void UseOver()
    {
        base.UseOver();
        
    }
  
}
class Skill_Box:Skill_CloseCombat{
    public override void Init(){
        base.Init();
    }
}
class SkillShoots:SkillBase
{

    Fixed2 rot;
    public override void Init()
    {
        key = KeyNum.Skill1;
        
        time = new Fixed(0.7f);
        timer = new Fixed(0);
        overTime=0.5f.ToFixed();
     
    }
  
    public override void StayUse()
    {
       
             
       if(player!=null){
            rot=data.Input.GetJoyStickDirection(key);
            if(rot.magnitude<0.1f){
                rot=data.transform.forward;
            }
            player.transform.Rotation=rot.ToRotation();
       }
    }
    public override void UseOver()
    {
        base.UseOver();
       
       // UnityEngine.Debug.LogError("bulletUse" + data.Input.GetJoyStickDirection(key).ToRotation());
        for (int i = -30; i <= 30; i += 5)
        {
            ShootBullet(data.transform.Position+data.transform.forward,rot.ToRotation() + i);
        }
    }
    protected void ShootBullet(Fixed2 position, Fixed rotation)
    {
        Bullet bullet = new Bullet();
        bullet.user = data;
        bullet.Init(data.client);
        bullet.Reset( position, rotation);
        data.client.objectManager.Instantiate(bullet);
      //  UnityEngine.Debug.LogError("bullet" + rotation);
    }
}


class Attack_02:SkillBase
{

    Fixed2 rot;
    public override void Init()
    {
        key = KeyNum.Skill1;

        time = new Fixed(0.7f);
        timer = new Fixed(0);
        overTime=0.5f.ToFixed();
     
    }
  
    public override void StayUse()
    {
       
             
       if(player!=null){
            rot=data.Input.GetJoyStickDirection(key);
            if(rot.magnitude<0.1f){
                rot=data.transform.forward;
            }
            player.transform.Rotation=rot.ToRotation();
       }
    }
    public override void UseOver()
    {
        base.UseOver();
       
       // UnityEngine.Debug.LogError("bulletUse" + data.Input.GetJoyStickDirection(key).ToRotation());
        for (int i = -30; i <= 30; i += 5)
        {
            ShootBullet(data.transform.Position+data.transform.forward,rot.ToRotation() + i);
        }
    }
    protected void ShootBullet(Fixed2 position, Fixed rotation)
    {
        Bullet bullet = new Bullet();
        bullet.user = data;
        bullet.Init(data.client);
        bullet.Reset( position, rotation);
        data.client.objectManager.Instantiate(bullet);
      //  UnityEngine.Debug.LogError("bullet" + rotation);
    }
}
class SkillRay:SkillBase{
    GunBase gun;
    RayShap ray;
    public override void Init()
    {
        key = KeyNum.Skill1;
        time = new Fixed(0.7f);
        timer = new Fixed(0);
        gun = new GunBase();
        ray = new RayShap(Fixed2.zero);
        gun.Init(20, data);
    
    }
   
    
   
    public override void StayUse()
    {
        var rot=data.Input.GetJoyStickDirection(key);
        //if(gun!=null) gun.Fire(data,rot.ToRotation() );  
        ShootBullet(data.transform.Position, rot);
     
    }
    protected void ShootBullet(Fixed2 position, Fixed2 direction)
    {
       // UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
        var others = data.client.physics.OverlapShap(ray.ResetDirection(position, direction, new Fixed(10)));
        foreach (var other in others)
        {
            if (other != data)
            {
                if(other is HealthData)
                {
                    (other as HealthData).GetHurt(new Fixed(10));
                }
            }
        }
        //Bullet bullet = new Bullet();
        //bullet.user = data;
        //bullet.Init(data .client);
        //bullet.Reset(position, rotation);
        //data.client.objectManager.Instantiate(bullet);
     
    }
}
class SkillGun:SkillBase
{
    GunBase gun;
    RayShap ray;
    public override void Init()
    {
        key = KeyNum.Skill1;
        time = new Fixed(0.7f);
        timer = new Fixed(0);
        gun = new GunBase();
        ray = new RayShap(Fixed2.zero);
        overTime=1.ToFixed();
        gun.Init(20, data);
    
    }
    public override void UseOver()
    {
        base.UseOver();
     
    
    }
    public override void StayUse()
    {
        var rot=data.Input.GetJoyStickDirection(key);
        if(gun!=null) gun.Fire(data,rot.ToRotation() );  
     
       if(player!=null){
        player.transform.Rotation=rot.ToRotation();
       }
    }

    
    protected void ShootBullet(Fixed2 position, Fixed2 direction)
    {
       // UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
        var others = data.client.physics.OverlapShap(ray.ResetDirection(position, direction, new Fixed(10)));
        foreach (var other in others)
        {
            if (other != data)
            {
                if(other is HealthData)
                {
                    (other as HealthData).GetHurt(new Fixed(10));
                }
            }
        }
        //Bullet bullet = new Bullet();
        //bullet.user = data;
        //bullet.Init(data .client);
        //bullet.Reset(position, rotation);
        //data.client.objectManager.Instantiate(bullet);
     
    }
}
