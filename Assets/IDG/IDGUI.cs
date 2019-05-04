using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IDGUI  {
    public static Transform canvas;
    public static void AddUI(GameObject ui)
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas").transform;
        }
        if (canvas != null)
        {
            ui.transform.SetParent(canvas);
        }
    }
	public static void Log(string value)
    {
        var ui =GameObject.Instantiate<GameObject>( Resources.Load("IDGUI/" + "Log") as GameObject);
        ui.GetComponentInChildren<Text>().text = value;
        AddUI(ui);
        var rect = (ui.transform as RectTransform);
        rect.anchorMax=new Vector2(1,0.5f);
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(Screen.width, rect.sizeDelta.y);
        rect.anchoredPosition3D = Vector3.zero;
        
        GameObject.Destroy(ui, 2);
    }
}
