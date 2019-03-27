using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
// using System.IO;
using VoxelRender;

public class VoxelRenderManager{

	public static VoxelData GetData(){
		VoxelData voxel= new VoxelData();
		return voxel;
	}
	
	public static GameObject CreateViewObj(VoxelData voxelData,Material material,VoxelInfo voxelInfo,float gridSize,Transform parent){
		var obj=new GameObject("Voxel"+voxelInfo.pos.ToVector3());
		obj.transform.position=parent.position+(voxelInfo.pos.ToVector3()-voxelData.size.ToVector3()/2+voxelData.size.y*Vector3.up/2+Vector3.one/2)*gridSize;
		obj.transform.localScale=Vector3.one*gridSize;
		obj.transform.parent=parent;
		obj.hideFlags=HideFlags.HideInHierarchy;
		var mesh=CreateViewMesh(voxelInfo.colorIndex);
		obj.AddComponent<MeshFilter>().mesh=mesh;
		obj.AddComponent<MeshRenderer>().material=material;
		return obj;
	}
	public static Texture2D CreatTexture(VoxelData voxel){
		if(voxel.colors==null){
			return null;
		}
		var texture=new Texture2D(voxel.colors.Count,1,TextureFormat.RGBA32,false);
		var colors=new List<Color>();
		foreach (var color in voxel.colors)
		{
			colors.Add(color.ToColor());
		}
		texture.SetPixels(colors.ToArray());
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply(false);
		return texture;
	}
	
	public static float halfSize = 0.5f;
	public static Vector3[] vertexs = new Vector3[] {
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),	
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (halfSize, -halfSize, halfSize),
			};

	public static	int[] triangles = new int[] {
				0, 2, 3, 
				0, 3, 1, 
				8, 4, 5, 
				8, 5, 9, 
				10, 6, 7, 
				10, 7, 11, 
				12, 13, 14, 
				12, 14, 15, 
				16, 17, 18, 
				16, 18, 19, 
				20, 21, 22, 
				20, 22, 23, 
			};

	public static Mesh CreateViewMesh(int colorIndex){
		var uv= new Vector2((colorIndex ) / 256f, 0.5f);
		Vector2[] uvs = new Vector2[] {
			uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv,uv
		};
		Mesh mesh = new Mesh ();
		mesh.vertices = vertexs;
		mesh.uv = uvs;
		mesh.colors = null;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();	
		return mesh;
	}
	public static Material CreateMaterial(VoxelData voxel,Texture2D tex){
		var material=new Material (Shader.Find ("Diffuse"));
		material.mainTexture=tex;
		return material;
	}
	public static VoxelData Parse(Dictionary<V3,Color> VoxelColors){
		VoxelData voxel=GetData();
		var colors=new Dictionary<Color,int>();
		var minPos=new V3(int.MaxValue);
		var maxPos=new V3(int.MinValue);
		foreach (var vc in VoxelColors)
		{
			
				minPos=minPos.MixMin(vc.Key);
				maxPos=maxPos.MixMax(vc.Key);
			
			
		}
		voxel.SetSceneScale(maxPos.x-minPos.x,maxPos.y-minPos.y,maxPos.z-minPos.z);
		foreach (var vc in VoxelColors)
		{
			var colorIndex=-1;
		
			if(colors.ContainsKey(vc.Value)){
				colorIndex=colors[vc.Value];
			}else
			{
				colorIndex=colors.Count;
				colors.Add(vc.Value,colorIndex);
			}

			
			voxel.SetVoxel(vc.Key.x-minPos.x,vc.Key.y-minPos.y,vc.Key.z-minPos.z,colorIndex);
		}
		foreach (var ci in colors)
		{
			var color= new ColorInfo();
			color.r=ci.Key.r;
			color.g=ci.Key.g;
			color.b=ci.Key.b;
			color.a=ci.Key.a;
			voxel.colors[ci.Value]=color;
		}
		return voxel;
	}
}
public class IDGData{
	public static void SerialData(string filePath,object obj)
     {
     	using (System.IO.FileStream fs = System.IO.File.Create(filePath))
		{
			BinaryFormatter bf = new BinaryFormatter();
			 bf.Serialize(fs, obj);
   		}
	}
	public static T DeserialData<T>(string filePath)where T:class
	{
		using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open))
		{
			BinaryFormatter bf = new BinaryFormatter();
			var obj = bf.Deserialize(fs);
			fs.Close();
			return obj as T;
		}
	}
}