using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace VoxelRender
{
	

[CustomEditor(typeof(VoxelRenderer))]
public class VoxelRendererEditor : Editor{
    VoxelRenderer _renderer;
    VoxelRenderer renderer{
        get{
            if(_renderer==null){
                _renderer= target as VoxelRenderer;
            }
            return _renderer;
        }
    }
    VoxelData voxelData{
        get{
            return renderer.voxelData;
        }
        set{
            renderer.voxelData=value;
        }
    }
	float defaultBoneCircle=0.05f;
	int selectId=0;
	
	public string[] selectNames={"渲染信息","编辑数据"};
    public override void OnInspectorGUI(){
       
       
       
		selectId  = GUILayout.Toolbar(selectId, selectNames);
	
		if(selectId==1){
			renderer.GridSize=EditorGUILayout.FloatField("体素渲染大小",renderer.GridSize);
			renderer.boneRoot=EditorGUILayout.ObjectField("指定根骨骼",renderer.boneRoot,typeof(Transform))as Transform;
			if(GUILayout.Button("加载.vox文件")){
				LoadVoxFileWindows();
			}
			if(renderer.boneRoot!=null){
				renderer.minBoneLen=EditorGUILayout.FloatField("骨骼检索最小长度",renderer.minBoneLen);
				if(GUILayout.Button("获取骨骼")){
					renderer.GetBones();
				}
				if(GUILayout.Button("绑定骨骼")){
					renderer.Bind();
				}
			
				if(GUILayout.Button(renderer.ShowVoxelWeight?"关闭权重显示":"打开权重显示")){
					renderer.ShowVoxelWeight=!renderer.ShowVoxelWeight;
				}
				if(GUILayout.Button("旋转渲染器")){
					renderer.renderRoot.Rotate(new Vector3(0,90,0));
				}
			}
		}else
		{	
			if(renderer.ShowVoxelWeight){
				renderer.ShowVoxelWeight=false;
			}
			GUILayout.Label("体素渲染大小 : "+(renderer.GridSize));
			GUILayout.Label("体素数目 : "+(voxelData.voxels.Count));
			GUILayout.Label("是否绑骨 : "+(renderer.boneRoot!=null));
			
		}
        
    }
	private void OnSceneGUI() {
		if(selectId==1){
			Handles.color=Color.white;
		
			DrawBoneCircle();
		}
	}
	

	void DrawBoneCircle(){
		if(renderer.bones==null)return;
		for (int i = 0; i < renderer.bones.Count; i++)
		{
			if(renderer.bones.Contains(renderer.bones[i].parent)){
				Handles.color=Color.white;
				Handles.DrawLine(renderer.bones[i].position,renderer.bones[i].parent.position);
			}
			
			Handles.color=Color.HSVToRGB(i*1f/ renderer.bones.Count,0.7f,1);
			var bone=renderer.bones[i];
			Handles.RadiusHandle(Quaternion.identity,bone.position,defaultBoneCircle);
			
		}
	
	}
		

	
    void CreateRenderData(){
        var texturePath="Assets/Model/VoxelModelData/"+renderer.name+"_vr.png";
		var tex=SavePng(VoxelRenderManager.CreatTexture(voxelData),texturePath);

        var materialPath="Assets/Model/VoxelModelData/"+renderer.name+"_vr.mat";
		renderer.material =SaveMat(VoxelRenderManager.CreateMaterial(voxelData,tex),materialPath);
		
        var maxVoxelCount=5000;
		if(voxelData.voxels.Count>maxVoxelCount){
			Debug.LogError("【VoxelRenderer】体素数目过大 "+voxelData.voxels.Count+"超过"+maxVoxelCount+" 中止生成显示块");
			return;
	    }
        SaveVoxelData();
    }

    void SaveVoxelData(){
        IDGData.SerialData("Assets/Model/VoxelModelData/"+renderer.name+".voxData",voxelData);
    }
    void LoadVoxelData(){
        voxelData=IDGData.DeserialData<VoxelData>("Assets/Model/VoxelModelData/"+renderer.name+".voxData");
    }
    void LoadMatrial(){
         renderer.material=AssetDatabase.LoadAssetAtPath("Assets/Model/VoxelModelData/"+renderer.name+"_vr.mat", typeof(Material)) as Material;
    }
    void LoadVoxFileWindows(){
		string path = UnityEditor.EditorUtility.OpenFilePanel(
				"导入.vox格式体素模型",
				"Assets/MagicaVoxel/Vox",
				"vox"
			);
		voxelData= VoxImporter.LoadVoxFile(path);
        CreateRenderData();
        renderer.InitRender();
	}
    Texture2D SavePng(Texture2D pngTex,string path){
        byte[] dataBytes = pngTex.EncodeToPNG();
        System.IO.FileStream fs = System.IO.File.Open(path,  System.IO.FileMode.OpenOrCreate);
        fs.Write(dataBytes, 0, dataBytes.Length);
        fs.Flush();
        fs.Close();
		AssetDatabase.Refresh();
		Texture2D newTex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		return newTex;
	}
	Material SaveMat(Material mat,string path){

		
		AssetDatabase.CreateAsset(mat,path);//"Assets/voxel.mat"
        AssetDatabase.Refresh();
		var newMat= AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
		return newMat;
	}

    public V3 Vector3ToV3(Vector3 vector3,float GridSize){
		var v=VoxelRenderManager.Fixed(vector3,GridSize);
		var v3=new V3();
		v3.x=(int)(v.x/GridSize+0.5f);
		v3.y=(int)(v.y/GridSize+0.5f);
		v3.z=(int)(v.z/GridSize+0.5f);
		return v3;
	}

	Texture2D newText=null;
	public void ParseMeshRendererToVoxel(float GridSize){

		var mesh=MeshMerge();
		
		if(mesh==null)return;
		var vertices=mesh.vertices;
		var normals=mesh.normals;
		Dictionary<V3,Color> colors=new Dictionary<V3, Color>();
		for (int i = 0; i < vertices.Length; i++)
		{

			var v=Vector3ToV3(vertices[i]- normals[i]*GridSize/2,GridSize);
			if(!colors.ContainsKey(v)){
				var uv=new int[]{(int)(mesh.uv[i].x*newText.width),(int)(mesh.uv[i].y*newText.height)};
				colors.Add(v,newText.GetPixel(uv[0],uv[1]));
			}
		}
		voxelData=VoxelRenderManager.Parse(colors);
	}
	public void TextureMerge(Material[] materials,CombineInstance[] combineInstances){
		List<Texture2D> Textures = new List<Texture2D>();
		for (int i = 0; i < materials.Length; i++)
		{
			Textures.Add(materials[i].GetTexture("_MainTex") as Texture2D);
		}

		newText = new Texture2D(512, 512, TextureFormat.RGBA32, true);
		Rect[] uvs = newText.PackTextures(Textures.ToArray(), 0);
	

		// reset uv
		Vector2[] uva, uvb;
		for (int j = 0; j < combineInstances.Length; j++)
		{
			uva = (Vector2[])(combineInstances[j].mesh.uv);
			uvb = new Vector2[uva.Length];
			for (int k = 0; k < uva.Length; k++)
			{
				uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
			}
			combineInstances[j].mesh.uv = uvb;
		}
	}
	public Mesh MeshMerge(){
		var skinedMeshs=renderer.GetComponentsInChildren<SkinnedMeshRenderer>();
		var meshfilters=renderer.GetComponentsInChildren<MeshFilter>();
		var mats=new List<Material>();
		CombineInstance[] combine=new CombineInstance[skinedMeshs.Length+meshfilters.Length];
		if(combine.Length<=0)return null;
		int index=0;
		Debug.Log("Start MergeMesh ...");
		
		foreach (var m in skinedMeshs)
		{
			combine[index].mesh=m.sharedMesh;
			//combine[index].subMeshIndex=index;
			combine[index].transform=m.transform.localToWorldMatrix;
			mats.AddRange(m.sharedMaterials);
			index++;
			Debug.Log("Mesh["+index+"] vertexCount "+m.sharedMesh.vertexCount);
		}
		foreach (var m in meshfilters)
		{
			combine[index].mesh=m.sharedMesh;
			//combine[index].subMeshIndex=index;
			combine[index].transform=m.transform.localToWorldMatrix;
			var r= m.GetComponent<MeshRenderer>();
			mats.Add(r.sharedMaterial);
			Debug.Log("Mesh["+index+"] vertexCount "+m.sharedMesh.vertexCount);
			index++;
		}
		Debug.Log("combine mesh count is "+combine.Length);
		for (int i = 0; i < renderer.transform.childCount; i++)
		{
			DestroyImmediate(renderer.transform.GetChild(i).gameObject);
		}
		TextureMerge(mats.ToArray(),combine);

		var newMesh=new Mesh();
		newMesh.CombineMeshes(combine);
		
		var newMaterial = new Material (Shader.Find ("Mobile/Diffuse"));
		newMaterial.mainTexture=newText;
		var obj= new GameObject("合成比对view");
		var ren= obj.AddComponent<MeshRenderer>();
		var filter=obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshCollider>().sharedMesh=newMesh;
		ren.material=newMaterial;
		filter.sharedMesh=newMesh;
		obj.transform.position=renderer.transform.position;
		Debug.Log("End MergeMesh vertexCount "+newMesh.vertexCount);
		return newMesh;
	}
}
}