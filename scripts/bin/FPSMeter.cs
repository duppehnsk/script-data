using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSMeter : MonoBehaviour
{
    private int frame;
    public Text _FPS_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StatFPS();
        frame = Convert.ToInt32(1f/Time.deltaTime);
        _FPS_text.text = Convert.ToString(frame);
    }
    void StatFPS()
    {
        if(frame >= 60)
        {
            _FPS_text.color = new Color(0, 255, 0);
        }
        if(frame <= 60 && frame >=30)
        {
            _FPS_text.color = new Color(255, 255, 0);
        }
        if(frame < 30)
        {
            _FPS_text.color = new Color(255, 0, 0);
        }
    }
}
