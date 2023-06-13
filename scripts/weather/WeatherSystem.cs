using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherSystem : MonoBehaviour
{
    [Header("Time")]
    public int hours; //часики
    public int minutes;
    [HideInInspector]
    public float seconds;
    public float time_speed = 1;
    public Text Time_Text;


    [Header("WeatherController")]
    public WeatherStateCreator[] weatherStates;
    public Light _sun;
    private int randomstate;
    public bool[] hours_weather_event;
    public Material skyboxMaterial;
    public float blendSpeedShader;
    private float blendAmount = 0f;

    [Header("RainController")]
    public GameObject _rainobject;
    private int RainGuarant = 50; // øàíñ ãàðàíòèðîâàííîãî äîæäÿ ïî óìîë÷àíèþ
    public int RainProcent; //êàêîâ øàíñ äîæäÿ ïðè ðàíäîìå èñïîëüçóåòñÿ äëÿ óðàâíåíèÿ ñ rainguarant;
    public int RainDuration; // ïðîäîëæèòåëüíîñòü äîæäèêà
    private int RainDuration_backup; // ðåçåðâíàÿ ïåðåìåííàÿ, äëÿ íîðìàëüíîé ðàáîòû òàéìåðîâ

    public int startrainhour;
    private int startraintime;

    [Header("Events")]
    public bool rain;
    public bool thunderbolt;

    void Start()
    {
        _rainobject.GetComponent<ParticleSystem>().Stop();
        _rainobject.GetComponentInChildren<ParticleSystem>().Stop();
        StartWeatherSystem();
        PredRainLogic();
        _rainobject.transform.rotation = Quaternion.identity;
    }
    void Update()
    {
        TimeManager();
        FixTime();
        WeatherLogic();
        RainLogic();
        UpdateSkyboxBlend();
    }
    void StartWeatherSystem() //ïðè ñòàðòå ìèðà - ïîäãðóæàåì è ïîäñòàâëÿåì ñêàéáîêñû, öâåòà ýìáèåíòà, òóìàíà, ñîëíöà ñîîòâåòñòâóþùèå óñòàíîâëåííîìó âðåìåíè
    {

        randomstate = UnityEngine.Random.Range(0, weatherStates.Length);// âûòàñêèâàåì ñëó÷àéíóþ ïîãîäó
        var newcubemap1 = weatherStates[randomstate].WeatherState[hours].Cubemap_tex;
        var newcubemap2 = weatherStates[randomstate].WeatherState[hours+1].Cubemap_tex;
        var suncolorstart = new Color((weatherStates[randomstate].WeatherState[hours].SunColor.x), (weatherStates[randomstate].WeatherState[hours].SunColor.y), (weatherStates[randomstate].WeatherState[hours].SunColor.z));
        var fogcolorstart = new Color((weatherStates[randomstate].WeatherState[hours].fog_color.x), (weatherStates[randomstate].WeatherState[hours].fog_color.y), (weatherStates[randomstate].WeatherState[hours].fog_color.z));
        var ambientcolotart = new Color((weatherStates[randomstate].WeatherState[hours].ambientColor.x), (weatherStates[randomstate].WeatherState[hours].ambientColor.y), (weatherStates[randomstate].WeatherState[hours].ambientColor.z));
        skyboxMaterial.SetTexture("_MainTex", newcubemap1);
        skyboxMaterial.SetTexture("_BlendTex", newcubemap2);
        hours_weather_event[hours] = true;
        RenderSettings.fogDensity = 0.009f;
        RenderSettings.ambientSkyColor = ambientcolotart;
        _sun.color = suncolorstart;
        RenderSettings.fogColor = fogcolorstart;
    }
    void WeatherLogic() //ñòàðòóåì ëîãèêó ïîãîäû
    {
        for (int i = 0; i < weatherStates[randomstate].WeatherState.Count; i++)
        {
            if(hours == i && !hours_weather_event[i])
            {
                StartBlendingWeather(); //íà÷èíàåì ëþòî ìåñèòü ïîãîäó
                hours_weather_event[i] = true; //÷òîáû íå áëåíäèë ìíîãî ðàç íåáî
            }
        }
    }


    void StartBlendingWeather()
    {
        StartCoroutine(BlendSkybox());
        StartCoroutine(ChangeLightColors());
    }

    IEnumerator ChangeLightColors() //ëþòî íà÷èíàåì ìåñèòü öâåòà ýìáèåíòà, ñêàéáîêñà, ñîëíöà è òóìàíà
    {
        Color startColorSun = _sun.color;
        Color startColorFog = RenderSettings.fogColor;
        Color startAmbientColor = RenderSettings.ambientSkyColor;
        Color startColorSky = skyboxMaterial.GetColor("_Tint");
        float elapsedTime = 0f;
        Color targetColorSun = new Color((weatherStates[randomstate].WeatherState[hours].SunColor.x), (weatherStates[randomstate].WeatherState[hours].SunColor.y), (weatherStates[randomstate].WeatherState[hours].SunColor.z));
        Color targetColorFog = new Color((weatherStates[randomstate].WeatherState[hours].fog_color.x), (weatherStates[randomstate].WeatherState[hours].fog_color.y), (weatherStates[randomstate].WeatherState[hours].fog_color.z));
        Color targetColorAmbient = new Color((weatherStates[randomstate].WeatherState[hours].ambientColor.x), (weatherStates[randomstate].WeatherState[hours].ambientColor.y), (weatherStates[randomstate].WeatherState[hours].ambientColor.z));
        Color targetColorSky = new Color((weatherStates[randomstate].WeatherState[hours].sky_color.x), (weatherStates[randomstate].WeatherState[hours].sky_color.y), (weatherStates[randomstate].WeatherState[hours].sky_color.z));
        while (elapsedTime < blendSpeedShader)
        {
            // Âû÷èñëèòü òåêóùèé öâåò, ïîñòåïåííî ïåðåõîäÿ îò íà÷àëüíîãî ê öåëåâîìó
            Color currentColorSun = Color.Lerp(startColorSun, targetColorSun, elapsedTime / blendSpeedShader);
            Color currentColorFog = Color.Lerp(startColorFog, targetColorFog, elapsedTime / blendSpeedShader);
            Color currentColorAmbient = Color.Lerp(startAmbientColor, targetColorAmbient, elapsedTime / blendSpeedShader);
            Color currentColorSky = Color.Lerp(startColorSky, targetColorSky, elapsedTime / blendSpeedShader);
            // Ïðèìåíèòü òåêóùèé öâåò ê èñòî÷íèêó ñâåòà
            _sun.color = currentColorSun;
            RenderSettings.fogColor = currentColorFog;
            RenderSettings.ambientSkyColor = currentColorAmbient;
            skyboxMaterial.SetColor("_Tint", currentColorSky);
            // Óâåëè÷èòü ïðîøåäøåå âðåìÿ
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Óñòàíîâèòü öåëåâîé öâåò â êà÷åñòâå îêîí÷àòåëüíîãî öâåòà
        _sun.color = targetColorSun;
        RenderSettings.fogColor = targetColorFog;
        RenderSettings.fogDensity = 0.009f;
        RenderSettings.ambientSkyColor = targetColorAmbient;
        skyboxMaterial.SetColor("_Tint", targetColorSky);
    }

    IEnumerator BlendSkybox() //êð÷ òóò ðàñ÷åòû ïîøëè ïî îòíîøåíèþ ñêîðîñòè ñìåøèâàíèÿ
    {
        float elapsedTime = 0f;
        float startBlendAmount = blendAmount;
        float targetBlendAmount = 1f;

        while (elapsedTime < blendSpeedShader)
        {
            blendAmount = Mathf.Lerp(startBlendAmount, targetBlendAmount, elapsedTime / blendSpeedShader);
            skyboxMaterial.SetFloat("_BlendAmount", blendAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blendAmount = targetBlendAmount;
        skyboxMaterial.SetFloat("_BlendAmount", blendAmount);

        skyboxMaterial.SetTexture("_MainTex", weatherStates[randomstate].WeatherState[hours].Cubemap_tex); //çàìåíèëè êóáìàïó êîòîðóþ ïîëó÷èëè â ñëîò îñíîâíîé òåêñòóðû, ÷òîáû íå äîïóñòèòü ñêà÷êà íàïðÿæåíèÿ â íåáå
        blendAmount = 0f;//îáíóëèëè äëÿ äàëüíåéøåãî áëåíäà
        skyboxMaterial.SetFloat("_BlendAmount", blendAmount);
        skyboxMaterial.SetTexture("_BlendTex", weatherStates[randomstate].WeatherState[hours+1].Cubemap_tex); //ïîäãîòîâèëè íîâóþ òåêñòóðó íåáà äëÿ áëåíäà
    }

    void UpdateSkyboxBlend()
    {
        skyboxMaterial.SetFloat("_BlendAmount", blendAmount);//ïðîñòî îáíîâëÿåì ïàðàìåòð
    }
    void TimeManager()
    {
        Time_Text.text = hours + " : " + minutes;
        seconds += Time.deltaTime * time_speed;
        if (seconds >= 5f)
        {
            minutes++;
            seconds = 0f;
        }
        if (minutes > 59)
        {
            seconds = 0f;
            minutes = 0;
            hours++;
        }
        if (hours > 23)
        {
            seconds = 0f;
            minutes = 0;
            hours = 0;
            NextDay();
        }
    }
    void FixTime()
    {
        if (hours < 10)
        {
            if (minutes < 10)
            {
                Time_Text.text = "0" + hours + ":" + "0" + minutes;
            }
            else
                Time_Text.text = "0" + hours + ":" + minutes;
        }
        if (hours >= 10)
        {
            if (minutes >= 10)
            {
                Time_Text.text = hours + ":" + minutes;
            }
            else
                Time_Text.text = hours + ":" + "0" + minutes;
        }
    }
    void NextDay()
    {
        PredRainLogic();
        // Êîä äëÿ íà÷àëà íîâîãî äíÿ
    }

    void PredRainLogic()// ñòàðòîâàÿ ëîãèêà äëÿ äîæäÿ íà ñïàâíå.
    {
        RainProcent = UnityEngine.Random.Range(10, 100);
        if(RainProcent > RainGuarant)
        {
            startrainhour = UnityEngine.Random.Range(0, 23);
            RainDuration = UnityEngine.Random.Range(1, 3);
        }
    }
    void RainLogic() //òóò ïîøåë ëþòåéøèé çàìåñ ñ ëîãèêîé äîæäÿ. óñëîâèÿ ïðîñòûå - åñëè âðåìÿ ñîâïàäàåò è äîæäü íå èäåò = íà÷àëî ëèâíÿ. Òàêæå è íàîáîðîò.
    {
        _rainobject.transform.rotation = Quaternion.identity;

        if (startrainhour == hours)
        {
            if(!rain)
            {
                _rainobject.GetComponent<ParticleSystem>().Play(); //çàïóñêàåì íàø ëèâåíü
                _rainobject.GetComponentInChildren<ParticleSystem>().Play();
                _rainobject.GetComponent<AudioSource>().Play();
                startraintime = hours;
                RainDuration_backup = startraintime + RainDuration;
                rain = true;
                startrainhour = 0;
            }
        }
        if(hours == RainDuration_backup)
        {
            if(rain)
            {
                _rainobject.GetComponent<ParticleSystem>().Stop();//îñòàíàâëèâàåì íàø ëèâåíü
                _rainobject.GetComponent<AudioSource>().Stop();
                _rainobject.GetComponentInChildren<ParticleSystem>().Stop();
                rain = false;
                startraintime = 0;
                RainDuration = 0;
            }
        }
    }
}
