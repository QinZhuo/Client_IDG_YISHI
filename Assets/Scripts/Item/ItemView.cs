using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class ItemView : NetObjectView<ItemData> { 
    public new SpriteRenderer renderer;
    protected override void OnStart(){
        
        if(data is SkillItem){
            var skillItem=data as SkillItem;
            var skillAssets=  GameViewAssetManager.instance.skillAssets.GetSkillAssets(skillItem.skillId);
            renderer.sprite=skillAssets.uiIcon;
      
        }else if(data is WeaponItem)
        {
            renderer.gameObject.SetActive(false);
            var weaponItem=data as WeaponItem;
            var weaponAssets= GameViewAssetManager.instance.weaponAssets.GetWeaponAssets(weaponItem.weaponId);
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
       
          
    }
}

public class WeaponItem:ItemData{
    public WeaponId weaponId;
    public override void PickUp(NetData other){
       // (other as PlayerData).skillList.AddSkill(SkillManager.GetSkill(skillId));
       var player= (other as PlayerData);
       var weapon=WeaponManager.GetWeapon(weaponId);
        player.weaponSystem.AddWeapon(weapon);
    }
}
public class ItemData : NetData
{
    public NetData user;

    public override void Init(FSClient  client)
    {
        base.Init(client);
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
            UnityEngine.Debug.Log("Stay触发Bullet！！！！");
        
        }
    }
    public override void OnPhysicsCheckEnter(NetData other)
    {
        if (other.tag == "Player" && other != user)
        {
            UnityEngine.Debug.Log("Enter触发Bullet！！！！");
            client.objectManager.Destory(this.view);
            //var gun = new GunBase();
            //gun.Init(20, this);
            //(other as PlayerData).AddGun(gun);
            PickUp(other);
           

        }
    }
    public virtual void PickUp(NetData other){

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
        return "Prefabs/ItemView";
    }
}
