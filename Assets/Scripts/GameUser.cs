using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IDG;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class GameUser : MonoBehaviour {
    public static GameUser user;
    public static string DataServerUrl = "http://127.0.0.1:34343/";
    string username;
    string loginToken;
    public FightRoom fightRoom;
    public InputField Input(string key)
    {
        return GameObject.Find(key).GetComponent<InputField>();
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        user = this;
        if (PlayerPrefs.HasKey("lastUsername")){
            Input("username").text = PlayerPrefs.GetString("lastUsername");
            Input("password").text = PlayerPrefs.GetString("lastLoginToken");
        }
    }
    public async Task<FightRoom> GetFightRoom()
    {
        var send = SendToken();
        send["cmd"] = "getFightRoom";
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        if (receive["status"] == "成功")
        {
            return JsonConvert.DeserializeObject<FightRoom>(receive["fightRoom"]);
        }
        else
        {
            return null;
        }
      
    }
    public async void Ready()
    {
        var send = SendToken();
        send["cmd"] = "switchReady";
        send["ready"] = true.ToString();
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
     
    }
    public async void Matching()
    {
        var send = SendToken();
        send["cmd"] = "matching";
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        Debug.LogError(receive.GetString());
    }
    public async void Login()
    {
        var send = new KeyValueProtocol();
        send["username"] = Input("username").text;
        
        if (Input("password").text == PlayerPrefs.GetString("lastLoginToken"))
        {
            send["loginToken"] = Input("password").text;
            send["cmd"] = "loginByToken";
        }
        else
        {
            send["password"] = StringTool.MD5( Input("password").text);
            send["cmd"] = "login";
        }
        var receive=await DataHttpClient.PostAsync(DataServerUrl, send);
        if (receive["status"] == "成功")
        {
            PlayerPrefs.SetString("lastUsername", receive["username"]);
            PlayerPrefs.SetString("lastLoginToken", receive["loginToken"]);
            username = send["username"];
            loginToken = receive["loginToken"];
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
        else
        {
            IDGUI.Log(receive["info"]);
            Input("username").text = "";
            Input("password").text = "";
        }
    }
    public async void Register()
    {
        var send = new KeyValueProtocol();
        send["username"] = Input("username").text;
        send["password"] = StringTool.MD5(Input("password").text);
        send["cmd"] = "register";
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        if (receive["status"] == "成功")
        {
            Login();
        }
        else
        {
            IDGUI.Log(receive["info"]);
            Input("username").text = "";
            Input("password").text = "";
        }
    }
    public async Task<bool> CreateCharacter()
    {
        var send = SendToken();
        send["cmd"] = "createCharacter";
        
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        return Success(receive);
    }
    public async Task<Character> GetCharacter()
    {
        var send = SendToken();
        send["cmd"] = "getCharacter";
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        if (int.Parse(receive["characters_count"]) > 0){
            return new Character()
            {
                info = receive["character_info0"],
                id = receive["character_id0"]
            };
        }
        else
        {
            return null;
        }
        
    }
    public async Task<bool> DeleteCharacter(string id)
    {
        var send = SendToken();
        send["cmd"] = "deleteCharacter";
        send["character_id"] = id;
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);
        return Success(receive);
    }
    public bool Success(KeyValueProtocol receive)
    {
        if (receive["status"] == "成功")
        {
            return true;
        }
        else
        {
            IDGUI.Log(receive["info"]);
            return false;
        }
    }
    KeyValueProtocol SendToken()
    {
        var send = new KeyValueProtocol();
        send["username"] = username;
        send["loginToken"] = loginToken;
        return send;
    }
    public async void FastLogin()
    {
        if (PlayerPrefs.HasKey("last_username")&&PlayerPrefs.HasKey("last_token"))
        {

        }
        var send = new KeyValueProtocol();
        send["cmd"] = "fastRegister";
        var receive = await DataHttpClient.PostAsync(DataServerUrl, send);

    }
}


public class FightRoom
{
    public int Id { get; set; }
    public string url { get; set; }
    public string ip { get; set; }
    public string port { get; set; }
    public List<PlayerInfo> playerInfos { get; set; }
    public bool InRoom(string userName)
    {
        foreach (var pi in playerInfos)
        {
            if (pi.username == userName)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckAllReady()
    {
        foreach (var player in playerInfos)
        {
            if (!player.isReady)
            {
                return false;
            }
        }
        return true;
    }
}
public class PlayerInfo
{
    public string username { get; set; }
    public string name { get; set; }
    public bool isReady { get; set; }
    public Character character { get; set; }

}