using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDG;
public class CameraFollow : MonoBehaviour
{
    public FightClientForUnity3D _fightclient;
    private TransformComponent target;
   public Vector3 offset;
    public float _movespeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            if (_fightclient.client.localPlayer == null)
                return;

            target = _fightclient.client.localPlayer.transform;
            
        }
        else
        {
          
            
            transform.position = Vector3.Lerp(transform.position, target.Position.ToVector3() - offset, Time.deltaTime * _movespeed);
        
         }

    }
}
