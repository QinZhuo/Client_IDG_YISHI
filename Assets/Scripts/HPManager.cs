using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class HPManager : MonoBehaviour, IGameManager
{
    public Camera mainCam;
    public GameObject hpViewPrefab;

    public Stack<HpView> viewPool =new Stack<HpView>();
   
    int IGameManager.InitLayer
    {
        get
        {
            return 100;
        }
    }

    void IGameManager.Init(FSClient client)
    {
        viewPool = new Stack<HpView>();
        mainCam = (client.unityClient as FightClientForUnity3D).mainCamera;
    }
    public void Recover(HpView view)
    {
        if(viewPool!=null&&view!=null) viewPool.Push(view.PoolRecover());
    }

    public HpView GetView()
    {
        if (viewPool.Count <= 0)
        {
            return Instantiate(hpViewPrefab, transform).GetComponent<HpView>();
        }
        else
        {
            return viewPool.Pop().PoolReset();
        }
    }
    
}
