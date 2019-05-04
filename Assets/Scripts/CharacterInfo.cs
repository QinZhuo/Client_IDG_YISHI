using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IDG;
public class Character
{
    public string id { get; set; }
    public string info { get; set; }
}
[System.Serializable]
public class ObjectGroup
{
    public List<GameObject> objs;
    public void SetActiveId(int index)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].SetActive(i == index);
        }
    }
}
public class CharacterInfo : MonoBehaviour {
    
    public GameObject posePeopleRefap;
    public Character character;
    public PeopleRenderer characterView;
    public ObjectGroup playerGroup;
    public ObjectGroup matchingGroup;
    public ObjectGroup camGroup;
    public ObjectGroup sceneGroup;
    public List<GameObject> fightRoomPosList;
    Coroutine coroutine;
    public void Matching(bool value)
    {
        if (value)
        {
            GameUser.user.Matching();
            matchingGroup.SetActiveId(1);
            coroutine = StartCoroutine(WaitMatchingOver());
        }
        else
        {
            matchingGroup.SetActiveId(0);

        }
       
    }
    public void Ready()
    {
        GameUser.user.Ready();
    }
    
    private void Start()
    {
        Fresh();


    }
    IEnumerator WaitMatchingOver()
    {
        while (true)
        {
            yield return new UnityEngine.WaitForSeconds(1);
            GetFightRoom();
            
        }
    }
    public string ClothingPath(KeyValueProtocol receive,string clothing)
    {
        var id=int.Parse( receive[clothing])%3+1;
        return clothing + id;
    }
    public async void DeleteCharacter()
    {
        if (await GameUser.user.DeleteCharacter(character.id))
        {
            Fresh();
        }
    }
    public void FreshFightRoom(FightRoom fightRoom)
    {
        for (int i = 0; i < fightRoom.playerInfos.Count; i++)
        {
            var character = fightRoom.playerInfos[i].character;
            FreshCharacter(character, fightRoomPosList[i]);
        }
    }
    public async void GetFightRoom()
    {
        var fightRoom = await GameUser.user.GetFightRoom();
        if (fightRoom != null)
        {
            Debug.LogError("人数" + fightRoom.playerInfos.Count);
            camGroup.SetActiveId(1);

            FreshFightRoom(fightRoom);


            sceneGroup.SetActiveId(1);
            if (fightRoom.CheckAllReady())
            {
                //Debug.LogError("ready!!! " + fightRoom.ip + ":" + fightRoom.port);
                if (fightRoom.ip != "" && fightRoom.port != "")
                {
                    StopCoroutine(coroutine);
                    GameUser.user.fightRoom = fightRoom;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("ZombieCity");
                }
            }
        }
        else
        {
            sceneGroup.SetActiveId(0);
            camGroup.SetActiveId(0);
        }
        
    }
    public void FreshCharacter(Character character,GameObject pos)
    {
        var renderer = pos.GetComponentInChildren<PeopleRenderer>();
        
        if (renderer == null)
        {
            var info = new KeyValueProtocol(character.info);
            renderer = Instantiate(posePeopleRefap, transform).GetComponent<PeopleRenderer>();
            renderer.clothingPath.Clear();
            renderer.clothingPath.Add("BodyMan");
            renderer.clothingPath.Add(ClothingPath(info, "Top"));
            renderer.clothingPath.Add(ClothingPath(info, "Bottom"));
            renderer.clothingPath.Add(ClothingPath(info, "Shoes"));
            renderer.transform.SetParent(pos.transform);
            renderer.transform.localPosition = Vector3.zero;
        }
        else
        {
            //renderer.clothingPath.Clear();
            //renderer.clothingPath.Add("BodyMan");
            //renderer.clothingPath.Add(ClothingPath(info, "Top"));
            //renderer.clothingPath.Add(ClothingPath(info, "Bottom"));
            //renderer.clothingPath.Add(ClothingPath(info, "Shoes"));
            //renderer.FreshMesh();
        }
    
    }
    public async void Fresh()
    {
        character =await GameUser.user.GetCharacter();
        if (character != null)
        {
            var info = new KeyValueProtocol(character.info);
            if (characterView == null)
            {
                characterView = Instantiate(posePeopleRefap, transform).GetComponent<PeopleRenderer>();
                characterView.transform.localPosition = Vector3.zero;
                characterView.clothingPath.Clear();
                characterView.clothingPath.Add("BodyMan");
                characterView.clothingPath.Add(ClothingPath(info, "Top"));
                characterView.clothingPath.Add(ClothingPath(info, "Bottom"));
                characterView.clothingPath.Add(ClothingPath(info, "Shoes"));
            }
            else
            {
                characterView.clothingPath.Clear();
                characterView.clothingPath.Add("BodyMan");
                characterView.clothingPath.Add(ClothingPath(info, "Top"));
                characterView.clothingPath.Add(ClothingPath(info, "Bottom"));
                characterView.clothingPath.Add(ClothingPath(info, "Shoes"));
                characterView.FreshMesh();
            }
            playerGroup.SetActiveId(1);
        }
        else
        {
            if (characterView != null)
            {
                Destroy(characterView.gameObject);
            }
            playerGroup.SetActiveId(0);
        }
    }
    public async void CreateCharacter()
    {
        if (await GameUser.user.CreateCharacter())
        {
            Fresh();
        }
    }
}
