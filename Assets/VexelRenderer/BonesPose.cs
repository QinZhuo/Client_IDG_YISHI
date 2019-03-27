using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BonesPose : MonoBehaviour {
	Transform[] bones;
	public GameObject bonesPrefabs;

	public List<GameObject> showList;
	// Use this for initialization
	[ContextMenu("ShowBones")]
	public void ShowBones(){
		bones=GetComponentsInChildren<Transform>();
		for (int i = 0; i < bones.Length; i++)
		{
			showList.Add(Instantiate(bonesPrefabs,bones[i].position,bones[i].rotation,bones[i].transform));
		}
	}
	[ContextMenu("BindAll")]
	public void BindAll(){
		var renders=GetComponentsInChildren<VoxelRender.VoxelRenderer>();
		foreach (var item in renders)
		{
		
			item.transform.parent=transform.parent;
		}

	}
	[ContextMenu("ClearShow")]
	public void ClearShow(){
		foreach (var item in showList)
		{
			DestroyImmediate(item);
		}
		showList.Clear();
	}
	private void Update() {
		if(Input.GetKeyDown(KeyCode.Return)){
			Debug.Log("enter");
		}
	}
}
