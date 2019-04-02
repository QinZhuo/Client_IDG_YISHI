using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class ZombieShow : NetObjectView<ZombieData> {
    
}


public class ZombieData : HealthData
{
    // protected GunBase gun;
    public ShapBase findShap;
    public Fixed CheckTimer=Fixed.Zero;
    public NetData player;
    public override void Start()
    {
        this.tag = "Zombie";
        Shap = new CircleShap(new Fixed(0.5f), 8);
        findShap = new CircleShap(5.ToFixed(), 10);
        // gun = new GunBase();
         rigibody.useCheck=true;
         base.Start();
        // gun.Init(2, this);
    }
    protected override void FrameUpdate()
    {
        if( CheckTimer<=0){
            if(player==null){
                var others = client.physics.OverlapShap(findShap, transform.Position);
                foreach (var other in others)
                {
                    if(other is PlayerData){
                        player=other;
                        Debug.Log("fined Player");
                        break;
                    }
                }
            }
            CheckTimer=20.ToFixed();
        }else
        {
            CheckTimer-= FSClient.deltaTime;
        }
        if(player!=null){
            transform.LookAt(player.transform.Position);
        }
        

       

    
      
        
        // Debug.Log("FrameUpdate"+ Position+":"+ Input.GetJoyStickDirection(FrameKey.MoveKey));
    }
    protected override void Die()
    {
        base.Die();
        client.objectManager.Destory(view);
    }
    public override string PrefabPath()
    {
        return "Prefabs/Zombie";
    }
}

