using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapPrefabList",menuName = "随机地图生成/生成预制体配置文件")]
public class MapPrefabList : ScriptableObject {
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
			if(key!="door"){
				Debug.LogWarning("类型"+type+"中不存在的key["+key+"]");
			}
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
		Debug.LogWarning("不存在的TileType["+type+"]");
		return new ViewList();
	}
}

[System.Serializable]
public struct ViewList
{
	public TileType type;
	public ViewPrefab[] prefabs;
	public GameObject GetPrefab(string key){
		foreach (var item in prefabs)
		{
			if(item.key==key){
				return item.prefab;
			}
		}
		return null;
	}
}
[System.Serializable]
public struct ViewPrefab{
	public string key;
    public GameObject prefab;
}