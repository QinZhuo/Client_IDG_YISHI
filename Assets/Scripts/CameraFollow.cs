using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG ;
using IDG;
public class CameraFollow : MonoBehaviour,IGameManager
{
    public int InitLayer{
        get{
            return 100;
        }
    }
    public FSClient client;
    private TransformComponent target;
    public Vector3 offset;
    public float _movespeed=1;

    [ContextMenu("SetOffset")]
    public void SetOffset(){
        offset=transform.localPosition;
    }
    // Start is called before the first frame update
    public void Init(FSClient  client){
		this.client=client;
	}
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            if (client==null||client.localPlayer == null){
              //    Debug.LogError("no localPlayer");
                return;
              
            }

            target = client.localPlayer.transform;
            
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.Position.ToVector3() + offset, Time.deltaTime * _movespeed);
          //   Debug.LogError( target.Position.ToVector3() );
         }

    }
}
