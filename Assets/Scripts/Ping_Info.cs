using UnityEngine;

public class Ping_Info : MonoBehaviour
{
	public string ip ;
	Ping ping;
	string label;
	GUIStyle guiStyle;

	void Start()
	{
		//ip = "121.42.114.17";    // 这里我用的是我网站的ip(www.u3d8.com)  需要替换成自己的服务器ip

		SendPing();

		guiStyle = new GUIStyle();
		guiStyle.normal.background = null;
		guiStyle.fontSize = 40;

	}

	bool isNetWorkLose = false;
	void OnGUI()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			label = "460";
			SetColor(460);
			isNetWorkLose = true;
		}
		else if (isNetWorkLose || (null != ping && ping.isDone))
		{
			isNetWorkLose = false;
			label = ping.time.ToString();
			SetColor(ping.time);
			ping.DestroyPing();
			ping = null;
			Invoke("SendPing", 1);//每秒Ping一次
		}

		GUI.Label(new Rect(10, 50, 200, 50), "ping:" + label + "ms", guiStyle);
	}
	void SendPing()
	{
		ping = new Ping(ip);
	}

	/// <summary>
	/// 仿王者荣耀延迟过高，颜色变化
	/// </summary>
	/// <param name="pingValue"></param>
	void SetColor(int pingValue)
	{
		if (pingValue < 100)
		{
			guiStyle.normal.textColor = new Color(0, 1, 0);
		}
		else if (pingValue < 200)
		{
			guiStyle.normal.textColor = new Color(1, 1, 0);
		}
		else
		{
			guiStyle.normal.textColor = new Color(1, 0, 0);
		}
	}
}
