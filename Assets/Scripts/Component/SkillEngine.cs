using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
public interface ISkillNodeRun
{
    SkillRuntime skill { get; set; }
}
public static class SkillNodeRun
{
    public static void ShootBullet(NetData netData, Fixed2 position, Fixed rotation)
    {
        Bullet bullet = new Bullet();
        bullet.user = netData;
        bullet.Init(netData.client);
        bullet.Reset(position, rotation);
        netData.client.objectManager.Instantiate(bullet);
    }
 
    public static void SetTrigger(this SkillNode node, SkillTrigger trigger,ISkillNodeRun run)
    {
        RunNodes(run, node.GetNodes(trigger));
    }
    public static void RunNodes(ISkillNodeRun run,List<SkillNode> nodes)
    {
        foreach (var node in nodes)
        {
            RunNode(run,node);
        }
    }
    public static void RunNode(ISkillNodeRun run, SkillNode node)
    {
        // UnityEngine.Debug.Log("runNode ["+node.trigger+"] "+node.type);
        switch (node.type)
        {
            case SkillNodeType.RotatePlayer:
                if (run.skill.player != null)
                {
                    var rot = run.skill.player.Input.GetJoyStickDirection(run.skill.key);
                    if (rot.magnitude < 0.1f)
                    {
                        rot = run.skill.player.transform.forward;
                    }
                    run.skill.player.transform.Rotation = rot.ToRotation();
                }
                break;
            case SkillNodeType.MoveCtr:
                if (node.boolParams[0])
                {
                    run.skill.player.CanMove = true;
                    run.skill.player.animRootMotion(false);
                }
                else
                {
                    run.skill.player.CanMove = false;
                    run.skill.player.animRootMotion(true);
                }
                break;
            case SkillNodeType.WaitTime:

                run.skill.player.client.coroutine.WaitCall(node.fixedParams[0],
                    () =>
                    {
                        RunNodes(run,node.GetNodes(SkillTrigger.Time));
                    });
                break;
            case SkillNodeType.LoopTime:
                //RunNodes(node.GetNodes(SkillTrigger.Time));
                //var c = run.player.client.coroutine.WaitCall(node.fixedParams[1],
                //    () =>
                //    {
                //        RunNodes(node.GetNodes(SkillTrigger.Time));
                //    }
                //    , true);
                //times.Add(node.fixedParams[0], c);
                break;
            case SkillNodeType.Damage:
                foreach (var other in run.skill.others)
                {
                    if (other is HealthData)
                    {
                        if (other != run.skill.player)
                        {
                            var enemy = other as HealthData;
                            //Debug.LogError("damage " + run.skill.weapon.weaponData.fixedParams["mainDamage"]);
                            if(run.skill.player.client.GetManager<TeamManager>().IsEnemy(enemy, run.skill.player))
                            {
                                enemy.GetHurt(node.fixedParams[0] + run.skill.weapon.weaponData.fixedParams["mainDamage"]);
                            }
                            
                        }
                    }
                }
                break;
            case SkillNodeType.StopLoop:
                //coroutine.StopCoroutine(times[node.fixedParams[0]]);
                //times.Remove(node.fixedParams[0]);
                break;
            case SkillNodeType.GunFire:
                run.skill.player.weaponSystem.curWeapon<Gun>().Fire(node, run);
                break;
            case SkillNodeType.CreatCheck:
                var check = new SkillCheck();
                check.skill = run.skill;
                check.Check(run.skill.player, node.fixedParams[0], node.fixedParams[1], node.fixedParams[2], node);
                break;
            case SkillNodeType.CreateBullet:
                ShootBullet(run.skill.player,run.skill.player.transform.Position, run.skill.player.transform.Rotation);
                break;
            default: break;
        }
    }
}
public class SkillCheck:ISkillNodeRun{
    
    PlayerData player;
    SkillNode node;

    SkillRuntime _skill;
    public SkillRuntime skill
    {
        get
        {
            return _skill;
        }

        set
        {
            _skill = value;
        }
    }

    public SkillCheck Check(PlayerData player,Fixed width,Fixed height,Fixed forwardOffset,SkillNode node){
        this.node=node;
        this.player=player;
		var shap= new BoxShap(height,width,player.transform.forward*forwardOffset+player.transform.Position,player.transform.Rotation);
        var otherList= player.client.physics.OverlapShap(shap);
        skill.others = otherList;
       
        if(otherList.Count>0){
           // UnityEngine.Debug.LogError("碰撞数 "+otherList.Count  );
            foreach (var other in otherList)
            {
                if(other is NetData){
            //        UnityEngine.Debug.LogError("碰撞 "+other.name+" "+other.GetType()   );
                }
            }
           node.SetTrigger(SkillTrigger.Check,this);
        }
        
		
		return this;
	}

    // public void RunNodes(List<SkillNode> nodes){
    //    foreach (var node in nodes)
    //    {
    //        RunNode(node);
    //    }
    //}
    //public void RunNode(SkillNode node){
       
    //   // UnityEngine.Debug.Log("runNode ["+node.trigger+"] "+node.type);
    //    switch (node.type)
    //    {
            
    //        case SkillNodeType.CreatCheck:
            
    //            new SkillCheck().Check(player,node.fixedParams[0],node.fixedParams[1],node.fixedParams[2],node);
    //        break;
    //        default:break;
    //    }
    //}

}
public enum SkillStatus
{
    CanUse,
    UseStart,
    UseOver,
    CoolDown,
}
public class SkillRuntime : ComponentBase,ISkillNodeRun
{
    public SkillId skillId
    {
        get
        {
            return skillData.skillId;
        }
    }
    public SkillRuntime(SkillData skillData)
    {
        this.skillData = skillData;
       
    }
    public List<NetData> _others=new List<NetData>();
    public List<NetData> others
    {
        get {  return _others; }
        set
        {
            _others = value;
        }
    }
    public Dictionary<Fixed, IEnumerator> times=new Dictionary<Fixed, IEnumerator>();
    public SkillStatus status = SkillStatus.CanUse;
    public KeyNum key
    {
        get
        {
            return skillData.key;
        }
    }
    public Fixed timer;

    public PlayerData player {
        get
        {
            return netData as PlayerData;
        }
    }
    public int skillSetId;
    public SkillData skillData;

    public CoroutineManager coroutine
    {
        get
        {
            return netData.client.coroutine;
        }
    }

    public SkillRuntime skill
    {
        get
        {
            return this;
        }
        set
        {
            
        }
    }
    public WeaponRuntime weapon;
    public void StartUse()
    {
        weapon = player.weaponSystem.curWeapon<WeaponRuntime>();
        skillData.SetTrigger(SkillTrigger.PressStart, this);
        

    }
    public void UseOver()
    {

        skillData.SetTrigger(SkillTrigger.PressOver, this);


        if (player.CanMove)
        {
            UnityEngine.Debug.LogError("错误UseOver " + status);
        }
        //          UnityEngine.Debug.LogError("useOver");
    }
    public void StayUse()
    {
        skillData.SetTrigger(SkillTrigger.PressStay, this);
        //        UnityEngine.Debug.LogError("stayUse");
    }

    public void AnimOver()
    {
        skillData.SetTrigger(SkillTrigger.AnimOver, this);

    }


    public void CoolDown()
    {
       
    }
    public override void Update()
    {
        if (netData.Input.GetKeyDown(key))
        {
             //    UnityEngine.Debug.LogError("update " + data.client.inputCenter.Time +" "+data.view.gameObject.name);
            UnityEngine.Debug.LogError("keyDown " + key + " " + netData.view.gameObject.name);
        }
        if (netData.Input.GetKeyUp(key))
        {
            UnityEngine.Debug.LogError("keyUp " + key + " " + netData.view.gameObject.name);     
        }
        if (status == SkillStatus.CanUse)
        {
            if (netData.Input.GetKeyDown(key))
            {

                StartUse();
                status = SkillStatus.UseStart;
            }
           
        }
        if (status == SkillStatus.UseStart)
        {
            if (netData.Input.GetKey(key))
            {
                // UnityEngine.Debug.LogError("GetKey使用技能 ["+skillId+"] ");
                StayUse();

            }
            if (netData.Input.GetKeyUp(key))
            {

                UseOver();
                // UnityEngine.Debug.LogError("GetKeyUp使用技能 ["+skillId+"] ");
                if (skillData.animTime > 0)
                {
                    (netData as PlayerData).SetAnimTrigger("UseSkill");
                   
                }
               
                timer = skillData.animTime;
                status = SkillStatus.UseOver;

            }
        }
         if (status == SkillStatus.UseOver)
        {
            if (timer > 0)
            {
                timer -= netData.deltaTime;
            }
            else
            {
                timer = skillData.coolDownTime;
                AnimOver();
                status = SkillStatus.CoolDown;

            }
        }
        else if (status == SkillStatus.CoolDown)
        {
            if (timer > 0)
            {
                timer -= netData.deltaTime;
            }
            else
            {
                status = SkillStatus.CanUse;
            }
        }
        //  CoolDown();

    }
   
} 



//public class SkillSet:ComponentBase{
//    public List<SkillBase> skills;
//    public int currentId;
//    public SkillBase GetCurrentSkill(){
//        if(currentId>=0){
//            return skills[currentId];
//        }else
//        {
//            return null;
//        }
//    }
//    public override void Init(){
//        skills=new List<SkillBase>();
//        currentId=-1;

//    }
//    public int NextId(){
//        currentId++;
//        currentId=currentId%skills.Count;
//        if(data==null){
//            UnityEngine.Debug.LogError("data is null");
//        }
//        var func= (data as PlayerData).skillList.changeSkill;
//        if(func!=null){
//            func(skills[currentId].skillId);
//        }
//        return currentId;
//    }
//    public void Add(SkillBase skill){
//        this.skills.Clear();
//        skill.skillSetId=this.skills.Count;
//        this.skills.Add(skill);
//        skill.set=this;
//        if(currentId==-1){
//            NextId();
//        }
         
//    }
//    public override void Update(){
        
//        var curSkill=GetCurrentSkill();
//        if(curSkill!=null){
//            curSkill.Update();
//        }
//        foreach (var skill in skills)
//        {
//            skill.CoolDown();

//        }
//    }
//}
public class SkillEngine:ComponentBase
{
    public Dictionary<KeyNum, SkillRuntime> skillTable;
    public Action<SkillId> changeSkill;
    public override void Init()
    {
        skillTable = new Dictionary<KeyNum, SkillRuntime>();
        //skillTable.Add(KeyNum.Skill1, null);
        //skillTable.Add(KeyNum.Skill2, null);
        //skillTable.Add(KeyNum.Skill3, null);
    }
    public SkillRuntime GetSkill(KeyNum key){
        SkillRuntime skill=null;
        if (skillTable.ContainsKey(key)){
            skill = skillTable[key];
        }
        return skill;
    }
    public void AddSkill(SkillId skillId){
        if(skillId==SkillId.none)return;
        AddSkill(SkillManager.GetSkill(skillId));
       
    }
    public void AddSkill(SkillRuntime skill)
    {
        skill.InitNetData(this.netData);
     
        if (skillTable.ContainsKey(skill.key))
        {
            skillTable[skill.key] = skill;
        }else
        {
            skillTable.Add(skill.key, skill);
            //UnityEngine.Debug.LogError("错误的Key "+skill.key);
        }
        changeSkill(skill.skillId);
        //
    }
    public override void Update()
    {
   
        foreach (var item in skillTable)
        {
            // if (item.Value.Count>0)
            // {
            //     item.Value[item.Value.Count-1].Update();
            // }

            item.Value.Update();
         //   UnityEngine.Debug.LogError(item.Key+" "+item.Value.skills.Count);
        } 
    }

}

