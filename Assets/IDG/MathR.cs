using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDG
{
    /// <summary>
    /// 定点数数学类
    /// </summary>
    class MathFixed
    {
        protected static int tabCount = 18*4;
        /// <summary>
        /// sin值对应表
        /// </summary>
        protected static readonly List<Fixed> _m_SinTab = new List<Fixed>();
        public static readonly Fixed PI = new Fixed(3.14159265f);
        protected static Fixed GetSinTab(Fixed r)
        {
            
            Fixed i =new Fixed( r.ToInt());
            //UnityEngine.Debug.Log(i.ToInt());
            if (i.ToInt() == _m_SinTab.Count-1)
            {
                return _m_SinTab[(int)i.ToInt()];
            }
            else
            {
               // UnityEngine.Debug.Log(i.ToInt()+":"+ _m_SinTab[i.ToInt()]+":"+ Ratio.Lerp(_m_SinTab[i.ToInt()], _m_SinTab[(i + 1).ToInt()], r - i));
                return Fixed.Lerp(_m_SinTab[(int)i.ToInt()], _m_SinTab[(int)(i+1).ToInt()], r - i);
            }
            
        }
        public static Fixed GetAsinTab(Fixed sin)
        {
            MathFixed math = Instance;
            //UnityEngine.Debug.Log("GetAsinTab");
            for (int i = _m_SinTab.Count-1; i >=0; i--)
            {
               
                if (sin > _m_SinTab[i])
                {
                    if (i == _m_SinTab.Count-1)
                    {
                        return new Fixed(i) / (tabCount / 4) * (PI / 2);
                    }
                    else
                    {
                        //return new Ratio(i);
                        return Fixed.Lerp(new Fixed(i), new Fixed(i + 1), (sin-_m_SinTab[i])/(_m_SinTab[i+1] - _m_SinTab[i])) / (tabCount / 4) * (PI / 2);
                    }
                }
            }
            return new Fixed();
        }
        protected static MathFixed Instance
        {
            get
            {
                if (_m_instance == null)
                {
                    _m_instance = new MathFixed();
                    
                }
                return _m_instance;
            }
        }
        protected static MathFixed _m_instance;
        protected MathFixed()
        {
            if (_m_instance == null)
            {
                
                _m_SinTab.Add(new Fixed(0f));//0
                _m_SinTab.Add(new Fixed(0.08715f));
                _m_SinTab.Add(new Fixed(0.17364f));
                _m_SinTab.Add(new Fixed(0.25881f));
                _m_SinTab.Add(new Fixed(0.34202f));//20
                _m_SinTab.Add(new Fixed(0.42261f));
                _m_SinTab.Add(new Fixed(0.5f));

                _m_SinTab.Add(new Fixed(0.57357f));//35
                _m_SinTab.Add(new Fixed(0.64278f));
                _m_SinTab.Add(new Fixed(0.70710f));
                _m_SinTab.Add(new Fixed(0.76604f));
                _m_SinTab.Add(new Fixed(0.81915f));//55
                _m_SinTab.Add(new Fixed(0.86602f));//60

                _m_SinTab.Add(new Fixed(0.90630f));
                _m_SinTab.Add(new Fixed(0.93969f));
                _m_SinTab.Add(new Fixed(0.96592f));
                _m_SinTab.Add(new Fixed(0.98480f));//80
                _m_SinTab.Add(new Fixed(0.99619f));

                _m_SinTab.Add(new Fixed(1f));
               
               
            }
        }
        public static Fixed PiToAngel(Fixed pi)
        {
            return pi / PI * 180;
        }
        public static Fixed Asin(Fixed sin)
        {
            if (sin < -1 || sin > 1) { return new Fixed(); }
            if (sin >= 0)
            {
                return GetAsinTab(sin);
            }
            else
            {
                return -GetAsinTab(-sin);
            }
        }
        public static Fixed Sin(Fixed r)
        {
           
            MathFixed math= Instance;
            //int tabCount = SinTab.Count*4;
            Fixed result=new Fixed();
            r = (r * tabCount / 2 / PI);
            //int n = r.ToInt();
            while (r < 0)
            {
                r += tabCount;
            }
            while (r > tabCount)
            {
                r -= tabCount;
            }
            if (r >= 0 && r <= tabCount / 4)                // 0 ~ PI/2
            {
                result = GetSinTab(r);
            }
            else if (r > tabCount / 4 && r < tabCount / 2)       // PI/2 ~ PI
            {
                r -= new Fixed(tabCount / 4);
                result = GetSinTab(new Fixed(tabCount / 4) - r);
            }
            else if (r >= tabCount / 2 && r < 3 * tabCount / 4)    // PI ~ 3/4*PI
            {
                r -= new Fixed(tabCount / 2);
                result = -GetSinTab(r);
            }
            else if (r >= 3 * tabCount / 4 && r < tabCount)      // 3/4*PI ~ 2*PI
            {
                r = new Fixed(tabCount) - r;
                result = -GetSinTab(r);
            }
            
            return result;
        }
        public static Fixed Abs(Fixed ratio)
        {
            return Fixed.Abs( ratio);
        }
        public static Fixed Sqrt(Fixed r)
        {
            return Fixed.Sqrt(r);
        }
        
        public static Fixed Cos(Fixed r)
        {
            return Sin(r + PI / 2);
        }
        public static Fixed SinAngle(Fixed angle)
        {
            return Sin(angle / 180 * PI);
        }
        public static Fixed CosAngle(Fixed angle)
        {
            return Cos(angle / 180 * PI);
        }
    }
}
