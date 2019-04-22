using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
using IDG ;
public class BulletShow : NetObjectView<Bullet> {
    public static int id=0;


  
 

    
    // Update is called once per frame
    //   void Update () {

    //}
}
public class Bullet : NetData
{
    public NetData user;
    public Fixed startTime;
    public override void Init(FSClient  client,int clientId=-1)
    {
        base.Init(client,clientId);
        rigibody.useCheckCallBack = true;
        isTrigger = true;
        startTime = client.inputCenter.Time;
    }
    public override void Start()
    {
        Shap = new BoxShap(new Fixed(2), new Fixed(0.3f));
        base.Start();
    }
    protected override void FrameUpdate()
    {
        
        transform.Position += transform.forward * (deltaTime * 10f);
        // Debug.Log("bullet" + Position);
        if (client.inputCenter.Time - startTime > 3)
        {
            client.objectManager.Destory(this.view);
        }
    }
    public override void OnPhysicsCheckStay(NetData other)
    {

        if (other.tag == "Player" && other != user)
        {
            UnityEngine.Debug.Log("Stay触发Bullet！！！！");
            //Destory<Bullet>(this.show);
        }
    }
    public override void OnPhysicsCheckEnter(NetData other)
    {
        if (other is HealthData && other != user)
        {
            UnityEngine.Debug.Log("Enter触发Bullet！！！！");
            
            (other as HealthData).GetHurt(new Fixed(10));
        }
        if(!(other is Bullet)){
             client.objectManager.Destory(this.view);
              UnityEngine.Debug.Log("destoy Bullet");
        }
    }
    public override void OnPhysicsCheckExit(NetData other)
    {
        if (other.tag=="Player" && other != user)
        {
            UnityEngine.Debug.Log("Exit触发Bullet！！！！");
            //Destory<Bullet>(this.show);
        }
    }
    public override string PrefabPath()
    {
        return "Prefabs/Bullet";
    }
}
