using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IDG;

namespace IDG.FSClient
{
    public class GunBase:ItemBase
    {   
        protected Fixed firingInterval;
        protected Fixed lastTime;
       // protected GunSetting gunSetting;
        protected Fixed timer;
        public void Init(float firingRate,NetData User)
        {
            this.firingInterval = new Fixed(1/firingRate);
            lastTime = Fixed.Zero;
           // gunSetting = DataManager.Instance.gunManager.gun;
            this.user = User;
            timer = Fixed.Zero;
        }
        public void Fire(NetData user, Fixed rotation)
        {
            var t =  user.client.inputCenter.Time - lastTime;
            if (t > 0.1f)
            {
             
                Fixed rote =new Fixed(0);
                
                lastTime = user.client.inputCenter.Time;
                ShootBullet(user.transform.Position,rotation+ rote);
            }
            else
            {

            }
        }
        protected virtual void ShootBullet(Fixed2 position, Fixed rotation)
        {
            Bullet data = new Bullet();
            data.user = this.user;
            data.Init(user.client);
            data.Reset(position, rotation);
            user.client.objectManager.Instantiate(data);
        } 
    }
}
