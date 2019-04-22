using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;

public class PlayerShow : NetObjectView<PlayerData> {
    // public NetInfo net;
   // public int clientId = -1;
    public Animator anim;
    public Transform leftHand;
    public Transform rightHand;
    private void Start()
    {
       
       
    }
    private void ChangeSkillAnim(SkillId id){
        var animation=GameViewAssetManager.instance.skillAssets.GetSkillAssets(id).animation;
        ChangeAnim("Skill",animation);
      //  Debug.LogError("更改动画 "+animation.name);
    }

    protected void ChangeAnim(string nameArry,params AnimationClip[] anims){
        var animOverride=anim.runtimeAnimatorController as AnimatorOverrideController;
        if(animOverride==null){
            animOverride=new AnimatorOverrideController();
            animOverride.runtimeAnimatorController=anim.runtimeAnimatorController;
        }
        string[] animNames=nameArry.Split('|');
        for (int i = 0; i < animNames.Length; i++)
        {
             animOverride[animNames[i]]=anims[i];
        }
        anim.runtimeAnimatorController=animOverride;
    }
    protected void ChangeWeapon(WeaponId id){
        var weaponAssets=GameViewAssetManager.instance.weaponAssets.GetWeaponAssets(id);
        var weaponPrefab=weaponAssets.ItemPrefab;
        if(weaponPrefab!=null){
           
            var obj=Instantiate(weaponPrefab);
            obj.transform.SetParent(rightHand);
            obj.transform.localPosition=Vector3.zero;
            obj.transform.localRotation=Quaternion.identity;
        }else
        {
             Debug.LogWarning("WeaponId "+id+" itemPrefab Is Null");
        }
        ChangeAnim("Idle|Walk",weaponAssets.idleAnim,weaponAssets.animation);
    }
    //float last;
    protected override void OnStart(){
        var player=(data as PlayerData);
        (data as PlayerData).SetAnimTrigger=anim.SetTrigger;
       (data as PlayerData).SetAnimFloat=anim.SetFloat;
               // data.ClientId = clientId;
        (data as PlayerData).skillList.changeSkill=ChangeSkillAnim;
        (data as PlayerData).weaponSystem.changeWeapon=ChangeWeapon;
        player.animRootMotion=(b)=>{
            anim.transform.localPosition=Vector3.zero;
            anim.applyRootMotion=b;
        };
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
// public enum AnimStatus
// {
//     none,
//     move,
//     useSkill,
    
// }
public class PlayerData: HealthData
{
    public Fixed move_speed = new Fixed(3);
    public Fixed2 move_dir { get; private set; }
    public SkillAction skillList;
    public WeaponSystem weaponSystem;
   // public AnimStatus status;
    public System.Action<bool> animRootMotion;
    public System.Action<string> SetAnimTrigger;
    public System.Action<string,float> SetAnimFloat;
    bool _canMove;
    public AiEngine ai;
    public bool CanMove{
        set{
            if(!value){
                SetAnimFloat("Speed",0);
            }
            _canMove=value;
        }
    }
    public override void Start()
    {
        
        this.tag = "Player";
        skillList= AddCommponent<SkillAction>();
        weaponSystem= AddCommponent<WeaponSystem>();
        Shap = new CircleShap(new Fixed(0.25f), 8);
        rigibody.useCheck=true;
        CanMove=true;
        if (IsLocalPlayer)
        {
            client.localPlayer = this;
            Debug.Log("client.localPlayer");
        }
        //  ai=new AiEngine();
        //  ai.aiName="AI_test";
        
        // ai=AddCommponent<AiEngine>(ai);
       
        // ai.emenyTag="Zombie";
        base.Start();
         weaponSystem.AddWeapon(WeaponId.无战斗);
         skillList.AddSkill(SkillManager.GetSkill(SkillId.拳击右直));
    }
  
    protected override void FrameUpdate()
    {
     
       //     Debug.LogError("move"+Input.GetKey(IDG.KeyNum.MoveKey) );
        
   //     Debug.LogError("move"+move);
      
        if(_canMove){
//             Debug.LogError("dir "+Input.GetJoyStickDirection(IDG.KeyNum.MoveKey)+" key "+ Input.GetKey(IDG.KeyNum.MoveKey) );
            move_dir = Input.GetKey(IDG.KeyNum.MoveKey) ? Input.GetJoyStickDirection(IDG.KeyNum.MoveKey):Fixed2.zero;
            transform.Position += move_dir * deltaTime* move_speed;
            if (move_dir.x != 0 || move_dir.y != 0)
            {
                transform.Rotation = Fixed.RotationLerp(transform.Rotation, move_dir.ToRotation(),new Fixed(0.5f));
                if(SetAnimFloat!=null)
                 SetAnimFloat("Speed",1);
                
            }else
            {
                 if(SetAnimFloat!=null)
                 SetAnimFloat("Speed",0);
            }
           
        }
    }

    public override string PrefabPath()
    {
        return "Prefabs/Player"+client.random.Range(1);
    }
}

