using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class ItemView : NetObjectView<ItemData> { 
    public new SpriteRenderer renderer;
    protected override void OnStart(){
        
        if(data is SkillItem){
            var skillItem=data as SkillItem;
            var skillAssets=  GameViewAssetManager.instance.skillAssets.Get(skillItem.skillId.ToString());
         
            renderer.sprite=skillAssets.uiIcon;
      
        }else if(data is WeaponItem)
        {
            renderer.gameObject.SetActive(false);
            var weaponItem=data as WeaponItem;
            var weaponAssets= GameViewAssetManager.instance.weaponAssets.Get(weaponItem.weaponId.ToString());
            if(weaponAssets.ItemPrefab!=null){
                var obj= Instantiate(weaponAssets.ItemPrefab,transform.position,Quaternion.identity);
                obj.transform.SetParent(transform);
            }
        }
       
    }
}
public class SkillItem:ItemData{
    public SkillId skillId;
    public override void PickUp(NetData other){
        (other as PlayerData).skillList.AddSkill(SkillManager.GetSkill(skillId));
        base.PickUp(other);
          
    }
}

public class WeaponItem:ItemData{
    public WeaponId weaponId;
    public override void PickUp(NetData other){
       // (other as PlayerData).skillList.AddSkill(SkillManager.GetSkill(skillId));
       var player= (other as PlayerData);
        player.weaponSystem.AddWeapon(weaponId);
        base.PickUp(other);
    }
}
public class ItemData : NetData
{
    public NetData user;

    public override void Init(FSClient  client,int clientId=-1)
    {
        base.Init(client,clientId);
        rigibody.useCheckCallBack = true;
        isTrigger = true;

    }
    public override void Start()
    {
        base.Start();
        Shap = new BoxShap(new Fixed(1), new Fixed(1));
    }
    protected override void FrameUpdate()
    {
        
        
    }
    public override void OnPhysicsCheckStay(NetData other)
    {

        if (other.tag == "Player" && other != user)
        {
           UnityEngine.Debug.Log("Stay触发 "+other.name);
        
        }
    }
    public override void OnPhysicsCheckEnter(NetData other)
    {
        if (other.tag == "Player" && other != user)
        {
            UnityEngine.Debug.LogError("Enter触发Bullet！！！！");
            
            //var gun = new GunBase();
            //gun.Init(20, this);
            //(other as PlayerData).AddGun(gun);
            
            (other as PlayerData).items.AddDropList(this);
            //PickUp(other);
           

        }
    }
    public virtual void PickUp(NetData other){
        client.objectManager.Destory(this.view);
    }
    public override void OnPhysicsCheckExit(NetData other)
    {
        if (other.tag=="Player" && other != user)
        {
        //    UnityEngine.Debug.LogError("Exit触发Bullet！！！！");
            //Destory<Bullet>(this.show);
             (other as PlayerData).items.RemoveDropList(this);
        }
    }
    public override string PrefabPath()
    {
        return "Prefabs/ItemView";
    }
}
