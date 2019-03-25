using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour {
//	public GameObject[] tileView;
	GridMap map;
	List<GameObject> viewObjs;
	public MapPrefabList mapPrefabList;
	
	
	
	
	
	[ContextMenu("ClearTile")]
	public void Clear(){
		
		if(viewObjs!=null&&viewObjs.Count>0){
			for (int i = 0; i < viewObjs.Count; i++)
			{
				DestroyImmediate(viewObjs[i]);
			}
			viewObjs.Clear();
		}else{
			viewObjs=new List<GameObject>();
		}
	}
	
	// Use this for initialization
	public void ShowMap(GridMap map){
		Clear();
		for (int x = 0; x < map.width; x++)
		{
			for (int y = 0; y < map.height; y++)
			{
				if(map[x,y].type==TileType.road){
					var view= TileView.Parse(map.GetNeighborEqualString(x,y,TileType.road));
					var prefab=mapPrefabList.GetPrefab(map[x,y].type,view.key);
					var obj=Instantiate(prefab,new Vector3(x,1,y),Quaternion.Euler(0,-view.rotation*90,0),transform);
					viewObjs.Add(obj);
					if(map.CreatTileCallBack!=null){
						var shapInfo=obj.GetComponent<ShapsInfo>();
						if(shapInfo!=null){
							var shaps=shapInfo.shaps;
							for (int i = 0; i < shaps.Length; i++)
							{
								map.CreatTileCallBack(obj.transform,shaps[i].points);
							}
						}
					}
				}
			}
		}

		foreach (var room in map.rooms)
		{
			for (int x =  room.left ;x <room.right; x++)
			{
				for (int y = room.down; y < room.up; y++)
				{
					if(map[x,y].type==TileType.room||map[x,y].type==TileType.canOpenDoor){
						var view= TileView.Parse(map.GetNeighborEqualString(x,y,TileType.room,TileType.canOpenDoor));
						var prefab=mapPrefabList.GetPrefab(TileType.room,view.key);
						viewObjs.Add(Instantiate(prefab,new Vector3(x,1,y),Quaternion.Euler(0,-view.rotation*90,0),transform));
						foreach (var dir in map[x,y].openDir)
						{
							var door=mapPrefabList.GetPrefab(TileType.room,"door");
							viewObjs.Add(Instantiate(door,new Vector3(x,1,y),Quaternion.Euler(0,-(int)dir*90,0),transform));
						}
					}
				}
			}
		}
		
	}
	private void OnDestroy() {
		Clear();
	}




}



public class TileView{
	public static string[] keys={"1000","1100","1110","1010","1111"};
	public string key;
	public int rotation;

	public static TileView Parse(string equal){
		TileView view=new TileView();
		foreach (var key in keys)
		{
			if(equal.Contains(key)){
				view.key=key;
				view.rotation=equal.IndexOf(key);
				return view;
			}
		}
		return view;
	}
}

public static class GripMapExtend{
	public static string GetNeighborEqualString(this GridMap map,int x, int y,params TileType[] types){
	
		string temp="";
		for (int i = 0; i < 4; i++)
		{
		
			if(map[x,y].openDir.Contains((TileDirection)i)){
			
				temp+="1";continue;
			}
			var node=map[x+GridMap.directions[i][0],y+GridMap.directions[i][1]];
		//	if(node!=null&&room!=null&&!room.ContainsTile(node)){temp+="0";continue;}
			if(node!=null&&ContainsType(node,types)){
				temp+="1";
			}else{
				temp+="0";
			}
		}
		
		temp+=temp;
		return temp;
	}

	public static bool ContainsType(Tile node , TileType[] types){
		foreach (var type in types)
		{
			if(node.type==type){
				return true;
			}
		}
		return false;
	}
}