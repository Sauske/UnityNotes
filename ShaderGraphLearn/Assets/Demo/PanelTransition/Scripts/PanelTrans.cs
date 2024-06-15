using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTrans : MonoBehaviour
{
    public RawImage img;
    private Material mat;
    private int sliderId;
    private float sliderValue;

    void Start()
    {
        mat = img.material;
        sliderId = Shader.PropertyToID("_Slider");
    }


    void Update()
    {
        sliderValue += Time.deltaTime;
        mat.SetFloat(sliderId, sliderValue);

        if (sliderValue > 3)
        {
            sliderValue = 0;
        }
    }
}
