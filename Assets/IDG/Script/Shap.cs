using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG ;
namespace IDG
{
   
    public class ShapData:NetData{
        public void SetShap(Vector2[] points){
            var ps=new Fixed2[points.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i]=points[i].ToFixed2();
            } 
            Shap=new ShapBase( ps);
          //  Debug.LogError("shap count "+points.Length);
        }
        public override void Start()
        {
        
        
            rigibody.useCheck=false;
            base.Start();
        }
        protected override void FrameUpdate()
        {
        
            
    
        }

        public override string PrefabPath()
        {
            return  "Prefabs/ShapView";
        }
    }
    public class CircleShap : ShapBase
    {
        public CircleShap(Fixed r, int num)
        {

            Fixed t360 = new Fixed(360);
            Fixed tmp = t360 / num;
            Fixed2[] v2s = new Fixed2[num];
            int i = 0;
            for (Fixed tr = new Fixed(0); tr < t360 && i < num; tr += tmp, i++)
            {
                v2s[i] = Fixed2.Parse(tr) * r;
            }

            Points = v2s;
        }
    }
    public class BoxShap : ShapBase
    {

        public BoxShap(Fixed x, Fixed y)
        {
            Fixed2[] v2s = new Fixed2[4];
            v2s[0] = new Fixed2(x / 2, y / 2);
            v2s[1] = new Fixed2(-x / 2, y / 2);
            v2s[2] = new Fixed2(x / 2, -y / 2);
            v2s[3] = new Fixed2(-x / 2, -y / 2);
            Points = v2s;
        }
    }
    public class RayShap : ShapBase
    {
        public static RayShap GetRay(Fixed2 origin, Fixed2 direction,Fixed length)
        {
            var shap = new RayShap(direction.normalized*length);
            shap._position = origin;
            return shap;
        }
        public RayShap ResetDirection(Fixed2 origin, Fixed2 direction,Fixed length)
        {
            position = origin;
            _points[1] = direction * length;
            ResetSize();
            return this;
        }

        public RayShap(Fixed2 direction)
        {
            Fixed2[] v2s = new Fixed2[2];
            v2s[0] = Fixed2.zero;
            v2s[1] = direction;
            Points = v2s;
        }
    }
}