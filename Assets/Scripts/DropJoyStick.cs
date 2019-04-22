using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IDG;
using IDG.MobileInput;
public class DropJoyStick : JoyStick {
    // public Image fillImage;
    // public Image backImage;
    // public SkillAction skillList;
    // Use this for initialization
    
    public KeyCode pcKey = KeyCode.F;

    // Update is called once per frame

    public FightClientForUnity3D unityClient;
    void Start () {
       
        unityClient=GetComponentInParent<FightClientForUnity3D>();
    }
	void Update () {

        PcControl();
       
        ;
      
    }
    void PcControl()
    {
        if (!useKey || onDrag|| unityClient.client.localPlayer==null) return;
        isDown = Input.GetKey(pcKey);

        Vector3 pos =Input.mousePosition- unityClient.mainCamera.WorldToScreenPoint(unityClient.client.localPlayer.view.transform.position);
        moveObj.transform.position = transform.position + pos.normalized * maxScale;
        Vector3 tmp = GetVector3();
        dir = new Fixed2(tmp.x, tmp.y);
    }
    
}
