using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSystem : MonoBehaviour
{
    [Header("WeatherSystem Component")]
    public WeatherSystem _weathersystem;

     [Header("Ambient System")]
    public AudioClip[] AmbientRandom;
    public GameObject[] AmbientObjects;
    // Start is called before the first frame update
    void Start()
    {
        int r = Random.Range(0, AmbientRandom.Length);
        AmbientObjects[0].GetComponent<AudioSource>().clip = AmbientRandom[r];
        AmbientObjects[0].GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
