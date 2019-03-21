using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapCreator))]
public class MapCreatorInspector:Editor
{
	
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		var creator=(MapCreator)target;
		if(GUILayout.Button("生成地图",GUILayout.Height(20))){
			creator.RandomMap();
		}
	}
} 