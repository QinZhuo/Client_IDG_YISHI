using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Info : MonoBehaviour {
	public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    public Color color;
    GUIStyle style=new GUIStyle();
    void Start()
    {
        //Application.targetFrameRate = 90;
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        style.fontSize=40;
        style.normal.textColor=color;
    }
    void OnGUI()
    {
        
        
        GUILayout.Label("FPS: " + fps.ToString("f2"),style);
    }
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
