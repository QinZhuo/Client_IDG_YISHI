using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
public enum SkillId:Int32
{
    none=-1,
    拳击左直=100,
    拳击右直=101,
    拳击左上勾=102,
    拳击右上勾=103,
    swordAttack=104,
}
public class SkillManager:ISerializable{
    public static SkillManager instance;
    public static Dictionary<SkillId,Func<SkillBase>> skillList;
    public static void Init(){
        instance=new SkillManager();
        instance.LoadTable();
    }
    public void LoadTable(){
         UnityEngine.Debug.Log("初始化技能中 ...");
        skillList=new Dictionary<SkillId, Func<SkillBase>>();
        DataFile.Instance.DeserializeToData("SkillsSetting.isk",this);
        // SkillData skillData=new SkillData();
        // skillData.animTime=1.ToFixed();
        // skillData.coolDownTime=1.ToFixed();
        // skillData.key=KeyNum.Skill1;
        // var node=new SkillNode();
        // node.trigger=SkillTrigger.结束;
        // node.type=NodeType.RotatePlayer;
        // skillData.nextNodes.Add(node);
        // Add(SkillId.拳击右上勾,skillData);
        // Add(SkillId.拳击左上勾,skillData);
        // Add(SkillId.拳击右直,skillData);
        // Add(SkillId.拳击左直,skillData);
        // Add(SkillId.swordAttack,skillData);
     
    }
    
    public  void Serialize(ByteProtocol protocol){
       
        
    }
    public void Deserialize(ByteProtocol protocol){
        skillList.Clear();
       var len=protocol.getInt32();
        for (int i = 0; i < len; i++)
        {
            var skilldata =new SkillData();
            skilldata.Deserialize(protocol);
            UnityEngine.Debug.Log("初始化技能 "+skilldata.skillId);
            Add(skilldata.skillId,skilldata);
            
        }
        UnityEngine.Debug.Log("技能初始化完毕 技能数"+skillList.Count);
    }
    public static SkillBase GetSkill(SkillId skillId){
      
        return skillList[skillId]();
    }
    public static void Add(SkillId skillId,SkillData skillData)
    {
        if(skillList.ContainsKey(skillId)){
            UnityEngine.Debug.LogError("重复的技能"+skillData.skillId);
            return;
        }
       skillList.Add(skillId,()=>{
           var skill=new SkillBase();
           skill.skillId=skillId;
           skill.skillData=skillData;
           return skill;
           });
    }
}

public enum SkillTrigger:Int32
{
    root=0,
    PressStart=1,
    PressStay=2,
    PressOver=3,
    AnimOver=4,
    Time=5,
    Check=6,
}
public enum SkillNodeType:Int32
{
    Root=0,
    RotatePlayer=1,
    MoveCtr=2,
    WaitTime=3,
    LoopTime=4,
    CreatCheck=5,
    Damage=6,
}
[System.Serializable]
public class SkillData:SkillNode,ISerializable{
    public SkillId skillId;
    public Fixed coolDownTime;
    public Fixed animTime;
    public KeyNum key;

    public override void Serialize(ByteProtocol protocol){
        protocol.push((Int32)skillId);
    //    UnityEngine.Debug.LogError("push skillId" +(Int32)skillId);
        protocol.push(coolDownTime);
        protocol.push(animTime);
        protocol.push((byte)key);
        base.Serialize(protocol);
        
    }
    public override void Deserialize(ByteProtocol protocol){
        skillId=(SkillId)protocol.getInt32();
    //    UnityEngine.Debug.LogError("get skillId" +(Int32)skillId);
        coolDownTime=protocol.getRatio();
        animTime=protocol.getRatio();
        key=(KeyNum)protocol.getByte();
        base.Deserialize(protocol);
    }
}
[System.Serializable]
public class SkillNode:ITreeNode<SkillNode>,ISerializable{
    public SkillTrigger trigger=SkillTrigger.root;
    public SkillNodeType type=SkillNodeType.Root;
    public List<SkillNode> nextNodes=new List<SkillNode>();
    public List<bool> boolParams=new List<bool>(); 
    public List<int> intParams=new List<int>(); 
    public List<SkillNode> childNodes{
        get{
            return nextNodes;
        }
    }
    public List<SkillNode> GetNodes(SkillTrigger trigger){
        List<SkillNode> nodes=new List<SkillNode>();
        foreach (var node in nextNodes)
        {
            if(node.trigger==trigger){
                nodes.Add(node);
            }
        }
        return nodes;
    }
    public virtual void Serialize(ByteProtocol protocol){
        protocol.push((Int32)trigger);
        protocol.push((Int32)type);
       // UnityEngine.Debug.LogError("push type" +(Int32)type);
        protocol.push(nextNodes.Count);
      //  UnityEngine.Debug.LogError("nextNode Count" +nextNodes.Count);
        foreach (var node in nextNodes)
        {
            node.Serialize(protocol);
        }
        protocol.push(boolParams.Count);
        foreach (var b in boolParams)
        {
            protocol.push(b);
        }
        protocol.push(intParams.Count);
        foreach (var b in intParams)
        {
            protocol.push(b);
        }
    }
    public virtual void Deserialize(ByteProtocol protocol){
        trigger=(SkillTrigger)protocol.getInt32();
        type=(SkillNodeType)protocol.getInt32();
      //  UnityEngine.Debug.LogError("get type" +(Int32)type);
        var len=protocol.getInt32();
       //  UnityEngine.Debug.LogError("get nextNode Count" +len);
        nextNodes.Clear();
        for (int i = 0; i < len; i++)
        {
            var node=new SkillNode();
            node.Deserialize(protocol);
            nextNodes.Add(node);
        }
        len=protocol.getInt32();
        boolParams.Clear();
        for (int i = 0; i < len; i++)
        {
            boolParams.Add(protocol.getBoolean());
        }
        len=protocol.getInt32();
        intParams.Clear();
        for (int i = 0; i < len; i++)
        {
            intParams.Add(protocol.getInt32());
        }
    }
}

// class Skill_CloseCombat:SkillBase
// {

//     Fixed2 rot;
//     public override void Init()
//     {
//         key = KeyNum.Skill1;
//         time = new Fixed(0.7f);
//         timer = new Fixed(0);
//         overTime=0.5f.ToFixed();
//     }
//     public override void StayUse()
//     {       
//        if(player!=null){
//             rot=data.Input.GetJoyStickDirection(key);
//             if(rot.magnitude<0.1f){
//                 rot=data.transform.forward;
//             }
//             player.transform.Rotation=rot.ToRotation();
//        }
//     }
//     public override void UseOver()
//     {
//         base.UseOver();
        
//     }
  
// }
// class Skill_Box:Skill_CloseCombat{
//     public override void Init(){
//         base.Init();
//     }
// }
// class SkillShoots:SkillBase
// {

//     Fixed2 rot;
//     public override void Init()
//     {
//         key = KeyNum.Skill1;
        
//         time = new Fixed(0.7f);
//         timer = new Fixed(0);
//         overTime=0.5f.ToFixed();
     
//     }
  
//     public override void StayUse()
//     {
       
             
//        if(player!=null){
//             rot=data.Input.GetJoyStickDirection(key);
//             if(rot.magnitude<0.1f){
//                 rot=data.transform.forward;
//             }
//             player.transform.Rotation=rot.ToRotation();
//        }
//     }
//     public override void UseOver()
//     {
//         base.UseOver();
       
//        // UnityEngine.Debug.LogError("bulletUse" + data.Input.GetJoyStickDirection(key).ToRotation());
//         for (int i = -30; i <= 30; i += 5)
//         {
//             ShootBullet(data.transform.Position+data.transform.forward,rot.ToRotation() + i);
//         }
//     }
//     protected void ShootBullet(Fixed2 position, Fixed rotation)
//     {
//         Bullet bullet = new Bullet();
//         bullet.user = data;
//         bullet.Init(data.client);
//         bullet.Reset( position, rotation);
//         data.client.objectManager.Instantiate(bullet);
//       //  UnityEngine.Debug.LogError("bullet" + rotation);
//     }
// }


// class Attack_02:SkillBase
// {

//     Fixed2 rot;
//     public override void Init()
//     {
//         key = KeyNum.Skill1;

//         time = new Fixed(0.7f);
//         timer = new Fixed(0);
//         overTime=0.5f.ToFixed();
     
//     }
  
//     public override void StayUse()
//     {
       
             
//        if(player!=null){
//             rot=data.Input.GetJoyStickDirection(key);
//             if(rot.magnitude<0.1f){
//                 rot=data.transform.forward;
//             }
//             player.transform.Rotation=rot.ToRotation();
//        }
//     }
//     public override void UseOver()
//     {
//         base.UseOver();
       
//        // UnityEngine.Debug.LogError("bulletUse" + data.Input.GetJoyStickDirection(key).ToRotation());
//         for (int i = -30; i <= 30; i += 5)
//         {
//             ShootBullet(data.transform.Position+data.transform.forward,rot.ToRotation() + i);
//         }
//     }
//     protected void ShootBullet(Fixed2 position, Fixed rotation)
//     {
//         Bullet bullet = new Bullet();
//         bullet.user = data;
//         bullet.Init(data.client);
//         bullet.Reset( position, rotation);
//         data.client.objectManager.Instantiate(bullet);
  
//     }
// }
// class SkillRay:SkillBase{
//     GunBase gun;
//     RayShap ray;
//     public override void Init()
//     {
//         key = KeyNum.Skill1;
//         time = new Fixed(0.7f);
//         timer = new Fixed(0);
//         gun = new GunBase();
//         ray = new RayShap(Fixed2.zero);
//         gun.Init(20, data);
    
//     }
   
    
   
//     public override void StayUse()
//     {
//         var rot=data.Input.GetJoyStickDirection(key);
//         //if(gun!=null) gun.Fire(data,rot.ToRotation() );  
//         ShootBullet(data.transform.Position, rot);
     
//     }
//     protected void ShootBullet(Fixed2 position, Fixed2 direction)
//     {
//        // UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
//         var others = data.client.physics.OverlapShap(ray.ResetDirection(position, direction, new Fixed(10)));
//         foreach (var other in others)
//         {
//             if (other != data)
//             {
//                 if(other is HealthData)
//                 {
//                     (other as HealthData).GetHurt(new Fixed(10));
//                 }
//             }
//         }
//         //Bullet bullet = new Bullet();
//         //bullet.user = data;
//         //bullet.Init(data .client);
//         //bullet.Reset(position, rotation);
//         //data.client.objectManager.Instantiate(bullet);
     
//     }
// }
// class SkillGun:SkillBase
// {
//     GunBase gun;
//     RayShap ray;
//     public override void Init()
//     {
//         key = KeyNum.Skill1;
//         time = new Fixed(0.7f);
//         timer = new Fixed(0);
//         gun = new GunBase();
//         ray = new RayShap(Fixed2.zero);
//         overTime=1.ToFixed();
//         gun.Init(20, data);
    
//     }
//     public override void UseOver()
//     {
//         base.UseOver();
     
    
//     }
//     public override void StayUse()
//     {
//         var rot=data.Input.GetJoyStickDirection(key);
//         if(gun!=null) gun.Fire(data,rot.ToRotation() );  
     
//        if(player!=null){
//         player.transform.Rotation=rot.ToRotation();
//        }
//     }

    
//     protected void ShootBullet(Fixed2 position, Fixed2 direction)
//     {
//        // UnityEngine.Debug.DrawRay(position.ToVector3(), direction.ToVector3()*10, UnityEngine.Color.red,0.1f);
//         var others = data.client.physics.OverlapShap(ray.ResetDirection(position, direction, new Fixed(10)));
//         foreach (var other in others)
//         {
//             if (other != data)
//             {
//                 if(other is HealthData)
//                 {
//                     (other as HealthData).GetHurt(new Fixed(10));
//                 }
//             }
//         }
//         //Bullet bullet = new Bullet();
//         //bullet.user = data;
//         //bullet.Init(data .client);
//         //bullet.Reset(position, rotation);
//         //data.client.objectManager.Instantiate(bullet);
     
//     }
// }
