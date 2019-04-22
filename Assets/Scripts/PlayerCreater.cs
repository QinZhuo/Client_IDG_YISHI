using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class PlayerCreater : MonoBehaviour,IGameManager {
    public int player=3;
    public int item=30;
    public int enemy=10;
     public int InitLayer{
        get{
            return 101;
        }
    }
    public void Init(FSClient  client){
         var map=client.GetManager<RandomMapCreator>();
         var playerPos=Fixed2.zero;
         if(map){
             playerPos= map.GetRandomPos();
         }else
         {
             playerPos=new Fixed2( client.random.Range(0,20),client.random.Range(0,20));
         }
        for (int i = 0; i < player; i++)
        {
           
            var player = new PlayerData();
            player.Init(client,i);
          

            if(map!=null){
                 player.transform.Position =playerPos+Fixed2.left*i.ToFixed();
            }else
            {
                 player.transform.Position = new IDG.Fixed2(0, i);
            }
           
            client.objectManager.Instantiate(player);
        }
        for (int i = 0; i < item; i++)
        {
           
           

           
                
            ItemData item;
            if(i%2==0){
                var Titem = new SkillItem();
                item=Titem;
                Titem.Init(client);
                if(client.random.Range(0,100)<50){
                    Titem.skillId=SkillId.拳击右直;
                }else
                {
                     Titem.skillId=SkillId.拳击左直;
                }
                
            }else
            {
                var Titem = new WeaponItem();
                item=Titem;
                Titem.Init(client);
                Titem.weaponId=WeaponId.白刀;
            }
         
            
            
            if(map){
                item.transform.Position = map.GetRandomPos();
            }else
            {
                item.transform.Position=new Fixed2( client.random.Range(0,20),client.random.Range(0,20));
            }
            client.objectManager.Instantiate(item);
            
           
          
        }
         for (int i = 0; i < enemy; i++)
        {
           
           
            

            
                
           var item = new ZombieData();
                  
            item.Init(client);
                if(map){
                item.transform.Position = map.GetRandomPos();
            }else
            {
                item.transform.Position=new Fixed2( client.random.Range(0,20),client.random.Range(0,20));
            }
            client.objectManager.Instantiate(item);
                    
            
           
          
        }
    }
	
	

}
