using System;
using System.Collections;
using System.Collections.Generic;
namespace IDG
{
    

public interface IWait
{
    /// <summary>
    /// 每帧检测是否等待结束
    /// </summary>
    /// <returns>迭代是否结束</returns>
    bool Tick();
}
/// <summary>
/// 按秒等待
/// </summary>
public class WaitForSeconds:IWait
{
    public Fixed curTime{
        get{
            return _seconds;
        }
    }
    Fixed _seconds = Fixed.Zero;
 
    public WaitForSeconds(Fixed seconds)
    {
        _seconds = seconds;
    }
    public WaitForSeconds(int seconds)
    {
        _seconds = seconds.ToFixed();
    }
    public bool Tick()
    {
        _seconds -= FSClient.deltaTime;
        return _seconds <= 0;
    }
}
public class CoroutineManager
{
    private List<IEnumerator> coroutineList = new List<IEnumerator>();
    private List<IEnumerator> endList=new List<IEnumerator>();      
    public IEnumerator StartCoroutine(IEnumerator ie)
    {
        coroutineList.Add(ie);
       // UnityEngine.Debug.LogError("添加协程");
        return ie;
    }
    public void StopCoroutine(IEnumerator ie)
    {
        try
        {
            coroutineList.Remove(ie);
        }
        catch (Exception e) { Console.WriteLine(e.ToString()); }
    }
    public void UpdateCoroutine()
    {
        IEnumerator node=null;
      
        foreach (var coroutine in coroutineList)
        {
          
            bool next=true;
            if(coroutine.Current is IWait){
                IWait wait=(IWait)coroutine.Current;
               // UnityEngine.Debug.LogError("迭代协程 "+(wait as WaitForSeconds).curTime);
                if(wait.Tick()){
                    
                    next=coroutine.MoveNext();
                }
            }else{
                next=coroutine.MoveNext();
            }
            if(!next){
                endList.Add(coroutine);
            }
            
           
        }
        if(endList.Count>0){
            foreach (var c in endList)
            {
                coroutineList.Remove(c);
            }
            endList.Clear();
        }
    }
}
}