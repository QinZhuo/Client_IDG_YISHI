using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDG ;

namespace IDG
{
    public class TransformComponent
    {
        private Fixed2 _position=new Fixed2();
        private Fixed2 _lastPos=new Fixed2();
        private Fixed _lastRota = new Fixed();
        private Fixed _rotation = new Fixed();

        public void LookAt(Fixed2 target){
            Rotation= (target-Position).ToRotation();
        }
        private NetData data;

        private Fixed2 _scale=Fixed2.one;
   
        public void Init(NetData data){
            this.data=data;
        }
        public Fixed2 Scale{
            get{
                return _scale;
            }set{
                if(value!=_scale){
                    _scale=value;
                    if( data.Shap!=null){
                    data.Shap.ResetSize();
                    Tree4.Move(data);
                    }
                }
            }
        }
        public Fixed2 PhysicsPosition{
            get{
                return _position;
            }
        }
        public Fixed2 Position
        {
            get
            {
                return _lastPos;
            }
            set
            {
                if(_position==value)return;
                if (data.Shap == null)
                {
                    _position = value;
                    _lastPos = _position;
                   
                }
                else
                {
                    _position = value;
                    data.client.physics.tree.SetActive(data);

                }

            }
        }
        public Fixed2 forward
        {
            get
            {
                return Fixed2.Parse(_rotation);
            }
        }
        public Fixed Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (data.Shap==null)
                {
                    while(value<0){
                        value+=360;
                    }
                    _rotation = value % 360;
                    _lastRota = _rotation;
                }
                else
                {
                    _rotation = value % 360;
                   
                }
               
            }
        }
        public void Reset(Fixed2 position,Fixed rotation)
        {
            _position = position;
            _lastPos=position;
            _rotation = rotation;
        }
        public void PhysicsEffect()
        {
            if (data.Shap == null) return;
            if (data.isTrigger || !data.rigibody.CheckCollision(data))
            {
                if (_position != _lastPos || _rotation != _lastRota)
                {
                    _lastPos = _position;
                    _lastRota = _rotation;
                    data.Shap.ResetSize();

                    Tree4.Move(data);
                }
            }
            else
            {
                _rotation = _lastRota;
                _position = _lastPos;
            }
        }
    }
}