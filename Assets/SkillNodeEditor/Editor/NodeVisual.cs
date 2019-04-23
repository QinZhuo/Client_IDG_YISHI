using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public enum Dir{
	left,
    up,
	right, 
}
public static class ViewLayout
{
    private static int allHeight=0;
    private static int allWights=0;
    private static int vHeight=0;
    public static void BeginV(){
        allHeight=0;
    }
    public static void BeginH(){
        allWights=0;
        vHeight=0;
    }
    public static void EndH(){
        allHeight+=vHeight;
    }
    public static Rect AutoScale(this Rect rect){
        return new Rect(rect.x,rect.y,Mathf.Max(allWights,rect.width),Mathf.Max(allHeight,rect.height));
    }
    public static Rect LayoutV(this Rect back,int height,int offset=1){
        var rect=new Rect(offset,allHeight,back.width-2*offset,height);
        allHeight+=height;
        return rect;
    }
    public static Rect LayoutH(this Rect back,int width,int height,int offset=1){
        var rect=new Rect(allWights,allHeight,width,height);
        if(height>vHeight){
            vHeight=height+offset;
        }
        allWights+=width;
        return rect;
    }
}
public static class ViewTool{
    public static float Fix(this float pos,float min,float max,float fixStep){
        while(pos>max){
            pos-=fixStep;
        }while (pos<min)
        {
            pos+=fixStep;
        }
        return pos;
    }
    public static Rect Add(this Rect rect,Vector2 v2){
        return new Rect(rect.x+v2.x,rect.y+v2.y,rect.width,rect.height);
    }
    public static Rect Sub(this Rect rect,Vector2 v2){
        return new Rect(rect.x-v2.x,rect.y-v2.y,rect.width,rect.height);
    }
	public static Vector3 GetPos(this Rect rect,Dir dir,float offset=0){
		if(dir==Dir.left){
        
			return new Vector3(rect.x+offset,rect.y+rect.height/2);
		}else if(dir==Dir.right)	
		{
            
			return new Vector3(rect.x+rect.width-offset,rect.y+rect.height/2);
		}else if(dir==Dir.up)
        {
            return new Vector3(rect.x+rect.width/2,rect.y+offset);
        }
        else
		{
			return Vector3.zero;
		}
	}
	public static Rect GetRect(this Vector3 position,Rect defaultRect){
		return new Rect(position.x-defaultRect.width/2,position.y-defaultRect.height/2,defaultRect.width,defaultRect.height);
	}
    public static Rect AlignCenter(this Rect back,Rect child){
        return new Rect((back.width-child.width)/2,child.y,child.width,child.height);
    }
    public static Rect GetPosRect(this Rect rect,Dir dir,Rect defaultRect){
        if(dir==Dir.left||dir==Dir.right){
            return rect.GetPos(dir,defaultRect.width/2).GetRect(defaultRect);
        }
		else
        {
             return rect.GetPos(dir,defaultRect.height/2).GetRect(defaultRect);
        }
	}
    // public static Rect GetRect(this Rect rect,Rect offsetRect){
    //     return new Rect(rect.x+offsetRect.x,rect.y+offsetRect.y,offsetRect.width,offsetRect.height);
    // }
    // 绘制曲线
    public static void DrawCurve(Vector3 start, Vector3 end)
    {
		float size=Mathf.Abs(start.x-end.x)/2;
		Handles.DrawBezier(start,end,start+Vector3.right*size,end+Vector3.left*size,Color.green,null,3f);
    }
}


public abstract class NodeView<DataT> where DataT:ITreeNode<DataT> 
{
	public static Rect NodeRect= new Rect(0, 0, 160, 50);
	public static Rect ConnectRect=new Rect(0,0,100,15);
    //public static Rect TitleRect=new Rect(0,0,100,20);
    
   
    public void Title(string title){
        ViewLayout.BeginV();
        var rect= NodeRect.LayoutV(15);
        GUI.Label(rect,title);
    }
    // 子节点
    protected List<NodeView<DataT>> childNodeList = new List<NodeView<DataT>>();

    public int ChildCount{
        get{
            return childNodeList.Count;
        }
    }
    public List<NodeView<DataT>> Childs{
        get{
            return childNodeList;
        }
    }
    public void AddChild(NodeView<DataT> child){
        childNodeList.Add(child);
        dataNode.childNodes.Add(child.dataNode);
    }
    
    // 在窗口中显示位置
    public Rect windowRect{
        get{
            return _windowRect.Add(offset());
        }
        set{
            _windowRect=value.Sub(offset());
        }
    }
    public System.Func<Vector2> offset;
    private Rect _windowRect;

    public DataT dataNode;
	public Dir condition=0;
	public abstract void Show();

	 /// <summary>
    /// 每帧绘制从 节点到所有子节点的连线
    /// </summary>
    /// <param name="nodeView"></param>
    public void DrawToChildCurve()
    {
        for (int i = childNodeList.Count - 1; i >= 0; --i)
        {
            NodeView<DataT> childNode = childNodeList[i];
            // 删除无效节点
            if ( childNode.isRelease)
            {
                childNodeList.RemoveAt(i);
                dataNode.childNodes.Remove(childNode.dataNode);
                continue;
            }
			var startPos=windowRect.GetPos(Dir.right);
			var endPos=childNode.windowRect.GetPos(Dir.left);
            ViewTool.DrawCurve(startPos,endPos);
			var rect= ((startPos+endPos)/2).GetRect(ConnectRect);
			ConnectShow(rect,childNode);
        }
    }
	public abstract void ConnectShow(Rect rect,NodeView<DataT> node);
    /// <summary>
    /// 是否为有效节点， isRelease = true 为已经销毁的节点，为无效节点
    /// </summary>
    public bool isRelease = false;
    /// <summary>
    /// 删除节点时调用
    /// </summary>
    public void Release()
    {
        isRelease = true;
    }
}

public abstract class NodeEditorWindow<T,DataT>:EditorWindow where T:NodeView<DataT>,new() where DataT:ITreeNode<DataT>,new()
{	
	[Range(0.5f,2f)]
	public float zoom=0.5f;
	public static EditorWindow _window;
 	// 保存窗口中所有节点
    private List<NodeView<DataT>> nodeRootList = new List<NodeView<DataT>>();
    // 当前选择的节点
    private NodeView<DataT> selectNode = null;


    public Vector2 viewOffset=Vector2.one*1;
	private float midLinePos=0.8f;
	private Texture2D backTex=null;
	public Event curEvent{
		get{
			return Event.current;
		}
	}
	public Vector2 mousePos;
	
	public bool makeTransitionMode=false;
	public static void CreatWindow<windowType> () where windowType:EditorWindow
	{
		_window = GetWindow<windowType> ();
		_window.minSize = new Vector2 (400, 400);
		_window.titleContent = new GUIContent ("Node Editor");
	}

	
    private void OnGUI()
    {
        
        mousePos = curEvent.mousePosition;
		GUI.backgroundColor=Color.gray;
		LeftGroup();
        RightGroup();
        // 重新绘制
        Repaint();
    }
    void RightGroup(){
        var rect=new Rect(position.width*midLinePos,0,position.width*(1-midLinePos),position.height);
        GUI.BeginGroup(rect);
        RightShow(rect);
        GUI.EndGroup();
    }
    protected abstract void RightShow(Rect rect);
	void LeftGroup(){
        var leftRect=new Rect(0,0,position.width*midLinePos,position.height);
		GUI.BeginGroup(leftRect);
		if(backTex!=null){
            var xTex=leftRect.width/backTex.width;
            var yTex=leftRect.height/backTex.height;
            var xStart=viewOffset.x.Fix(-backTex.width,0,backTex.width);
            var yStart=viewOffset.y.Fix(-backTex.height,0,backTex.height);
          
            for (int x = 0; x <= xTex+1; x++)
            {
                for (int y = 0; y <= yTex+1; y++)
                {
                    GUI.DrawTexture (new Rect (xStart+backTex.width*x, yStart+backTex.height*y,backTex.width,backTex.height), backTex);
                }
            }
			
		}else
		{
			//Debug.LogError("background.png is null");
			backTex=Resources.Load<Texture2D>("background");
			//backTex=Resources.Load<Texture2D>("background.png");
		}
        //遍历所有节点，移除无效节点
        for (int i = nodeRootList.Count - 1; i >= 0; --i)
        {
            if (nodeRootList[i].isRelease)
            {
                nodeRootList.RemoveAt(i);
            }
        }

        if (curEvent.button == 1) // 鼠标右键
        {
            if (curEvent.type == EventType.MouseDown)
            {
                if (!makeTransitionMode)
                {
                    bool clickedOnNode = false;
                    
                    selectNode = GetMouseInNode();
                    clickedOnNode = (selectNode != null);

                    if (!clickedOnNode)
                    {
                        ShowMenu(0);
                    }
                    else
                    {
                        ShowMenu(1);
                    }
                }
            }
        }
        
        // 选择节点为空时，无法连线
        if (selectNode == null)
        {
            makeTransitionMode = false;
        }

        if (!makeTransitionMode)
        {
            if (curEvent.type == EventType.MouseUp)
            {
                selectNode = null;
            }
        }

        // 在连线状态，按下鼠标
        if (makeTransitionMode && curEvent.type == EventType.MouseDown)
        {
            
            NodeView<DataT> newSelectNode = GetMouseInNode();
            // 如果按下鼠标时，选中了一个节点，则将 新选中根节点 添加为 selectNode 的子节点
            if(newSelectNode==null){

            }else
            if (selectNode != newSelectNode)
            {
                selectNode.AddChild(newSelectNode);
            }

            // 取消连线状态
            makeTransitionMode = false;
            // 清空选择节点
            selectNode = null;
        }

        // 连线状态下 选择节点不为空 
        if (makeTransitionMode && selectNode != null)
        {

            ViewTool.DrawCurve(selectNode.windowRect.GetPos(Dir.right), mousePos);
        }

        // 开始绘制节点 
        // 注意：必须在  BeginWindows(); 和 EndWindows(); 之间 调用 GUI.Window 才能显示
        BeginWindows();
        for (int i = 0; i < nodeRootList.Count; i++)
        {
            NodeView<DataT> nodeView = nodeRootList[i];
            nodeView.windowRect = GUI.Window(i,  nodeView.windowRect, DrawNodeWindow,"");
            nodeView.DrawToChildCurve();
        }
        EndWindows();
        if(curEvent.button==0){
            if (curEvent.type == EventType.MouseDrag)
            {
                selectNode = GetMouseInNode();
                if(selectNode==null){
                    viewOffset+= curEvent.delta;
                }
            }
        }

		GUI.EndGroup();
	}
	public string test;
    void DrawNodeWindow(int id)
    {
        NodeView<DataT> nodeView = nodeRootList[id];
		
        // 可拖拽位置的 window
		nodeView.Show();
        nodeView.windowRect= nodeView.windowRect.AutoScale();
		GUI.DragWindow();
      
    }

    // 获取鼠标所在位置的节点
    protected NodeView<DataT> GetMouseInNode()
    {
     
        NodeView<DataT> selectRoot = null;
        for (int i = 0; i < nodeRootList.Count; i++)
        {
            NodeView<DataT> nodeView = nodeRootList[i];
            // 如果鼠标位置 包含在 节点的 Rect 范围，则视为可以选择的节点
            if (nodeView.windowRect.Contains(mousePos))
            {
                selectRoot = nodeView;
               
                break;
            }
        }

        return selectRoot;
    }

    private void ShowMenu(int type)
    {  
        GenericMenu menu = new GenericMenu();
        if (type == 0)
        {
            // 添加一个新节点
            CreateNodeMenu(menu);
        }
        else
        {
            // 连线子节点
            
            NodeMenu(menu);
        }
        
        menu.ShowAsContext();
        Event.current.Use();
    }
    protected virtual void CreateNodeMenu(GenericMenu menu){
          menu.AddItem(new GUIContent("Add Node"), false, ()=>{AddNode();});
    }
    protected virtual void NodeMenu(GenericMenu menu){
        menu.AddItem(new GUIContent("Make Transition"), false, MakeTransition);
        menu.AddSeparator("");
        // 删除节点
        menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);
    }

    protected T GetNode()
    {
        T nodeView = new T();
        nodeView.dataNode=new DataT();
        nodeView.offset=()=>{return viewOffset;};
        return nodeView;
    }
    // 添加节点
    protected T AddNode() 
    {
        var nodeView=GetNode();
        nodeView.windowRect = new Rect(mousePos.x, mousePos.y, NodeView<DataT>.NodeRect.width, NodeView<DataT>.NodeRect.height);
        nodeRootList.Add(nodeView);
        return nodeView;
    }

    protected void AddTree(NodeView<DataT> nodeRoot,Vector2 pos){
        nodeRootList.Add(nodeRoot);
        nodeRoot.windowRect = new Rect(pos.x,pos.y, NodeView<DataT>.NodeRect.width, NodeView<DataT>.NodeRect.height);
        int i=-nodeRoot.ChildCount/2;
        foreach (var node in nodeRoot.Childs)
        {
            AddTree(node,pos+new Vector2(NodeView<DataT>.NodeRect.width,i*NodeView<DataT>.NodeRect.height)*2);
            i++;
        }
    }

    // 连线子节点
    private void MakeTransition()
    {
        makeTransitionMode = true;
    }
    protected void ClearAll(){
        foreach (var node in nodeRootList)
        {
            node.Release();
        }
    }
    // 删除节点
    private void DeleteNode()
    {
        
        selectNode = GetMouseInNode();
        if (selectNode != null)
        {
            selectNode.Release();
            nodeRootList.Remove(selectNode);
        }
    }

   

   
	
}
