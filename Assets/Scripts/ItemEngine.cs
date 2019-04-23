using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG ;


    public class ItemEngine:ComponentBase
    {
        public List<ItemData> canDropList=new List<ItemData>();
        public override void Init(){
            
        }
        public void AddDropList(ItemData item){
            canDropList.Add(item);
        }
        public void RemoveDropList(ItemData item){
            canDropList.Remove(item);
        }
        public void PickUp(int i=0){
            if(canDropList.Count>i){
                canDropList[i].PickUp(data);
                canDropList.RemoveAt(i);
            }
        }
        public override void Update(){
           
        }

   
    }
