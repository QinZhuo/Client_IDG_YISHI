using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewAssetManager : MonoBehaviour {
	public static GameViewAssetManager instance; 
	public SkillAssets skillAssets;
	public WeaponAssets weaponAssets;
	// Use this for initialization
	private void Awake() {
		instance=this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
