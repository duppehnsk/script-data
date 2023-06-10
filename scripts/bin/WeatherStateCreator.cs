using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weather", menuName = "X-Ray/Create/New Weather State")]
public class WeatherStateCreator : ScriptableObject
{
    public List<State> WeatherState = new List<State>();
}
[System.Serializable]
public class State
    {
        [Header("Config WeatherState")]
        public float WHour;
        public Cubemap SkyBox_Mat;
        public Vector3 sky_color;
        public Vector3 fog_color;
        public float fog_density;
        public Vector3 ambientColor;
        public Vector3 SunColor;
    }
