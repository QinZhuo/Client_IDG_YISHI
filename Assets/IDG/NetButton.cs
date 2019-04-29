using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace IDG
{


    public class NetButton :MonoBehaviour
    {

        public KeyNum key;
        public KeyCode pcKey;
        public bool isDown=false;
        protected KeyNum KeyValue()
        {
            return isDown ? key : 0;
        }
        private void Update()
        {
            isDown = Input.GetKey(pcKey);
        }
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                GetComponent<Button>().onClick.AddListener(
                () =>
                {

                    if (isDown == false)
                    {
                        StartCoroutine(Click());
                    }

                });
            }
        }
        
        IEnumerator Click()
        {
            isDown = true;
            yield return 1;
            isDown = false;
        }
    }
}