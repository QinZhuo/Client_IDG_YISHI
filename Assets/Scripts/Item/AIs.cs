using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;


public class AiManager:ISerializable{
    public static AiManager instance;
    public static Dictionary<string,AiData> aiList;
    public static void Init(){
        instance=new AiManager();
        instance.LoadTable();
    }
    public void LoadTable(){
         UnityEngine.Debug.Log("初始化AI中 ...");
        aiList=new Dictionary<string,AiData>();
        DataFile.Instance.DeserializeToData("AIsSetting.iai",this);
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
        aiList.Clear();
       var len=protocol.getInt32();
        for (int i = 0; i < len; i++)
        {
            var data =new AiData();
            data.Deserialize(protocol);
            UnityEngine.Debug.Log("初始化AI ["+data.name+"]");
            aiList.Add(data.name,data);
            
        }
        UnityEngine.Debug.Log("AI初始化完毕 AI数"+aiList.Count);
    }
    public static AiData Get(string name){
        
        if(aiList.ContainsKey(name)){
            return aiList[name];
        }else
        {
            UnityEngine.Debug.LogError("不存在AI【"+name+"】");
            return null;
        }
       
    }

}

public enum AINodeStatus{
    Success,
    Failure,
  //  Running,
}
[System.Serializable]
public class AiData:AINode,ISerializable{
    public string name="";
    public override void Serialize(ByteProtocol protocol){
        protocol.push(name);
        base.Serialize(protocol);
        
    }
    public override void Deserialize(ByteProtocol protocol){
        name=protocol.getString();
        
        base.Deserialize(protocol);
    }
}
[System.Serializable]
public class AINode:ITreeNode<AINode>,ISerializable{
    public Int32 intType=0;

     public ActionType ActionType{
        get{
            return (ActionType)intType;
        }
        set{
            intType=(Int32)value;
        }
    }
     public CompositeType CompositeType{
        get{
            return (CompositeType)intType;
        }
        set{
            intType=(Int32)value;
        }
    }
    public List<AINode> childNodes{
        get{ return childes;}
    }
    public List<AINode> childes=new List<AINode>();
    public virtual AINodeStatus Update(Func<AINode,AINodeStatus> actionCall){
         
        if(intType>10&&intType<100){
            var type=CompositeType;
          //  UnityEngine.Debug.LogError("AIUpdate 组合节点["+type+"] len "+childNodes.Count );
            if(type==CompositeType.Sequences){
                foreach (var aiNode in childes)
                {
                    if(aiNode.Update(actionCall)==AINodeStatus.Failure){
                        return AINodeStatus.Failure;
                    }
                }
                return AINodeStatus.Success;
            }else if(type==CompositeType.Selector){
                foreach (var aiNode in childes)
                {
                    if(aiNode.Update(actionCall)==AINodeStatus.Success){
                        return AINodeStatus.Success;
                    }
                }
                return AINodeStatus.Failure;
            }
        }else if( intType>100)
        {
            return actionCall(this);
        }
        
        return AINodeStatus.Success;
    }

    public virtual void Serialize(ByteProtocol protocol){
        
        protocol.push((Int32)intType);
        
        
       // protocol.push((Int32)type);
       // UnityEngine.Debug.LogError("push type" +(Int32)type);
        protocol.push(childNodes.Count);
      //  UnityEngine.Debug.LogError("nextNode Count" +nextNodes.Count);
        foreach (var node in childNodes)
        {
            node.Serialize(protocol);
        }
        // protocol.push(boolParams.Count);
        // foreach (var b in boolParams)
        // {
        //     protocol.push(b);
        // }
        // protocol.push(intParams.Count);
        // foreach (var b in intParams)
        // {
        //     protocol.push(b);
        // }
    }
    public virtual void Deserialize(ByteProtocol protocol){
       
        intType=protocol.getInt32();
      //  UnityEngine.Debug.LogError("get type" +(Int32)type);
        var len=protocol.getInt32();
       //  UnityEngine.Debug.LogError("get nextNode Count" +len);
        childNodes.Clear();
        for (int i = 0; i < len; i++)
        {
            var node=new AINode();
            node.Deserialize(protocol);
            childNodes.Add(node);
        }
        // len=protocol.getInt32();
        // boolParams.Clear();
        // for (int i = 0; i < len; i++)
        // {
        //     boolParams.Add(protocol.getBoolean());
        // }
        // len=protocol.getInt32();
        // intParams.Clear();
        // for (int i = 0; i < len; i++)
        // {
        //     intParams.Add(protocol.getInt32());
        // }
    }
}

public enum CompositeType{
    Sequences=11,
    Selector=12,
}

public enum ActionType{
    hasEnemy=101,
    findEnemy=102,
    enemyInRange=103,
    moveToEnemy=104,
    rotateToEnemy=105,
    skill1=106,
    findPath=107,
}


