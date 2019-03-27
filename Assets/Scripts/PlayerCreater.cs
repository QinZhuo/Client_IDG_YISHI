using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
using IDG.FSClient;

public class PlayerCreater : MonoBehaviour,IGameManager {
    public int player=3;
    public int item=30;
    public int enemy=10;
     public int InitLayer{
        get{
            return 101;
        }
    }
    public void Init(FSClient client){
         var map=client.GetManager<RandomMapCreator>();
           var playerPos= map.GetRandomPos();
        for (int i = 0; i < player; i++)
        {
           
            var player = new PlayerData();
            player.Init(client);
            player.clientId = i;

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
           
           
            

            if(map!=null){
                
                  var item = new ItemData();
                  item.Init(client);
                    item.transform.Position = map.GetRandomPos();
                
                client.objectManager.Instantiate(item);
            }
           
          
        }
         for (int i = 0; i < enemy; i++)
        {
           
           
            

            if(map!=null){
                
                  var item = new ZombieData();
                  
                    item.Init(client);
                     item.transform.Position = map.GetRandomPos();
                    client.objectManager.Instantiate(item);
                    
            }
           
          
        }
    }
	
	

}
