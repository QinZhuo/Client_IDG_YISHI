using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
using IDG ;

public class PlayerShow : NetObjectView<PlayerData> {
    // public NetInfo net;
   // public int clientId = -1;
    public Animator anim;
    private void Start()
    {
       
       
    }
    private void ChangeSkillAnim(SkillId id){
        var animOverride=new AnimatorOverrideController(anim.runtimeAnimatorController);
            var animation=GameViewAssetManager.instance.skillAssets.GetSkillAssets(id).animation;
        animOverride ["Skill"]=animation;
        anim.runtimeAnimatorController=animOverride;
      //  Debug.LogError("更改动画 "+animation.name);
    }
    //float last;
    protected override void OnStart(){
        (data as PlayerData).SetAnimTrigger=anim.SetTrigger;
       (data as PlayerData).SetAnimFloat=anim.SetFloat;
               // data.ClientId = clientId;
        (data as PlayerData).skillList.changeSkill=ChangeSkillAnim;
    }
    
	// public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime){
    //     if(delayTime > 0){
    //         yield return new WaitForSeconds(delayTime);
    //     }
    //     if(lockMovement){
    //         rpgCharacterMovementController.LockMovement();
    //     }
    //     if(lockAction){
    //         LockAction();
    //     }
    //     if(timed){
    //         if(lockTime > 0){
    //             yield return new WaitForSeconds(lockTime);
    //         }
    //         UnLock(lockMovement, lockAction);
    //     }
	// }


    // Use this for initialization


    // Update is called once per frame
    //void Update () {

    //}
}
public abstract class HealthData : NetData
{
    protected Fixed _m_Hp=new Fixed(100);
    protected bool isDead=false;
    public virtual void GetHurt(Fixed atk)
    {
        if (!isDead)
        {
            _m_Hp -= atk;
            Debug.Log(this.name + " GetHurt "+atk+" Hp:"+_m_Hp);
            if (_m_Hp <= 0)
            {
                Die();
            }
        }
        
    }
    protected virtual void Die()
    {
        isDead = true;
        Debug.Log(this.name + "dead!!!");
    }
}
public enum AnimStatus
{
    none,
    move,
    useSkill,
    
}
public class PlayerData: HealthData
{
    public Fixed move_speed = new Fixed(3);
    public Fixed2 move_dir { get; private set; }
    public SkillSystem skillList;
   
    public AnimStatus status;
    public System.Action<string> SetAnimTrigger;
    public System.Action<string,float> SetAnimFloat;
    
    public override void Start()
    {
        
        this.tag = "Player";
        skillList= AddCommponent<SkillSystem>();
        Shap = new CircleShap(new Fixed(0.25f), 8);
        rigibody.useCheck=true;
        
     
        if (IsLocalPlayer)
        {
            client.localPlayer = this;
            Debug.Log("client.localPlayer");
        }
        base.Start();
    }
    protected override void FrameUpdate()
    {
     
       //     Debug.LogError("move"+Input.GetKey(IDG.KeyNum.MoveKey) );
        
   //     Debug.LogError("move"+move);
       
        if(status==AnimStatus.none||status==AnimStatus.move){
            move_dir = Input.GetKey(IDG.KeyNum.MoveKey) ? Input.GetJoyStickDirection(IDG.KeyNum.MoveKey):Fixed2.zero;
            transform.Position += move_dir * deltaTime* move_speed;
            if (move_dir.x != 0 || move_dir.y != 0)
            {
                transform.Rotation = Fixed.RotationLerp(transform.Rotation, move_dir.ToRotation(),new Fixed(0.5f));
                status=AnimStatus.move;
                
            }else
            {
                status=AnimStatus.none;
               
            }
           
        }
        if(SetAnimFloat!=null)
         SetAnimFloat("Speed",status==AnimStatus.move?1:0);
        
      
  
    }

    public override string PrefabPath()
    {
        return "Prefabs/Player"+client.random.Range(1);
    }
}

