using System.Net.Sockets;
using System;
using System.Linq;
namespace IDG {
    public class Random
    {
        ulong seed;
        public Random(ulong seed){
            this.seed=seed;
        }

        public int next(){
           seed=seed*214013L;
           return (int)((seed>>16)&0x7fff);
        }

        public int Range(int max){
            return next()%max;
        }

         public int Range(int min, int max){
            return min + Range(max - min);
        }
        public Fixed RangeFixed()
        {
            return Range(0, 10000).ToFixed() / 10000;
        }
        public Fixed RangeFixed(Fixed max)
        {
            return RangeFixed() * max;
        }
        public Fixed RangeFixed(Fixed min,Fixed max)
        {
            return min + RangeFixed(max - min);
        }
    }
}