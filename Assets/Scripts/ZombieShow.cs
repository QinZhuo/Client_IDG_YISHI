using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class ZombieShow : PlayerShow{
    
}


public class ZombieData : PlayerData
{
    // protected GunBase gun;
   
    //public AiEngine ai;
    public override void Start()
    {
        base.Start();
        this.tag = "Zombie";
        if(ai!=null){
            ai.emenyTag="Player";
        }
        else
        {
            //  UnityEngine.Debug.LogError("ai is Null");
            ai=new AiEngine();
            ai.aiName="AI_test";
            ai=AddCommponent<AiEngine>(ai);
            ai.emenyTag="Player";
        }
       
        // gun = new GunBase();
        // rigibody.useCheck=true;
        // var ai=new AiEngine();
        // ai.aiName="AI_test";
        // ai.aiActionFunc=AiAction;
        // ai=AddCommponent<AiEngine>(ai);
       
        
       
        // gun.Init(2, this);
    }
    // public AINodeStatus AiAction(AINode node){
    //     Debug.LogError("AI_"+(ActionType)node.intType);
    //     return AINodeStatus.Success;
    // }
    // protected override void FrameUpdate()
    // {
    //     if( CheckTimer<=0){
    //         if(player==null){
    //             var others = client.physics.OverlapShap(findShap, transform.Position);
    //             foreach (var other in others)
    //             {
    //                 if(other is PlayerData){
    //                     player=other;
    //                     Debug.Log("fined Player");
    //                     break;
    //                 }
    //             }
    //         }
    //         CheckTimer=20.ToFixed();
    //     }else
    //     {
    //         CheckTimer-= FSClient.deltaTime;
    //     }
    //     if(player!=null){
    //         transform.LookAt(player.transform.Position);
    //     }
        

       

    
      
        
    //     // Debug.Log("FrameUpdate"+ Position+":"+ Input.GetJoyStickDirection(FrameKey.MoveKey));
    // }
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

