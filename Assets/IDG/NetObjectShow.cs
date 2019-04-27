using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IDG 
{
    abstract public class View:MonoBehaviour
    {
        private NetData _data;
        public NetData data{
            get{
                return _data;
            }
            set{
                _data=value;
                Init();
            }
        }
        protected virtual void OnStart(){

        }
        public void Init(){
            transform.position=data.transform.Position.ToVector3();
            _data.OnStart=OnStart;
        }
        public virtual System.Type GetDataType()
        {
            return null;
        }
    }
    /// <summary>
    /// 网络物体显示类（需要渲染模型的物体的显示类）
    /// </summary>
    /// <typeparam name="T">该物体对应的数据类实现</typeparam>
    abstract public class NetObjectView<T> : View where T:NetData,new()
    {
        public bool showGizmo=true;
        // /// <summary>
        // /// 数据类对象
        // /// </summary>
        
        public T Data{
            get{
                 return data as T;
            }
        }
      
    
        ///// <summary>
        ///// 初始化碰撞体信息
        ///// </summary>
        //protected void InitCollider()
        //{
        //    Collider2DBase_IDG collider2D = GetComponent<Collider2DBase_IDG>();
        //    if (collider2D != null)
        //    {
        //        Debug.Log("collider2D");
        //        data.Shap= collider2D.GetShap();
        //    }
        //}

    
        /// <summary>
        /// 显示位置与网络位置进行差值同步
        /// </summary>
        /// <param name="timer">差值同步速度</param>
        protected void LerpNetPos(float timer)
        {
            if (data == null) return;
         
            transform.position = Vector3.Lerp(transform.position, data.transform.Position.ToVector3(), timer);
            transform.rotation = Quaternion.Euler(0, -data.transform.Rotation.ToFloat(), 0);
            
        }
       
        protected virtual void Update()
        {
            LerpNetPos(Time.deltaTime*10);
        }

        /// <summary>
        /// 显示碰撞形状与包围盒大小
        /// </summary>
        private void OnDrawGizmos()
        {
            if (data==null||data.Shap == null||!showGizmo) return;
            Gizmos.color = Color.white;
           // Gizmos.DrawWireCube(data.Shap.position.ToVector3(), new Vector3(data.Width.ToFloat(), 0, data.Height.ToFloat()));
             Gizmos.color = Color.blue;
            int i;
            for ( i = 0; i < data.Shap.PointsCount-1; i++)
            {
                Gizmos.DrawSphere((data.Shap.GetPoint(i) + data.transform.Position).ToVector3(),0.1f);
                  Gizmos.DrawLine((data.Shap.GetPoint(i) + data.transform.Position).ToVector3(),(data.Shap.GetPoint(i+1) + data.transform.Position).ToVector3());
            }
            if(i<data.Shap.PointsCount)
            Gizmos.DrawSphere((data.Shap.GetPoint(i) + data.transform.Position).ToVector3(),0.1f);
        }
        
        //public void InitClient(){
        //    this.data.InitClient .temp);
       
        //}

        
        public override System.Type GetDataType()
        {
            return typeof(T);
        }
    }
    //[System.Serializable]

    /// <summary>
    /// 基础网络数据类 所有网络相关的物体数据基类（需同步位置渲染模型的与需要帧调用的类继承此类）
    /// </summary>
    public abstract class NetData
    {
        public string tag;
        public string name;
        public bool active=true;
        
        public int clientId=-1;
        private ShapBase _shap;
         
        public View view;
       
        public TransformComponent transform;
        public PhysicsComponent rigibody;
        public List<ComponentBase> comList;

        public FSClient client;

        public System.Action OnStart;
        public bool IsLocalPlayer
        {
            get
            {
                return client.inputCenter.IsLocalId(clientId);
            }
        }
        public Fixed Width
        {
            get
            {
                return Shap.width;
            }
        }
        public Fixed Height
        {
            get
            {
                return Shap.height;
            }
        }
        /// <summary>
        /// 当前对象所处的四叉树空间
        /// </summary>
        public List<Tree4> trees = new List<Tree4>();
        public bool isTrigger=false;
        
        public abstract string PrefabPath();
        protected abstract void FrameUpdate();

        bool start = false;
        protected void DataFrameUpdate()
        {
            if (!active) return;
            if (!start) { Start(); start = true; }
            transform.PhysicsEffect();
            FrameUpdate();
            foreach (var item in comList)
            {
                item.Update();
            }
            rigibody.Update();
           
        }
        public virtual void Init(FSClient client,int clientid=-1)
        {
            this.client = client;
            client.inputCenter.frameUpdate += DataFrameUpdate;
            this.clientId=clientid;
            Input=client.inputCenter[this.clientId];
            comList = new List<ComponentBase>();
            rigibody =new PhysicsComponent();
            rigibody.Init(OnPhysicsCheckEnter,OnPhysicsCheckStay,OnPhysicsCheckExit);
            rigibody.netdata = this;
            transform=new TransformComponent();
            transform.Init(this);
            
       //     Debug.Log(name+"init");
        }
        public T AddCommponent<T>(T cm=null) where T :IDG.ComponentBase ,new()
        {
            if(cm==null){
                cm = new T();
            }
            cm.InitNetData(this);
            this.comList.Add(cm);
            return cm ;
        }
        public virtual void Start()
        {
            OnStart();
        }
        

        public virtual void OnPhysicsCheckStay(NetData other)
        {
           // UnityEngine.Debug.Log("Stay触发");
        }
        public virtual void OnPhysicsCheckEnter(NetData other)
        {
            if (other.tag == "Bullet")
            {
                UnityEngine.Debug.LogError("Enter触发 tag[" + tag + "]" + GetHashCode()+" bullet contain"+other.rigibody.lastCollisonDatas.Contains(this)+" c "+ other.rigibody.collisonDatas.Contains(this));
            }
        }
        public virtual void OnPhysicsCheckExit(NetData other)
        {
            // UnityEngine.Debug.Log("Exit触发");
        }
        public void Reset(Fixed2 position,Fixed rotation)
        {
           transform.Reset(position,rotation);

        }
       
        
      

        public void Destory()
        {
            this.active = false;
            client.inputCenter.frameUpdate -= DataFrameUpdate;
            client.physics.Remove(this);
        }
       
        
        

       
        public ShapBase Shap
        {
            get
            {
                return _shap;
            }
            set
            {

                if (_shap != null)
                {
                    client.physics.Remove(this);
                }
                if (value != null)
                {
                    
                    _shap = value;
                    _shap.data = this;
                    _shap.ResetSize();
                    client.physics.Add(this);
                    
                }
                else
                {
                    _shap = value;
                    
                }
                
            }
        }
        public Fixed deltaTime
        {
            get
            {
                return FSClient.deltaTime;
            }
        }
        public InputBase Input;
       
    }
}