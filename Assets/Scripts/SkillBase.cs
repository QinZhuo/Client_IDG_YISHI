using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
using IDG.FSClient;

public class SkillBase:ComponentBase
{
    public KeyNum key;
    public Fixed timer;
    public Fixed time;
    public int skillSetId;
    public SkillSet set;
    public virtual void UseOver()
    {
        timer = time;
     //   UnityEngine.Debug.LogError("UseOver");
    }
    public virtual void StayUse(){
      //  timer = time;
      //  UnityEngine.Debug.LogError("StayUse");
    }

    public void CoolDown(){
        if (timer > 0)
        {
           timer -= data.deltaTime;
        }
    }
    public override void Update()
    {
        if(timer<=0)
        {
            if(data.Input.GetKey(key)){
                StayUse();
            }
            if (data.Input.GetKeyUp(key))
            {
                
                UseOver();
                set.NextId();
              
            }
        }
     
    }
    
}
public class SkillSet{
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
    public SkillSet Init(){
        skills=new List<SkillBase>();
        currentId=-1;
        return this;
    }
    public int NextId(){
        currentId++;
        currentId=currentId%skills.Count;
       
        return currentId;
    }
    public void Add(SkillBase skill){
        if(currentId==-1)currentId=0;
         skill.skillSetId=this.skills.Count;
        this.skills.Add(skill);
        skill.set=this;
         
    }
    public void Update(){
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
    
    public SkillSystem()
    {
        skillTable = new Dictionary<KeyNum, SkillSet>();
        skillTable.Add(KeyNum.Skill1, new SkillSet().Init());
        skillTable.Add(KeyNum.Skill2, new SkillSet().Init());
        skillTable.Add(KeyNum.Skill3, new SkillSet().Init());
    }
    public SkillBase GetCurrentSkill(KeyNum key){
        SkillBase skill=null;
        if (skillTable.ContainsKey(key)){
            skill=skillTable[key].GetCurrentSkill();
        }
        return skill;
    }
    public void AddSkill(SkillBase skill)
    {
        skill.InitNetData(this.data);
        if (skillTable.ContainsKey(skill.key))
        {
            skillTable[skill.key].Add(skill);
        }
      
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

