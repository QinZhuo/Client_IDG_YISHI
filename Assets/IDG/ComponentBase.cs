using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG;
namespace IDG
{
    public  class ComponentBase 
    {
        public NetData netData;
        public ComponentBase()
        {

        }
        public virtual void Init()
        {

        }
        public void InitNetData(NetData data)
        {
            this.netData = data;
            Init();
        }
        public virtual  void Update()
        {

        }
      
    }
   
}
