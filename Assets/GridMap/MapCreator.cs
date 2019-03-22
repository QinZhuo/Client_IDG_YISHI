using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MapCreator : MonoBehaviour {
	
	public MapView mapView;
	protected GridMap map;
	// Use this for initialization
	private void Awake() {
		mapView=GetComponent<MapView>();
	}
	[ContextMenu("randomMap")]
	public void RandomMap () {
		map=new GridMap();
		map.Init(30,30);
		map.RandomRoom();
		map.PrimConnect();
		//map.RandomConnect(2);
		mapView.ShowMap(map);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmos() {
		if(map!=null){
			//map.GizmoShow();
		}
	
	}
}





public class GridMap
{
	public Tile this[int x,int y]{
		get{
			if(InMap(x,y)){
				return this.nodes[x,y];
			}else{
				return null;
			}
		}
	}
	public Tile[,] nodes;
	public static int[][] directions;
	public int width;
	public int height;
	public Func<int,int,int> RandomRange;
	public List<Room> rooms;
	public Color[] colors;
	public void Init(int width,int height,Func<int,int,int> randomRangeFuc=null){
		this.height=(height/2)*2+1;
		this.width=(width/2)*2+1;
		if(randomRangeFuc!=null){
			RandomRange=randomRangeFuc;
		}else
		{
			RandomRange=UnityEngine.Random.Range;
		}
		nodes=new Tile[this.width,this.height];
		directions=new int[4][];
		directions[0]=new int[]{1,0};
		directions[1]=new int[]{0,1};
		directions[2]=new int[]{-1,0};
		directions[3]=new int[]{0,-1};
		rooms=new List<Room>();
		colors=new Color[10];
		
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i]=ColorTool.RandomColorHSV();
		}
		for (int x = 0; x < this.width; x+=1)
		{
			for (int y = 0; y < this.height; y+=1)
			{
				nodes[x,y]=new Tile().Init(x,y);
			}
		}
		
	}
	public void RandomRoom(){
		for (int i = 0; i <10; i++)
		{
			int checkNum=0;
			while(!AddRoom(Room.RandomRoom(10,this))&&checkNum<20){checkNum++;};
		}
	}
	
	public bool AddRoom(Room room){
		if(!room.Check())return false;
	
		for (int x = room.left; x < room.right; x++)
		{
			for (int y = room.down; y < room.up;y++)
			{
				nodes[x,y].type=TileType.room;
			}
		}
		room.SetCanOpenDoor();
		rooms.Add(room);
		return true;
	}
	// public void FindDoor(){
	// 	foreach (var room in rooms)
	// 	{
	// 		room.FindDoor();
	// 	}
	// }
	public void GizmoShow(){
		for (int x = 0; x < width; x+=1)
		{
			for (int y = 0; y < height; y+=1)
			{
				if(nodes[x,y].type!=TileType.none){
					Gizmos.color=colors[(int)nodes[x,y].type];
					Gizmos.DrawCube(new Vector3(x,0,y),Vector3.one);
				}
				// if(nodes[x,y].road!=null){
				
				// }
			}
		}
		
		
	}
	public bool InMap(int x,int y){
		if(x<width&&x>=0&&y<height&&y>=0){
			return true;
		}
		return false;
	}

	public List<Tile> GetNeib(Tile node,params TileType[] types){
		List<Tile> nodeList=new List<Tile>();
		if(types.Length==0)types=new TileType[]{TileType.none};
		foreach (var d in directions)
		{
			if(InMap(node.x+d[0],node.y+d[1])){
				var otherNode=nodes[node.x+d[0],node.y+d[1]];
				foreach (var t in types)
				{
					if(otherNode.type==t){
						nodeList.Add(otherNode);
						break;
					}
				}
			}
		}
		return nodeList;
	}
	// public List<Tile> GetNeib2Size(Tile node,params TileType[] types){
	// 	List<Tile> nodeList=new List<Tile>();
	// 	if(types.Length==0)types=new TileType[]{TileType.none};
	// 	foreach (var d in directions)
	// 	{
	// 		if(InMap(node.x+d[0]*2,node.y+d[1]*2)){
	// 			var otherNode=nodes[node.x+d[0]*2,node.y+d[1]*2];
	// 			var road=nodes[node.x+d[0]*1,node.y+d[1]*1];
	// 			foreach (var t in types)
	// 			{
	// 				if(otherNode.type==t){
	// 					otherNode.road=road;
	// 					nodeList.Add(otherNode);
	// 					break;
	// 				}
	// 			}
				
	// 		}
	// 	}
	// 	return nodeList;
	// }
	public void PrimConnect(){
		foreach (var room in rooms)
		{
			room.InitDistance(rooms);
		}
		List<Room> connectedroom=new List<Room>();
		List<Room> otherRoom=new List<Room>();
		otherRoom.AddRange(rooms);
		var tRoom=otherRoom[0];
		otherRoom.Remove(tRoom);
		connectedroom.Add(tRoom);
		while (otherRoom.Count>0)
		{
		    var minRooms= GetMinDistanceRoom(connectedroom,otherRoom);
			if(minRooms==null){
				Debug.LogError("获取最小距离房间出错");
				break;
			}
			minRooms[0].Connect(minRooms[1]);
			otherRoom.Remove(minRooms[1]);
			connectedroom.Add(minRooms[1]);
		}
		

	}

	public void RandomConnect(int num){
		for (int i = 0; i < num; i++)
		{
			var room1=rooms[RandomRange(0,rooms.Count-1)];
			var room2=rooms[RandomRange(0,rooms.Count-1)];
			while(rooms.Count>2&&room1==room2){
				room2=rooms[RandomRange(0,rooms.Count-1)];
			}
			room1.Connect(room2);
		}
		
	}
	public Room[] GetMinDistanceRoom(List<Room> rooms1,List<Room> rooms2){
		Room[] minRooms=new Room[2];
		int minLen=int.MaxValue;

		// for (int i = 0; i < rooms1.Count; i++)
		// {
		// 	for (int j = 0; j < rooms2.Count; j++)
		// 	{
		// 		var room=rooms1[i];
		// 		var key=rooms2[j];
				
		// 	}
		// }
		foreach (var room in rooms1)
		{
			foreach (var key in rooms2)
			{
				if(minLen>room.distance[key]){
					minLen=room.distance[key];
					minRooms[0]=room;
					minRooms[1]=key;
				}
			}
		}
		if(minRooms[0]==null||minRooms[1]==null){
			Debug.LogError("min distance"+minLen);
			return null;
		}
		return minRooms;
	}
	// public int[] GetNullPoint(){
	// 	for (int x = 0; x < this.width; x+=2)
	// 	{
	// 		for (int y = 0; y < this.height; y+=2)
	// 		{
	// 			if(nodes[x,y].type==0){
	// 				return new int[]{x,y};
	// 			}
	// 		}
	// 	}
	// 	return null;
	// }
	// public void PrimMaze(){
	// 	int[] startPos=GetNullPoint();
	// 	if(startPos==null)return;
	// 	Tile cur=null;
	// 	List<Tile> roads=new List<Tile>();
	// 	roads.Add(nodes[startPos[0],startPos[1]]);
	// 	while(roads.Count>0){
	// 		cur=roads[Random.Range(0,roads.Count)];
	// 		roads.Remove(cur);
	// 		cur.type=TileType.road;
	// 		if(cur.road!=null){
	// 			cur.road.type=TileType.road;
	// 		}
	// 		roads.AddRange(GetNeib2Size(cur));
	// 	}
	// 	PrimMaze();
	// }



	// public void Fix(){
	// 	List<Tile> fixNodes=new List<Tile>();
	// 	for (int x = 0; x < this.width; x+=1)
	// 	{
	// 		for (int y = 0; y < this.height; y+=1)
	// 		{
	// 			if(this[x,y].type!=TileType.road)continue;
	// 			var neibs=GetNeib(this[x,y],TileType.road);
	// 			if(neibs.Count<=1&&!this[x,y].haveDoor){
	// 				//Debug.Log("["+x+","+y+"]"+neibs.Count+":"+neibs[0].type);
	// 				this[x,y].type=TileType.none;
	// 				if(neibs.Count==1&&neibs[0].type==TileType.road){
	// 					fixNodes.Add(neibs[0]);
	// 				}
	// 			}
	// 		}
	// 	}
	// 	List<Tile> tempNodes=new List<Tile>();
	// 	while (fixNodes.Count>0)
	// 	{
			
	// 		foreach (var item in fixNodes)
	// 		{
	// 			var neibs=GetNeib(item,TileType.road);
	// 			if(neibs.Count<=1&&!item.haveDoor){
	// 				//Debug.Log("["+x+","+y+"]"+neibs.Count+":"+neibs[0].type);
	// 				item.type=TileType.none;
	// 				if(neibs.Count==1&&neibs[0].type==TileType.road){
	// 					tempNodes.Add(neibs[0]);
	// 				}
	// 			}
				
	// 		}
	// 		fixNodes.Clear();
	// 		fixNodes.AddRange(tempNodes);
	// 		tempNodes.Clear();
	// 	}
	// }
}


public class Room
{
	public GridMap map;
	public  int x;
	public  int y;
	public int width;
	public int height;

	public int left{get{return x;}}
	public int right{get{return x+width;}}
	public int up{get{return y+height;}}
	public int down{get{return y;}}

	public List<Tile> canOpenDoor;
	public Dictionary<Room,int> distance;

	public int Distance(Room other){
		return Mathf.Abs(other.x-x)+Mathf.Abs(other.y-y);
		
	}
	public void InitDistance(List<Room> others){
		distance=new Dictionary<Room, int>();

		for (int i = 0; i < others.Count; i++)
		{
			if(others[i]==this)continue;
			distance.Add(others[i],Distance(others[i]));
		}
	
	}

	public void Connect(Room other){
		
		//Connect(map[x,y],other.map[other.x,other.y]);
		int count=0;
		while(!FindRoad(RandomCanOpenDoor(),other.RandomCanOpenDoor())){
			count++;
			if(count>10){
				Debug.LogError("连通地图失败！！！");
			}
		}
		
	}
	// protected void Connect(Tile a,Tile b){

	// 	var d=a.GetDirection(b);

	// 	var x=0;
	// 	for (x = a.x; ; x+=d[0])
	// 	{
	// 		if(map[x,a.y]==null)break;
	// 		if(map[x,a.y].type!=TileType.room)
	// 		map[x,a.y].type=TileType.road;
			
	// 		if(x == b.x)break;
	// 	}
	// 	for (int y = a.y; ; y+=d[1])
	// 	{
	// 		if(map[x,y]==null)break;
	// 		if(map[x,y].type!=TileType.room)
	// 		map[x,y].type=TileType.road;
	// 		if(y==b.y)break;
	// 	}
	// }

	public Tile RandomCanOpenDoor(){
		return canOpenDoor[map.RandomRange(0,canOpenDoor.Count-1)];
	}
	public bool FindRoad(Tile a,Tile b){
	
		var nodes= new List<Tile>();
		nodes.Add(a);
		var tNodes=new List<Tile>();
		var finded=new List<Tile>();
		bool success=false;
		int count=0;
		while (nodes.Count>0)
		{
			if(count>100){Debug.LogError("fiand Error"); return false;}
			tNodes.Clear();
			for (int i = 0; i < nodes.Count; i++)
			{
				var roadlist=map.GetNeib(nodes[i],TileType.none,TileType.canOpenDoor,TileType.road);
				
				for (int j = 0; j < roadlist.Count; j++)
				{
					if(!finded.Contains(roadlist[j])){
						if(roadlist[j].type!=TileType.canOpenDoor||roadlist[j]==b){
							roadlist[j].road=nodes[i];
							finded.Add(roadlist[j]);
							tNodes.Add(roadlist[j]);
						}
					}
				}
			}
			if(tNodes.Contains(b)){success=true; break;}
		
			nodes.Clear();
			nodes.AddRange(tNodes);
			count++;
		}
		if(success){
			b.OpenDoor(b.road);
			var node=b.road;
			while(node.road!=a&&node!=null){
				node.type=TileType.road;
				node=node.road;
			}
			node.type=TileType.road;
			node.OpenDoor(node.road);
		}
		return success;
	}
	public Room Init(int left,int down,int width,int height,GridMap map){
		x=left;
		y=down;
		this.width=width;
		this.height=height;
		this.map=map;
		canOpenDoor=new List<Tile>();
		return this;
	}
	public static Room RandomRoom(int size,GridMap map){
		var r=new Room().Init(map.RandomRange(1,map.width-size-1),
								map.RandomRange(1,map.height-size-1),
								map.RandomRange(size/2,size),
								map.RandomRange(size/2,size),
								map
								);
		
		return r;
	}

	public bool Check(){
		for (int i = this.x-1; i < this.x+width+1; i++)
		{
			for (int j = this.y-1; j < this.y+height+1; j++)
			{
				if(map[i,j]==null||map.nodes[i,j].type!=TileType.none){
					return false;
				}
			}	
		}
		return true;
	}

	
	public void SetCanOpenDoor(){
		for (int i = left; i < right; i++)
		{
			for (int j = down; j < up; j++)
			{
				if(i==left||i==right-1||j==down||j==up-1){
					//if(Contains(i,j)){
						map[i,j].type=TileType.canOpenDoor;
						canOpenDoor.Add(map[i,j]);
					//}
					
				}
			}
		}
		//map[left,down].type=TileType.canOpenDoor;
	}

	// public void FindDoor(){
	// 	List<Door> doors=new List<Door>();
	// 	for (int i = this.x; i < right; i++)
	// 	{
	// 		for (int j = this.y; j <up; j++)
	// 		{
	// 			if(i==x||j==y||i==right-1||j==up-1){
	// 				foreach (var dir in GridMap.directions)
	// 				{
	// 					if(IsCanOpenDoor(i,j,dir)){
	// 						doors.Add(new Door().Init(map[i,j],map[i+dir[0],j+dir[1]]));
	// 					}
					

	// 				}
	// 			}
	// 		}	
	// 	}
	// 	for (int i = 0; i < Random.Range(1,3); i++)
	// 	{
	// 		if(doors.Count>0){
	// 			doors.Remove(doors[Random.Range(0,doors.Count)].Open());
	// 		}

	// 	}
		
	// }
	
	public bool ContainsTile(Tile tile){
		return Contains(tile.x,tile.y);
	}
	// public bool IsCanOpenDoor(int x,int y,int[] dir){
	// 	int dx=x+dir[0];
	// 	int dy=y+dir[1];
	// 	int dx2=x+dir[0]*2;
	// 	int dy2=y+dir[1]*2;
	// 	if(!map[x,y].roomFind&&map[dx,dy]!=null&& map[dx,dy].type!=TileType.none&&!map[dx,dy].roomFind){
	// 		if(!Contains(dx,dy)){
	// 			return true;
	// 		}
	// 	}else if(!map[x,y].roomFind&&map[dx2,dy2]!=null&&map[dx2,dy2].type==TileType.road&&!map[dx2,dy2].roomFind)
	// 	{
	// 		if(!(Contains(dx2,dy2))){
	// 			return true;
	// 		}
	// 	}
	// 	return false;
	// }
	public bool Contains(int x, int y){
		if(x<right&&x>=left&&y<up&&y>=down){
			return true;
		}
		return false;
	}
}
public class Tile{
	public int x;
	public int y;

	public TileType type;

	//public bool haveDoor;
	public List<TileDirection> openDir;
	public bool roomFind;
	public Tile road;
	public Tile Init(int x,int y){
		this.x=x;
		this.y=y;
		roomFind=false;
		//haveDoor=false;
		openDir=new List<TileDirection>();
		return this;	
	}
	public void OpenDoor(Tile other){
		openDir.Add(GetDirection(other));
		other.openDir.Add(other.GetDirection(this));
	}
	public TileDirection GetDirection(Tile other){
		int[] tDir={other.x-x,other.y-y};
		int i=0;
		foreach (var dir in GridMap.directions)
		{
			if(tDir[0]==dir[0]&&tDir[1]==dir[1]){
				break;
			}
			i++;
		}
		return (TileDirection)i;
	}
	public int[] GetMoveDirection(Tile other){
		var d=new int[2];
		if(other.x>x){
			d[0]=1;
		}else if(other.x<x){
			d[0]=-1;
		}
		if(other.y>y){
			d[1]=1;
		}else if(other.y<y)
		{
			d[1]=-1;
		}
		return d;
	}

}

public enum TileDirection
{
	right=0,
	up=1,
	left=2,
	down=3,
}
public enum TileType{
	none,
	road,
	room,
	canOpenDoor,
	
}

public class Door
{
	public Tile from;
	public Tile to;

	
	public Door Init(Tile a,Tile b){
		from=a;
		to=b;
		return this;
	}
	// public Door Open(){
	// //	from.type=2;
		
	// 	to.haveDoor=true;
	// 	from.haveDoor=true;
	// 	if(to.type==TileType.none){
	// 		to.type=TileType.road;
	// 	}
	// 	from.openDir=GetDirection(from,to);
	// 	to.openDir=
	// 	from.roomFind=true;
	// 	to.roomFind=true;
	// 	return this;
	// }
	
}
public class ColorTool{
	/// <summary>
	/// 随机产生色调不同的颜色
	/// </summary>
	/// <param name="s">饱和度</param>
	/// <param name="v">明度</param>
	/// <returns></returns>
	public static Color RandomColorHSV(float s=0.7f,float v=1){
		return Color.HSVToRGB(UnityEngine.Random.Range(0,1f),s,v);
	}
}

