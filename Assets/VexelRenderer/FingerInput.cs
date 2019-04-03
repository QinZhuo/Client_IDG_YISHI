using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FingerAction
{
	none,
	scale
}
public class FingerInput : MonoBehaviour {
	public static Action<float> ScaleChange{
		get{
			return Instance._ScaleChange;
		}
		set{
			Instance._ScaleChange=value;
		}
	}
	public static FingerAction action;


	private Action<float> _ScaleChange;
	static FingerInput _instance;
	static FingerInput Instance{
		get{
			if(_instance==null){
				_instance=new GameObject("FingerInput").AddComponent<FingerInput>();
			}
			return _instance;
		}
	}

	bool MultiMove{
		get{
			if(Input.touchCount>1){
				if(Input.GetTouch(0).phase==TouchPhase.Moved||Input.GetTouch(1).phase==TouchPhase.Moved){
					return true;
				}
			}
			return false;
		}
	}
	float _minCheck=0f;
	bool MinCheck(float value){
		return Mathf.Abs(value)>=_minCheck*Time.deltaTime;
	}
	bool MinCheck(Vector2 v2){
		return MinCheck( v2.x)||MinCheck(v2.y);
	}

	Touch this[int index]{
		get{
			return Input.GetTouch(index);
		}
	}
	private void Update() {
		Scale();
	}
	
	
	bool Different(Vector2 a,Vector2 b)
	{
		if(Vector2.Angle(a,b)>150){
			return true;
		}else{
			return false;
		}
	}
	bool Same(Vector2 a, Vector2 b){
		if(Vector2.Angle(a,b)<30){
			return true;
		}else
		{
			return false;
		}
	}
	float GetScale(Touch a,Touch b){
		if(Same(b.deltaPosition,a.position-b.position)){
			return -Vector2.Distance(a.deltaPosition,b.deltaPosition);
		}else
		{
			return Vector2.Distance(a.deltaPosition,b.deltaPosition);
		}
	}
	GUIStyle style=new GUIStyle();
	private void Start() {
		style.fontSize=40;
		
	}
	private void OnGUI() {
		GUI.color=Color.green;
		GUILayout.Label("触摸数目 "+Input.touchCount,style);
		if(Input.touchCount>1){
			GUILayout.Label("触摸移动方向 "+this[0].deltaPosition.normalized+"  "+this[1].deltaPosition.normalized,style);
			GUILayout.Label("触摸两点移动夹角 "+Vector2.Angle(this[0].deltaPosition.normalized,this[1].deltaPosition.normalized),style);
			GUILayout.Label("触摸点远离 "+Different(this[0].deltaPosition.normalized,this[1].deltaPosition.normalized),style);
			GUILayout.Label("放缩 "+GetScale(this[0],this[1]),style);
		}
		
	}
	void Scale(){
		if(MultiMove&&Application.platform==RuntimePlatform.Android){
			if(action==FingerAction.none){
				var a=this[0].deltaPosition;
				var b=this[1].deltaPosition;
				if(MinCheck(a)&&MinCheck(b)){
					if(Different(a,b)){
						action=FingerAction.scale;
					}
				}	
			}
			if(action==FingerAction.scale){
				var a=this[0].deltaPosition;
				var b=this[1].deltaPosition;
				if(Different(a,b)){
					_ScaleChange(-GetScale(this[0],this[1])/Screen.height);
				}else
				{
					action=FingerAction.none;
				}
			}
		}
		if(Application.platform==RuntimePlatform.WindowsEditor)
		{
			if(MinCheck(Input.GetAxis("Mouse ScrollWheel"))){
				_ScaleChange(-Input.GetAxis("Mouse ScrollWheel"));
			}
		}
		
	}
		
	
}
