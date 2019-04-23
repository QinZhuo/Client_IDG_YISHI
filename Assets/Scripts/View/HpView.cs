using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpView : MonoBehaviour {

    public Slider hpSlider;
    public void SetSlider(float value)
    {
        hpSlider.value = value;
    }

    public HpView PoolReset()
    {
        gameObject.SetActive(true);
        hpSlider.value = 1;
        return this;
    }
    public HpView PoolRecover()
    {
        gameObject.SetActive(false);
        return this;
    }

	
}
