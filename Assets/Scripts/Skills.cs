using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
using IDG.FSClient;

class SkillShoots:SkillBase
{
    public override void Init()
    {
        key = KeyNum.Skill1;
        time = new Fixed(0.7f);
        timer = new Fixed(0);
    }
   
    PlayerData player=null;
    public override void StayUse()
    {
         if(data is PlayerData){
            player=data as PlayerData;
            if(player==null&&player.moveCtrRot){
           
                player.moveCtrRot=false;
               
            }
        }
             
       if(player!=null){
            var rot=data.Input.GetJoyStickDirection(key);
        player.transform.Rotation=rot.ToRotation();
       }
    }
    public override void UseOver()
    {
        base.UseOver();
        if(player!=null){
            player.moveCtrRot=true;
            player=null;
        }
       // UnityEngine.Debug.LogError("bulletUse" + data.Input.GetJoyStickDirection(key).ToRotation());
        for (int i = -30; i <= 30; i += 5)
        {
            ShootBullet(data.transform.Position+data.transform.forward,data.Input.GetJoyStickDirection(key).ToRotation() + i);
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
        UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
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
        gun.Init(20, data);
    
    }
    PlayerData player;
    
    public override void UseOver()
    {
        base.UseOver();
        if(player!=null){
            player.moveCtrRot=true;
            player=null;
        }
    
    }
    public override void StayUse()
    {
        var rot=data.Input.GetJoyStickDirection(key);
        if(gun!=null) gun.Fire(data,rot.ToRotation() );  
        if(player==null&& data is PlayerData){
            player=data as PlayerData;
             if(player.moveCtrRot){
                player.moveCtrRot=false;
              
            }
            
        }
       if(player!=null){
        player.transform.Rotation=rot.ToRotation();
       }
    }

    
    protected void ShootBullet(Fixed2 position, Fixed2 direction)
    {
        UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
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
