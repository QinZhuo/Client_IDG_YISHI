using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG ;


    public class AiEngine:ComponentBase
    {
        public string emenyTag="";
        public string aiName;
        public AiData aiData;
        public Func<AINode,AINodeStatus> aiActionFunc;
        public PlayerData player;
        public VirtulInput vInput;
        public NetData target;
        public bool hasDirection=false;
        public Fixed2 moveDir;
        public override void Init(){
            aiData=AiManager.Get(aiName);
            player=data as PlayerData;
            vInput=new VirtulInput();
            player.Input=vInput;
        }
        public bool DontHasPath(Fixed2 dir){
            var others= player.client.physics.OverlapShap(new ShapBase(player.Shap.GetPoints(),player.transform.Position+dir.normalized*0.5f.ToFixed(),dir.ToRotation()));
            if(others.Contains(player)){
               others.Remove(player);
            }
            if(others.Count>0){
                return true;
            }else
            {
                return false;
            }
        }
        public AINodeStatus AiAction(AINode node){
           // UnityEngine.Debug.LogError("AI_"+(ActionType)node.intType);
            vInput.Key.Reset();
            var type=(ActionType)node.intType;
            switch (type)
            {
                case ActionType.hasEnemy:{
                    if(target!=null&&target.active){
                        return AINodeStatus.Success;
                    }else
                    {
                       
                      //  vInput.Key.SetKey(false,KeyNum.MoveKey);
                     
                        return AINodeStatus.Failure;
                    }
                    
                }
                case ActionType.findEnemy:{
                        var others = player.client.physics.OverlapShap( new CircleShap(5.ToFixed(), 6), player.transform.Position);
                        foreach (var other in others)
                        {
                            if(other.tag==emenyTag){
                                target=other;
                               // vInput.Key.SetKey(false,KeyNum.MoveKey);
                                return AINodeStatus.Success;
                                
                            }
                        }
                        if(hasDirection){
                            vInput.SetJoyStickDirection(KeyNum.MoveKey,moveDir);
                        }else
                        {
                            moveDir=Fixed2.Parse(player.client.random.Range(0,360).ToFixed());
                            hasDirection=true;
                            player.client.coroutine.WaitCall(3.ToFixed(),()=>{hasDirection=false;});
                        }
                     return AINodeStatus.Failure;
                }
                case ActionType.enemyInRange:{
                    if(Fixed2.DistanceLess(player.transform.Position,target.transform.Position,2.5f.ToFixed())){
                         vInput.Key.SetKey(false,KeyNum.MoveKey);
                        return AINodeStatus.Success;
                    }else
                    {
                        return AINodeStatus.Failure;
                    }
                }
                case ActionType.skill1:{
                    vInput.SetJoyStickDirection(KeyNum.Skill1,target.transform.Position-player.transform.Position);
                   player.client.coroutine.WaitCall(0.5f.ToFixed(),()=>{ vInput.Key.SetKey(false,KeyNum.Skill1);});
                   return AINodeStatus.Success;
                }
                case ActionType.findPath:{
                     moveDir=target.transform.Position-player.transform.Position;
                    
                    if(DontHasPath(moveDir)){
                        var rot=moveDir.ToRotation();
                        rot+=90;
                        moveDir=Fixed2.Parse(rot);
                        if(DontHasPath(moveDir)){
                            rot-=180;
                             moveDir=Fixed2.Parse(rot);
                        }
                    }
                    return AINodeStatus.Success;
                }
                case ActionType.moveToEnemy:{
                   
                     vInput.SetJoyStickDirection(KeyNum.MoveKey,moveDir);
                     return AINodeStatus.Success;
                }
                default:break;
            }

            return AINodeStatus.Failure;
        }
        public override void Update(){
            aiData.Update(AiAction);
        }

   
    }
