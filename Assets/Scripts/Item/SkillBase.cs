using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
using IDG ;
public enum SkillStatus
{
    CanUse,
    UseOver,
    CoolDown,
}
public class SkillBase:ComponentBase
{
    public SkillId skillId; 

    public SkillStatus status=SkillStatus.CanUse;
    public KeyNum key;
    public Fixed timer;
    public Fixed time;
    public PlayerData player=null;
    public int skillSetId;
    public SkillSet set;
    public Fixed overTime;
    public virtual void UseOver()
    {
       
     //   UnityEngine.Debug.LogError("UseOver");
    }
    public virtual void StayUse(){
      //  timer = time;
      //  UnityEngine.Debug.LogError("StayUse");
    }

    public void CoolDown(){
        if (status==SkillStatus.CoolDown)
        {
            if(timer > 0){
                timer -= data.deltaTime;
            }else
            {
                status=SkillStatus.CanUse;
            }
        }
    }
    public override void Update()
    {
        if(status==SkillStatus.CanUse)
        {
            if(data.Input.GetKey(key)){
                if(player==null){
                    player=data as PlayerData;
                  
                }else{
                    player.status=AnimStatus.useSkill;
                }
                StayUse();
                
            }
            if (data.Input.GetKeyUp(key))
            {
                
                UseOver();
             //   UnityEngine.Debug.LogError("使用技能 ["+skillId+"] ");
                (data as PlayerData).SetAnimTrigger("UseSkill");
                 timer = overTime;
                 status=SkillStatus.UseOver;
                
              
            }
        }else if(status==SkillStatus.UseOver)
        {
            if(timer > 0){
                timer -= data.deltaTime;
            }else
            {
                timer=time;
                if(player!=null){
                    
                    player.status=AnimStatus.none;
                }
                
               
                set.NextId();
                status=SkillStatus.CoolDown;
            }
        }
     
    }
    
}
public class SkillSet:ComponentBase{
    public List<SkillBase> skills;
    public int currentId;
    public SkillBase GetCurrentSkill(){
        if(currentId>=0){
            return skills[currentId];
        }else
        {
            return null;
        }
    }
    public override void Init(){
        skills=new List<SkillBase>();
        currentId=-1;

    }
    public int NextId(){
        currentId++;
        currentId=currentId%skills.Count;
        if(data==null){
            UnityEngine.Debug.LogError("data is null");
        }
        var func= (data as PlayerData).skillList.changeSkill;
        if(func!=null){
            func(skills[currentId].skillId);
        }
        return currentId;
    }
    public void Add(SkillBase skill){
      
         skill.skillSetId=this.skills.Count;
        this.skills.Add(skill);
        skill.set=this;
        if(currentId==-1){
            NextId();
        }
         
    }
    public override void Update(){
        foreach (var skill in skills)
        {
            skill.CoolDown();
           
        }
        var curSkill=GetCurrentSkill();
        if(curSkill!=null){
            curSkill.Update();
        }
    }
}
public class SkillSystem:ComponentBase
{
    public Dictionary<KeyNum, SkillSet> skillTable;
    public Action<SkillId> changeSkill;
    public override void Init()
    {
        skillTable = new Dictionary<KeyNum, SkillSet>();
        skillTable.Add(KeyNum.Skill1, new SkillSet());
        skillTable.Add(KeyNum.Skill2, new SkillSet());
        skillTable.Add(KeyNum.Skill3, new SkillSet());
        foreach (var skillSet in skillTable)
        {
            skillSet.Value.InitNetData(data);
        }
    }
    public SkillBase GetCurrentSkill(KeyNum key){
        SkillBase skill=null;
        if (skillTable.ContainsKey(key)){
            skill=skillTable[key].GetCurrentSkill();
        }
        return skill;
    }
    public void AddSkill(SkillId skillId){
        if(skillId==SkillId.none)return;
        AddSkill(SkillManager.GetSkill(skillId));
    }
    public void AddSkill(SkillBase skill)
    {
        skill.InitNetData(this.data);
        if (skillTable.ContainsKey(skill.key))
        {
            skillTable[skill.key].Add(skill);
        }
        //UnityEngine.Debug.LogError("add "+skill.skillId);
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
        } 
    }

}

