using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {
	GridMapCreator creator;
	GridMap map;
	// Use this for initialization
	[ContextMenu("randomMap")]
	void Init () {
		map=new GridMap();
		map.Init(30,30);
		map.RandomRoom();
		map.PrimMaze();
		map.FindDoor();
		map.Fix();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmos() {
		if(map!=null)
		map.GizmoShow();
	}
}


public class GridMapCreator
{
	
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
	public List<Room> rooms;
	public Color[] colors;
	public void Init(int width,int height){
		this.height=(height/2)*2+1;
		this.width=(width/2)*2+1;
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
		for (int i = 0; i < 5; i++)
		{
			int checkNum=0;
			while(!AddRoom(Room.RandomRoom(10,this))&&checkNum<5){checkNum++;};
		}
		
	}
	int tempT=3;
	public bool AddRoom(Room room){
		if(!room.Check())return false;
		tempT++;
		for (int x = room.left; x < room.right; x++)
		{
			for (int y = room.down; y < room.up;y++)
			{
				nodes[x,y].type=tempT;
			}
		}
		rooms.Add(room);
		return true;
	}
	public void FindDoor(){
		foreach (var room in rooms)
		{
			room.FindDoor();
		}
	}
	public void GizmoShow(){
		for (int x = 0; x < width; x+=1)
		{
			for (int y = 0; y < height; y+=1)
			{
				if(nodes[x,y].type>0){
					Gizmos.color=colors[nodes[x,y].type];
					Gizmos.DrawCube(new Vector3(x,0,y),Vector3.one);
				}else if(nodes[x,y].type==-1)
				{
					Gizmos.color=new Color(1,1,1,0.3f);
					Gizmos.DrawCube(new Vector3(x,0,y),Vector3.one);
				}
			}
		}
	}
	public bool InMap(int x,int y){
		if(x<width&&x>=0&&y<height&&y>=0){
			return true;
		}
		return false;
	}
	public List<Tile> GetNeib(Tile node,params int[] types){
		List<Tile> nodeList=new List<Tile>();
		if(types.Length==0)types=new int[]{0};
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
	public List<Tile> GetNeib2Size(Tile node,params int[] types){
		List<Tile> nodeList=new List<Tile>();
		if(types.Length==0)types=new int[]{0};
		foreach (var d in directions)
		{
			if(InMap(node.x+d[0]*2,node.y+d[1]*2)){
				var otherNode=nodes[node.x+d[0]*2,node.y+d[1]*2];
				var road=nodes[node.x+d[0]*1,node.y+d[1]*1];
				foreach (var t in types)
				{
					if(otherNode.type==t){
						otherNode.road=road;
						nodeList.Add(otherNode);
						break;
					}
				}
				
			}
		}
		return nodeList;
	}

	public int[] GetNullPoint(){
		for (int x = 0; x < this.width; x+=2)
		{
			for (int y = 0; y < this.height; y+=2)
			{
				if(nodes[x,y].type==0){
					return new int[]{x,y};
				}
			}
		}
		return null;
	}
	public void PrimMaze(){
		int[] startPos=GetNullPoint();
		if(startPos==null)return;
		Tile cur=null;
		List<Tile> roads=new List<Tile>();
		roads.Add(nodes[startPos[0],startPos[1]]);
		while(roads.Count>0){
			cur=roads[Random.Range(0,roads.Count)];
			roads.Remove(cur);
			cur.type=1;
			if(cur.road!=null){
				cur.road.type=1;
			}
			roads.AddRange(GetNeib2Size(cur));
		}
		PrimMaze();
	}

	public void Fix(){
		List<Tile> fixNodes=new List<Tile>();
		for (int x = 0; x < this.width; x+=1)
		{
			for (int y = 0; y < this.height; y+=1)
			{
				if(this[x,y].type!=1)continue;
				var neibs=GetNeib(this[x,y],1,2);
				if(neibs.Count<=1){
					//Debug.Log("["+x+","+y+"]"+neibs.Count+":"+neibs[0].type);
					this[x,y].type=-1;
					if(neibs.Count==1&&neibs[0].type==1){
						fixNodes.Add(neibs[0]);
					}
				}
			}
		}
		List<Tile> tempNodes=new List<Tile>();
		while (fixNodes.Count>0)
		{
			
			foreach (var item in fixNodes)
			{
				var neibs=GetNeib(item,1,2);
				if(neibs.Count<=1){
					//Debug.Log("["+x+","+y+"]"+neibs.Count+":"+neibs[0].type);
					item.type=-1;
					if(neibs.Count==1&&neibs[0].type==1){
						tempNodes.Add(neibs[0]);
					}
				}
				
			}
			fixNodes.Clear();
			fixNodes.AddRange(tempNodes);
			tempNodes.Clear();
		}
	}
}

public class Room
{
	public GridMap map;
	public int x;
	public int y;
	public int width;
	public int height;

	public int left{get{return x;}}
	public int right{get{return x+width;}}
	public int up{get{return y+height;}}
	public int down{get{return y;}}
	
	public Room Init(int left,int down,int width,int height,GridMap map){
		x=left;
		y=down;
		this.width=width;
		this.height=height;
		this.map=map;
		return this;
	}
	public static Room RandomRoom(int size,GridMap map){
		return new Room().Init(Random.Range(0,map.width-size),
								Random.Range(0,map.height-size),
								Random.Range(size/2,size),
								Random.Range(size/2,size),
								map
								);
	}

	public bool Check(){
		for (int i = this.x; i < this.x+width; i++)
		{
			for (int j = this.y; j < this.y+height; j++)
			{
				if(map.nodes[i,j].type!=0){
					return false;
				}
			}	
		}
		return true;
	}

	public void FindDoor(){
		List<Door> doors=new List<Door>();
		for (int i = this.x; i < right; i++)
		{
			for (int j = this.y; j <up; j++)
			{
				if(i==x||j==y||i==right-1||j==up-1){
					foreach (var dir in GridMap.directions)
					{
						if(IsCanOpenDoor(i,j,dir)){
							doors.Add(new Door().Init(map[i,j],map[i+dir[0],j+dir[1]]));
						}
					

					}
				}
			}	
		}
		for (int i = 0; i < Random.Range(1,3); i++)
		{
			if(doors.Count>0){
				doors.Remove(doors[Random.Range(0,doors.Count)].Open());
			}

		}
		
	}
	
	public bool ContainsTile(Tile tile){
		return Contains(tile.x,tile.y);
	}
	public bool IsCanOpenDoor(int x,int y,int[] dir){
		int dx=x+dir[0];
		int dy=y+dir[1];
		int dx2=x+dir[0]*2;
		int dy2=y+dir[1]*2;
		if(map[dx,dy]!=null&& map[dx,dy].type!=0){
			if(!Contains(dx,dy)){
				return true;
			}
		}else if(map[dx2,dy2]!=null&&map[dx2,dy2].type==1)
		{
			if(!(Contains(dx2,dy2))){
				return true;
			}
		}
		return false;
	}
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

	public int type;

	public Tile road;
	public Tile Init(int x,int y){
		this.x=x;
		this.y=y;
		return this;	
	}

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
	public Door Open(){
	//	from.type=2;
		to.type=2;
		return this;
	}
}
public class ColorTool{
	/// <summary>
	/// 随机产生色调不同的颜色
	/// </summary>
	/// <param name="s">饱和度</param>
	/// <param name="v">明度</param>
	/// <returns></returns>
	public static Color RandomColorHSV(float s=0.7f,float v=1){
		return Color.HSVToRGB(Random.Range(0,1f),s,v);
	}
}