using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG.FSClient;
public class RandomMapCreator : MapCreator,IGameManager{
	
	// Use this for initialization
	void Start () {
		
	}
	public void Init(FSClient client){
		map=new GridMap();
		map.Init(30,30,client.random.Range);
		map.RandomRoom();
		map.PrimConnect();
		//map.RandomConnect(2);
		mapView.ShowMap(map);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
