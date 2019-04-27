using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG ;

namespace IDG
{
    public class PhysicsComponent
    {
        public bool useCheckCallBack=false;

        public bool useCheck=false;
        public NetData netdata;
     
        public List<NetData> lastCollisonDatas = new List<NetData>();
        public List<NetData> collisonDatas=new List<NetData>();
        public void AddCollistionData(NetData other)
        {
            if (!collisonDatas.Contains(other))
            {
                collisonDatas.Add(other);
            }
        }
        //public void ClearCollistionData()
        //{
        //    //collisonDatas.Clear();
        //}
        public void Init(Action<NetData> enter,Action<NetData> stay, Action<NetData> exit){
            OnPhysicsCheckEnter=enter;
            OnPhysicsCheckStay=stay;

            OnPhysicsCheckExit=exit;
        }
        public void Update(){
            if (useCheckCallBack)
            {
                foreach (var other in this.collisonDatas)
                {
                    if (!lastCollisonDatas.Contains(other))
                    {
                        OnPhysicsCheckEnter(other);
                    }
                    else
                    {
                        lastCollisonDatas.Remove(other);
                    }
                    //Debug.Log("1 "+this.collisonDatas+" "+this.collisonDatas.Count);
                    OnPhysicsCheckStay(other);
                }
                foreach (var other in lastCollisonDatas)
                {
                    OnPhysicsCheckExit(other);
                }
            }
            lastCollisonDatas.Clear();
            lastCollisonDatas.AddRange(collisonDatas);

           
            //  collisonDatas = ShapPhysics.CheckAll(this);

            foreach (var tree in netdata.trees)
            {
                if (tree.collisonInfo.active)
                {
                    collisonDatas.Clear();
                    break;
                }
            }

        }

        public bool CheckCollision(NetData a)
        {
           
            foreach (var item in collisonDatas)
            {

                if (!item.isTrigger)
                {
                    return true;
                }
            }

            return false;
        }
        public Action<NetData> OnPhysicsCheckStay;
        public Action<NetData> OnPhysicsCheckEnter;
        public Action<NetData> OnPhysicsCheckExit;
    }
}