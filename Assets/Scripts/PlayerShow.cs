using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public struct AnimData
{
    public string name;
    public AnimationClip clip;
    public float speed;
    public AnimData(string name,AnimationClip clip,float speed = 1)
    {
        this.name = name;
        this.clip = clip;
        this.speed = speed;
    }
}
public class PlayerShow : NetObjectView<PlayerData> {
    // public NetInfo net;
   // public int clientId = -1;
    public Animator anim;
    public Transform leftHand;
    public Transform rightHand;
    public HPManager hpManager;
    public HpView hpView;
    PlayerData player;
    private void Start()
    {
        hpManager = netData.client.GetManager<HPManager>();
       
    }
    private void ChangeSkillAnim(SkillId id){
        var skill=GameViewAssetManager.instance.skillAssets.Get(id.ToString());
        ChangeAnim(new AnimData( "Skill",skill.useOverAnim,skill.useOverAnimSspeed));
      //  Debug.LogError("更改动画 "+animation.name);
    }

    protected void ChangeAnim(params AnimData[] anims){
        var animOverride=anim.runtimeAnimatorController as AnimatorOverrideController;
        if(animOverride==null){
            animOverride=new AnimatorOverrideController();
            animOverride.runtimeAnimatorController=anim.runtimeAnimatorController;
        }
        
        for (int i = 0; i < anims.Length; i++)
        {
             animOverride[anims[i].name]=anims[i].clip;
            anim.SetFloat(anims[i].name + "_Speed", anims[i].speed);

        }
        
        anim.runtimeAnimatorController=animOverride;
    }
    protected void ChangeWeapon(WeaponId id){
        var weaponAssets=GameViewAssetManager.instance.weaponAssets.Get(id.ToString());
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

        ChangeAnim(new AnimData("Idle", weaponAssets.idleAnim), new AnimData("Walk", weaponAssets.animation));
    }
    //float last;
    protected override void OnStart(){
        player = (netData as PlayerData);
        player.SetAnimTrigger=anim.SetTrigger;
        player.SetAnimFloat=anim.SetFloat;
        // data.ClientId = clientId;
        player.skillList.changeSkill=ChangeSkillAnim;
        player.weaponSystem.changeWeapon=ChangeWeapon;
        player.animRootMotion=(b)=>{
            anim.transform.localPosition=Vector3.zero;
            anim.applyRootMotion=b;
        };
       // (data.client.unityClient as FightClientForUnity3D).mainCamera
    }
    override protected void Update()
    {
        base.Update();
        Vector3 pos = hpManager.mainCam.WorldToViewportPoint(transform.position);
        if (InCamera(pos))
        {
            if (hpView == null)
            {
                hpView=hpManager.GetView();
            }
           
            hpView.transform.position = hpManager.mainCam.WorldToScreenPoint(transform.position + Vector3.up * 2.5f);
            hpView.SetSlider((player.Hp / player.MaxHealth).ToFloat());

        }
        else
        {
            if (hpView != null)
            {
                hpManager.Recover(hpView);
                hpView = null;
            }
        }
       // hpView.SetSlider((player.Hp / player.MaxHealth).ToFloat());
        
    }
    protected bool InCamera(Vector3 camPos)
    {
        if (camPos.x < 0 || camPos.x > 1 || camPos.y < 0 || camPos.y > 1)
        {
            return false;
        }
        else
        {
            return true;

        }
    }

    private void OnDestroy()
    {
        hpManager.Recover(hpView);
        hpView = null;
    }
    // Use this for initialization


    // Update is called once per frame
    //void Update () {

    //}
}
public abstract class HealthData : NetData, ITeam
{
    protected Fixed _m_Hp=new Fixed(100);
    protected Fixed maxHealth = 100.ToFixed();
    public int team;
    int ITeam.team
    {
        get
        {
            return team;
        }
    }
    public Fixed Hp
    {
        get
        {
            return _m_Hp;
        }
    }
    public Fixed MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }
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
    public Fixed move_speed = new Fixed(2);
    public Fixed2 move_dir { get; private set; }
    public SkillEngine skillList;
    public WeaponEngine weaponSystem;
   // public AnimStatus status;
    public System.Action<bool> animRootMotion;
    public System.Action<string> SetAnimTrigger;
    public System.Action<string,float> SetAnimFloat;
    bool _canMove;
    public AiEngine ai;
    public ItemEngine items;
    public bool CanMove{
        set{
            if(!value){
                SetAnimFloat("Speed",0);
            }
            _canMove=value;
        }
        get
        {
            return _canMove;
        }
    }
  

    public override void Start()
    {
        
        this.tag = "Player";
        skillList= AddCommponent<SkillEngine>();
        weaponSystem= AddCommponent<WeaponEngine>();
        Shap = new CircleShap(new Fixed(0.25f), 8);
        rigibody.useCheck=true;
        rigibody.useCheckCallBack = true;
        CanMove=true;
        if (IsLocalPlayer)
        {
            client.localPlayer = this;
            Debug.Log("client.localPlayer");
        }
        team = 1;
        items =AddCommponent<ItemEngine>();
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
        if(Input.GetKeyUp(KeyNum.Drop)){
            items.PickUp();
        }
    }

    public override string PrefabPath()
    {
        return "Prefabs/Player"+client.random.Range(1);
    }
}

