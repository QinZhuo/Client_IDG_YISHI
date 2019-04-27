using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using IDG;

public class SkillNodeView:NodeView<SkillNode>
{
	public override void Show(){
		if(dataNode.type==SkillNodeType.Root&&dataNode is SkillData){
			var skillData=dataNode as SkillData;
			Title("技能【"+skillData.skillId+"】");
			skillData.skillId=(SkillId)EditorGUI.EnumPopup(NodeRect.LayoutV(20),skillData.skillId);
		}else
		{
			Title("["+dataNode.type+"]");
			dataNode.type=(SkillNodeType)EditorGUI.EnumPopup(NodeRect.LayoutV(20),dataNode.type);
		}

		ViewLayout.BeginH();
		for (int i = 0; i < dataNode.boolParams.Count; i++)
		{
			dataNode.boolParams[i]=EditorGUI.Toggle(NodeRect.LayoutH(15,15),dataNode.boolParams[i]);
		}
		ViewLayout.EndH();


        ViewLayout.BeginH();
		for (int i = 0; i < dataNode.fixedParams.Count; i++)
		{
			dataNode.fixedParams[i]= EditorGUI.FloatField( NodeRect.LayoutH(30,15), dataNode.fixedParams[i].ToFloat()).ToFixed();
		}
		ViewLayout.EndH();
	}
	public override void ConnectShow(Rect rect,NodeView<SkillNode> node){
		node.dataNode.trigger=(SkillTrigger) EditorGUI.EnumPopup(rect,node.dataNode.trigger);
	}
}
public class SkillNodeEditorWindow : NodeEditorWindow<SkillNodeView,SkillNode> {
	public string[] skillNames=new string[0];
	public int skillIndex=0;
	public SkillNode curSkillNode{
		get{
			return GetMouseInNode().dataNode;
		}
	}
	public static SkillAssets skillAssets;
	[MenuItem("Window/技能节点编辑器")]
	public static void ShowWindow ()
	{
		CreatWindow<SkillNodeEditorWindow>();
	}
	[UnityEditor.Callbacks.OnOpenAsset(1)]
	public static bool AutoOpenCanvas (int instanceID, int line) 
	{
		if(line!=-1)return false;
		if (Selection.activeObject != null && Selection.activeObject.GetType () == typeof(SkillAssets))
		{
			string NodeCanvasPath = AssetDatabase.GetAssetPath (instanceID);
			SkillNodeEditorWindow.ShowWindow();
			if(skillAssets==null){
				skillAssets=Selection.activeObject as SkillAssets;
				var v =(_window as SkillNodeEditorWindow);
				v.ParseNode();
				v.ParseSkills();
			}else
			{
				skillAssets=Selection.activeObject as SkillAssets;
				var v =(_window as SkillNodeEditorWindow);
				v.ParseNode();
				v.ParseSkills();
			}
			
			return true;
		}
		return false;
	}
	public void ParseNode(){
		ClearAll();
		var skillRoot=skillAssets.assets[skillIndex].data;
		var root=GetNode();
		SubParse(root,skillRoot);
		AddTree(root,new Vector2(0,_window.position.height/2));
	}
	public void SubParse(SkillNodeView root,SkillNode skillData){
		root.dataNode=skillData;
		foreach (var skill in skillData.nextNodes)
		{
			var skillView=GetNode();
			root.Childs.Add(skillView);
			SubParse(skillView,skill);
		}
	}
	
	protected override void RightShow(Rect rect){
		ViewLayout.BeginV();
		GUI.Label(rect.LayoutV(15),"技能");
		skillIndex=EditorGUI.Popup(rect.LayoutV(15),skillIndex,skillNames);
		if(GUI.Button(rect.LayoutV(20),"load")){
			ParseNode();
		}
	
		//GUI.Button(rect.GetRect(new Rect(0,0,rect.width,50)),"load");
	}
	protected override void NodeMenu(GenericMenu menu){
		base.NodeMenu(menu);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("添加变量/bool"),false,AddBool);
		menu.AddItem(new GUIContent("减少变量/bool"),false,RemoveBool);
		menu.AddItem(new GUIContent("添加变量/int"),false,AddInt);
		menu.AddItem(new GUIContent("减少变量/int"),false,RemoveInt);
	}

	protected void AddBool(){
		curSkillNode.boolParams.Add(false);
	}
	protected void RemoveBool(){
		if(curSkillNode.boolParams.Count>0){
			curSkillNode.boolParams.RemoveAt(curSkillNode.boolParams.Count-1);
		}
		
	}
	protected void AddInt(){
		curSkillNode.fixedParams.Add(0.ToFixed());
	}
	protected void RemoveInt(){
		if(curSkillNode.fixedParams.Count>0){
			curSkillNode.fixedParams.RemoveAt(curSkillNode.fixedParams.Count-1);
		}
		
	}
	void ParseSkills(){
		skillNames=SkillNames();
	}
	protected string[] SkillNames(){
		List<string> names=new List<string>();
		foreach (var skill in skillAssets.assets)
		{
			names.Add(skill.data.skillId.ToString());
		}
		Debug.LogError("len "+names.Count);
		return names.ToArray();
	}
}
