using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace VoxelRender
{
	

	[System.Serializable]
	public class VoxelData{
		// 序列化数据
		public V3 size;

		// public string materialPath;
		// public string texturePath;
		public List<VoxelInfo> voxels=new List<VoxelInfo>();
		public List<ColorInfo> colors=new List<ColorInfo>();

		public void SetSceneScale(int x,int y,int z){
			size.x=x;
			size.y=y;
			size.z=z;
		}
		public void SetVoxel(int x,int y, int z,int index){
			var v=new VoxelInfo();
			v.pos.x=x;
			v.pos.y=y;
			v.pos.z=z;
			v.colorIndex=index;
			voxels.Add(v);
		}
		
		
	}
	public static class V3Extend{
		public static V3 ToV3(this Vector3 vector3){
			var v3=new V3();
			v3.x=(int)vector3.x;
			v3.y=(int)vector3.y;
			v3.z=(int)vector3.z;
			return v3;
		}
	}
	[System.Serializable]
	public struct V3{
		public int x;
		public int y;
		public int z;
		public V3(int init){
			x=init;
			y=init;
			z=init;
		}

		public V3 MixMin(V3 other){
			var v=new V3();
			v.x=Mathf.Min(x,other.x);
			v.y=Mathf.Min(y,other.y);
			v.z=Mathf.Min(z,other.z);
			return v;
		}
		public V3 MixMax(V3 other){
				var v=new V3();
			v.x=Mathf.Max(x,other.x);
			v.y=Mathf.Max(y,other.y);
			v.z=Mathf.Max(z,other.z);
			return v;
		}
		public Vector3 ToVector3(){
			return new Vector3(x,y,z);
		}

	}
	[System.Serializable]
	public struct ColorInfo{
		public float r;
		public float g;
		public float b;
		public float a;

		public Color ToColor(){
			return new Color(r,g,b,a);
		}
	}
	[System.Serializable]
	public struct VoxelInfo{
		public int colorIndex;
		public V3 pos;
		
	}
	

}