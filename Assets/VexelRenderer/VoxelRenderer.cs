using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VoxelRender
{
	
public class VoxelRenderer : MonoBehaviour {

	public Transform boneRoot;
	public Transform renderRoot;

	public Transform[] voxels;
	public Vector3[] bindPose;
	public int[] bindIndex;
	public List<Transform> bones;
	public float GridSize=0.05f;
	public float minBoneLen=0.15f;

	public Material material;
    public VoxelData voxelData;
	public bool _showWeight;
	public bool ShowVoxelWeight{
		set{
			_showWeight=value;
			renderRoot.gameObject.SetActive(!_showWeight);
		}get{
			return _showWeight;
		}
	}
	private void Start() {
		InitRender();
	}
	

	private void OnDrawGizmosSelected() {
		if(_showWeight){
			for (int i = 0; i < bindIndex.Length; i++)
			{
				
				var boneIndex=bindIndex[i];
				Gizmos.color=Color.HSVToRGB(boneIndex*1f/ bones.Count,0.7f,1);
				Gizmos.DrawCube(voxels[i].position,Vector3.one*GridSize);
			}
		}
	}

	public void InitRender(){
		ClearVoxel(transform.Find("renderer"));
		if(renderRoot==null){
			renderRoot=new GameObject("renderer").transform;
			renderRoot.parent=transform;
			renderRoot.localPosition=Vector3.zero;
		}
		int i=0;
		voxels=new Transform[voxelData.voxels.Count];
		foreach (var v in voxelData.voxels)
		{
			voxels[i]=VoxelRenderManager.CreateViewObj(voxelData,material,v,GridSize,renderRoot).transform;
			i++;
		}	
	}
	public void ClearVoxel(Transform transform){
		if(transform==null)return;
		if(voxels!=null)
		foreach (var v in voxels)
		{
			if(v!=null)DestroyImmediate(v.gameObject);
		}
		var child=new List<Transform>();
		child.AddRange(transform.GetComponentsInChildren<Transform>());
		child.Remove(transform);
		foreach (var v in child)
		{
			if(v!=null)DestroyImmediate(v.gameObject);
		}
		voxels=null;
	}
	public static float DisPoint2Line(Vector3 point,Vector3 linePoint1,Vector3 linePoint2)
    {
	
        Vector3 vec1 = point - linePoint1;
        Vector3 vec2 = linePoint2 - linePoint1;
        Vector3 vecProj = Vector3.Project(vec1, vec2);
        float dis =  Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
        return dis;
		
    }

	public float Project(Vector3 d1,Vector3 d2){
		return (Vector3.Angle(d1, d2)>=90?1:-1)*Vector3.Project(d1,d2).magnitude;
	}
	public Vector3 Distance(Vector3 point,Transform origin){
		
		return new Vector3(
			Project(point-origin.transform.position,origin.transform.right),
			Project(point-origin.transform.position,origin.transform.up),
			Project(point-origin.transform.position,origin.transform.forward)
		);
	
	}
	public Vector3[] Bind(Transform[] points,int[] bindIndex){
		var bindPose=new Vector3[points.Length];
		
		for (int i = 0; i < bindPose.Length; i++)
		{
			var bone=bones[bindIndex[i]];
			
			Debug.Log(points[i].name+" "+points[i].transform.position+" and "+bone.name+" "+bone.position+" distance "+Distance(points[i].transform.position,bone));
			bindPose[i]=Distance(points[i].transform.position,bone);
		}
		return bindPose;
	}

	public void GetBones(){
		bones=GetBones(boneRoot);
	}
	 List<Transform> GetBones(Transform bondRoot){
		List<Transform> bs=new List<Transform>();
		if(bondRoot==null)return null;
		bs.Add(bondRoot);
		for (int i = 0; i < bondRoot.childCount; i++)
		{
			var childRoot= bondRoot.GetChild(i);
			if(bondRoot.childCount>3){
				if(Vector3.Distance(childRoot.position,bondRoot.position)<minBoneLen){
					continue;
				}
			}
			bs.AddRange(GetBones(childRoot));
		}
		return bs;
	}
	[ContextMenu("bind")]
	public void Bind(){
		
		bindIndex=new int[voxels.Length];
		List<Transform> voxelList=new List<Transform>();
		voxelList.AddRange(voxels);
		for (int i = 0; i < voxelList.Count; i++)
		{
			var voxel=voxelList[i];
			float minDis=float.MaxValue;
			bindIndex[i]=0;
			for (int j = 0; j < bones.Count; j++)
			{
				var bone=bones[j];
				//if(Vector3.Angle (bone.forward,voxel.position-bone.position)<90){
					if(minDis>Vector3.Distance(bones[j].position,voxelList[i].position)){
						minDis=Vector3.Distance(bones[j].position,voxelList[i].position);
						bindIndex[i]=j;
					
					}
				//}
			}
		}
		
		bindPose= Bind(voxels,bindIndex);
		Debug.Log("BindOver");
	}
	
	
	
	public void RenderVoxel(Transform voxel,Transform bone,Vector3 bindPose){
		voxel.transform.rotation=Quaternion.identity;
		voxel.transform.position=VoxelRenderManager.Fixed(bone.transform.position-( bone.right*bindPose.x+bone.forward*bindPose.z+bone.up*bindPose.y),GridSize);
	}

	 private void LateUpdate() {
		if(voxels!=null&&boneRoot!=null&&bindPose!=null){
			for (int i = 0; i < bindPose.Length; i++)
			{
				RenderVoxel(voxels[i].transform,bones[bindIndex[i]],bindPose[i]);
			}
		}

	}
	
}
}
