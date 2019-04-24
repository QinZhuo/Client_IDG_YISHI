using System;
using System.Collections.Generic;
using IDG;

public class AiManager: DataManager<AiManager,AiData>
{
    public override string TableName()
    {
        return "AiSetting";
    }
}

public enum AINodeStatus{
    Success,
    Failure,
}
[System.Serializable]
public class AiData:AINode,IDataClass{
    public string name="";

    public string Id
    {
        get
        {
            return name;
        }
    }

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
        
        
       
        protocol.push(childNodes.Count);
  
        foreach (var node in childNodes)
        {
            node.Serialize(protocol);
        }
      
    }
    public virtual void Deserialize(ByteProtocol protocol){
       
        intType=protocol.getInt32();
   
        var len=protocol.getInt32();
  
        childNodes.Clear();
        for (int i = 0; i < len; i++)
        {
            var node=new AINode();
            node.Deserialize(protocol);
            childNodes.Add(node);
        }
      
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


