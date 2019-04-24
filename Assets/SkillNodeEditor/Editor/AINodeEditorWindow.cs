using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using System;
// using IDG;
public class AINodeView:NodeView<AINode>
{
	public override void Show(){
		if(dataNode.intType>10&&dataNode.intType<100){
			
			Title("组合【"+dataNode.CompositeType+"】");
			dataNode.CompositeType=(CompositeType)EditorGUI.EnumPopup(NodeRect.LayoutV(20),dataNode.CompositeType);
		}else if(dataNode.intType>100&&dataNode.intType<1000)
		{
			
			Title("动作【"+dataNode.ActionType+"】");
			dataNode.ActionType=(ActionType)EditorGUI.EnumPopup(NodeRect.LayoutV(20),dataNode.ActionType);
		}
		// if(dataNode.type==SkillNodeType.Root&&dataNode is SkillData){
		// 	var skillData=dataNode as SkillData;
		// 	Title("技能【"+skillData.skillId+"】");
		// 	skillData.skillId=(SkillId)EditorGUI.EnumPopup(NodeRect.LayoutV(20),skillData.skillId);
		// }else
		// {
		// 	Title("["+dataNode.type+"]");
		// 	dataNode.type=(SkillNodeType)EditorGUI.EnumPopup(NodeRect.LayoutV(20),dataNode.type);
		// }

		// ViewLayout.BeginH();
		// for (int i = 0; i < dataNode.boolParams.Count; i++)
		// {
		// 	dataNode.boolParams[i]=EditorGUI.Toggle(NodeRect.LayoutH(15,15),dataNode.boolParams[i]);
		// }
		// ViewLayout.EndH();

		// ViewLayout.BeginH();
		// for (int i = 0; i < dataNode.intParams.Count; i++)
		// {
		// 	dataNode.intParams[i]=EditorGUI.IntField(NodeRect.LayoutH(30,15),dataNode.intParams[i]);
		// }
		// ViewLayout.EndH();
	}
	public override void ConnectShow(Rect rect,NodeView<AINode> node){
		// node.dataNode.trigger=(SkillTrigger) EditorGUI.EnumPopup(rect,node.dataNode.trigger);
	}
	
}
public class AINodeEditorWindow : NodeEditorWindow<AINodeView,AINode> {
	 public string[] AInames=new string[0];
	 public int aiIndex=0;
	public AINode curSkillNode{
		get{
			return GetMouseInNode().dataNode;
		}
	}
	public static AIAssets aiAssets;
	[MenuItem("Window/AI节点编辑器")]
	public static void ShowWindow ()
	{
		CreatWindow<AINodeEditorWindow>();
	}
	[UnityEditor.Callbacks.OnOpenAsset(1)]
	public static bool AutoOpenCanvas (int instanceID, int line) 
	{
		if(line!=-1)return false;
		if (Selection.activeObject != null && Selection.activeObject.GetType () == typeof(AIAssets))
		{
			string NodeCanvasPath = AssetDatabase.GetAssetPath (instanceID);
			ShowWindow();
			if(aiAssets==null){
				aiAssets=Selection.activeObject as AIAssets;
				var v =(_window as AINodeEditorWindow);
				v.ParseNode();
				v.ParseAIs();
			}else
			{
				aiAssets=Selection.activeObject as AIAssets;
				var v =(_window as AINodeEditorWindow);
				v.ParseNode();
				v.ParseAIs();
			}
			
			return true;
		}
		return false;
	}
	public void ParseNode(){
		ClearAll();
		var aiRoot=aiAssets.assets[aiIndex].data;
		var root=GetNode();
		SubParse(root,aiRoot);
		AddTree(root,new Vector2(0,_window.position.height/2));
	}
	public void SubParse(AINodeView root,AINode skillData){
		root.dataNode=skillData;
		foreach (var skill in skillData.childNodes)
		{
			var skillView=GetNode();
			root.Childs.Add(skillView);
			SubParse(skillView,skill);
		}
	}
	
	protected override void RightShow(Rect rect){
		ViewLayout.BeginV();
		GUI.Label(rect.LayoutV(15),"AI");
		aiIndex=EditorGUI.Popup(rect.LayoutV(15),aiIndex,AInames);
		if(GUI.Button(rect.LayoutV(20),"load")){
			ParseNode();
		}
	
		//GUI.Button(rect.GetRect(new Rect(0,0,rect.width,50)),"load");
	}
	protected override void CreateNodeMenu(GenericMenu menu){
		menu.AddItem(new GUIContent("添加 组合 节点"), false, ()=>{
			AddNode().dataNode.intType=11;
		});
		menu.AddItem(new GUIContent("添加 动作 节点"), false, ()=>{
			AddNode().dataNode.intType=101;
		});
	}
	protected override void NodeMenu(GenericMenu menu){
		base.NodeMenu(menu);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("设置为根节点"),false,SetRootNode);
		// menu.AddItem(new GUIContent("添加变量/bool"),false,AddBool);
		// menu.AddItem(new GUIContent("减少变量/bool"),false,RemoveBool);
		// menu.AddItem(new GUIContent("添加变量/int"),false,AddInt);
		// menu.AddItem(new GUIContent("减少变量/int"),false,RemoveInt);
	}
	protected void SetRootNode(){
		aiAssets.assets[aiIndex].data=curSkillNode as AiData;
	}

	// protected void AddBool(){
	// 	curSkillNode.boolParams.Add(false);
	// }
	// protected void RemoveBool(){
	// 	if(curSkillNode.boolParams.Count>0){
	// 		curSkillNode.boolParams.RemoveAt(curSkillNode.boolParams.Count-1);
	// 	}
		
	// }
	// protected void AddInt(){
	// 	curSkillNode.intParams.Add(0);
	// }
	// protected void RemoveInt(){
	// 	if(curSkillNode.intParams.Count>0){
	// 		curSkillNode.intParams.RemoveAt(curSkillNode.intParams.Count-1);
	// 	}
		
	// }
	void ParseAIs(){
		AInames=AINames();
	}
	protected string[] AINames(){
		List<string> names=new List<string>();
		int i=0;;
		foreach (var ai in aiAssets.assets)
		{
			i++;
			names.Add(ai.data.name!=""?ai.data.name:"AI_"+i);
		}
		Debug.LogError("len "+names.Count);
		return names.ToArray();
	}
}
