using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RandomMapSetting",menuName = "随机地图生成/生成设置文件")]
public class RandomMapSetting : ScriptableObject {
	public GameObject nullPrefab;
	public ViewList[] viewLists;
	
	[ContextMenu("AutoSetKey")]
	public void AutoSetKey(){
		foreach (var viewList in viewLists)
		{
			for (int i = 0; i < viewList.prefabs.Length; i++)
			{
				foreach (var key in TileView.keys)
				{
					if(viewList.prefabs[i].prefab.name.Contains(key)){
						viewList.prefabs[i].key=key;
						break;
					}
				}
			}
		}
	
	}
	public GameObject GetPrefab(TileType type,string key){
		var prefab= GetList(type).GetPrefab(key);
		if(prefab==null){
			prefab=nullPrefab;
		}
		return prefab;
	}
	protected ViewList GetList(TileType type){
		foreach (var item in viewLists)
		{
			if(type==item.type){
				return item;
			}
		}
		return new ViewList();
	}
}

