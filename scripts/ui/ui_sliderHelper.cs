using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_sliderHelper : MonoBehaviour
{

    public Text UnderSliderText;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float res = ((float)(int)(slider.value * 100)) / 100;
        UnderSliderText.text = res.ToString();
    }
}
