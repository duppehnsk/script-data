using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ui_options : MonoBehaviour
{
    [Header("Объекты окна настроек")]
    public GameObject VideoSettings;
    public GameObject SoundSettings;
    public GameObject GameSettings;
    public GameObject BaseGraphicsSettings;
    public GameObject AdvanceGraphicsSettings;

    [Header("Слайдеры окна звуки")]
    public Slider VolumeMaster;
    public Slider VolumeMusic;
    public Slider VolumeSound;
    [Header("Микшер")]
    public AudioMixer mixer;

    [Header("Слайдер чувствительности")]
    public Slider SensivitySlider;
    [Header("Выпадающий списки")]
    public Dropdown DropRender;
    public Dropdown DropQuality;
    public Dropdown TypeControl;
    [Header("Кнопки true-false")]
    public Toggle ByDistance;
    public Toggle ByDrawCrosshair;
    public Toggle ByIDStalkers;
    public Toggle ByFPSshow;

    public Toggle ByShadowFrSun;
    public Slider DistanceView;
    public Slider FOV;

    public Text ButAdvLabel;




    private float sens;
    private float VMusic;
    private float VSounds;
    private float VMaster;
    private int nResolution;
    private int nQuality;
    private int nTControl;
    private bool hasdistance;

    private float _fov;
    private float _distanceview;
    
    private bool showfps;
    private bool drawcross;
    private bool idstalkers;
    private bool adv;

    

    public void OpenAdvGrSet()
    {
        if (adv == true)
        {
            adv = false;
            
        } else adv = true;

        if (adv == false)
        {
            ButAdvLabel.text = "Расширенные";
            AdvanceGraphicsSettings.SetActive(false);
            BaseGraphicsSettings.SetActive(true);
        }
        if (adv == true)
        {
            ButAdvLabel.text = "Базовые";
            AdvanceGraphicsSettings.SetActive(true);
            BaseGraphicsSettings.SetActive(false);
        }

        
         //Debug.Log(AdvGrSet.activeSelf);
    }

    public void SetFov()
    {
        _fov  = FOV.value;
        if (FOV.value < 67)
        {
            PlayerPrefs.SetFloat("FOV", 68f);
        }
    }
    public void SetDistanceView()
    {
        _distanceview  = DistanceView.value;
    }

    public void SaveSettings()
	{
        PlayerPrefs.SetFloat("FOV", _fov);
        PlayerPrefs.SetFloat("DistanceView", _distanceview);
		PlayerPrefs.SetFloat("Sensivity", sens);
        PlayerPrefs.SetInt("DistanceMeter", hasdistance.GetHashCode());
        PlayerPrefs.SetInt("Quality", nQuality);
		PlayerPrefs.SetFloat("VolumeMusic", VMusic);
		PlayerPrefs.SetFloat("VolumeSound", VSounds);
        PlayerPrefs.SetFloat("VolumeMaster", VMaster);
        PlayerPrefs.SetInt("TControl", nTControl);
        PlayerPrefs.SetInt("DrawCrosshair", drawcross.GetHashCode());
        PlayerPrefs.SetInt("IDStalkers", idstalkers.GetHashCode());
        PlayerPrefs.SetInt("showfps", showfps.GetHashCode());
		UpdateUI();
        FindObjectOfType<ui_mainmenu>().GoToMain();

    }

    public void SetShowFps()
    {
        showfps = ByFPSshow.isOn;
    }
    public void SetDrawCrosshair()
    {
        drawcross = ByDrawCrosshair.isOn;
    }
    public void SetIDStalkers()
    {
        idstalkers = ByIDStalkers.isOn;
    }
    public void SetDistanceMeter()
    {
        hasdistance = ByDistance.isOn;
    }
    public void CancelChangeSettings()
	{
		UpdateUI();
        FindObjectOfType<ui_mainmenu>().GoToMain();
    }

    private void UpdateUI()
    {
        bool idst;
        if(PlayerPrefs.GetInt("IDStalkers")==1)
        {
            idst = true;
        }else idst = false;

        bool drawcros;
        if(PlayerPrefs.GetInt("DrawCrosshair")==1)
        {
            drawcros = true;
        }else drawcros = false;
        
        bool distancedev;
        if(PlayerPrefs.GetInt("DistanceMeter")==1)
        {
            distancedev = true;
        }else distancedev = false;
        
        bool showfpsdev;
        if(PlayerPrefs.GetInt("showfps")==1)
        {
            showfpsdev = true;
        }else showfpsdev = false;
        
        FOV.value = PlayerPrefs.GetFloat("FOV");
        DistanceView.value = PlayerPrefs.GetFloat("DistanceView");
		SensivitySlider.value = PlayerPrefs.GetFloat("Sensivity");
		VolumeMusic.value = PlayerPrefs.GetFloat("VolumeMusic");
        VolumeMaster.value = PlayerPrefs.GetFloat("VolumeMaster");
        VolumeSound.value = PlayerPrefs.GetFloat("VolumeSound");
        DropQuality.value = PlayerPrefs.GetInt("Quality");
        TypeControl.value = PlayerPrefs.GetInt("TControl");
        ByDrawCrosshair.isOn = drawcros;
        ByIDStalkers.isOn = idst;
        ByDistance.isOn = distancedev;
        ByFPSshow.isOn = showfpsdev;
        

    }

    public void SetSensivity()
    {
        float valset = SensivitySlider.value;
        sens = valset;
    }

    public void SetVolumeMusic()
    {
        float vol =0;
        vol= VolumeMusic.value;
        VMusic = vol;
        mixer.SetFloat("Music", Mathf.Log10(vol) * 20);
    }
      public void SetVolumeSounds()
    {

        float vol1 =0;
        vol1 = VolumeSound.value;
        VSounds = vol1;
        mixer.SetFloat("Sound", Mathf.Log10(vol1) * 20);
    }
    public void SetVolumeMaster()
    {
        float vol2 = 0;
        vol2 = VolumeMaster.value;
        VMaster = vol2;
        mixer.SetFloat("Master", Mathf.Log10(vol2) * 20);
    }


    public void setRender()
    {
    //QualitySettings.SetQualityLevel(dropdown.value, true);//Изменяем уровен графики
    }
    public void setTControl()
    {
    nTControl = TypeControl.value;
    }
    public void setQuality()
    {
    QualitySettings.SetQualityLevel(DropQuality.value, true);//Изменяем уровен графики
    nQuality = DropQuality.value;
    }



    void Start()
    {   
        UpdateUI();
        VideoSettings.SetActive(true);
        SoundSettings.SetActive(false);
        GameSettings.SetActive(false);  
    }

    public void GoToVideo()
    {
        VideoSettings.SetActive(true);
        SoundSettings.SetActive(false);
        GameSettings.SetActive(false);
    }
    public void GoToSound()
    {
        VideoSettings.SetActive(false);
        SoundSettings.SetActive(true);
        GameSettings.SetActive(false);
    }
    public void GoToGame()
    {
        VideoSettings.SetActive(false);
        SoundSettings.SetActive(false);
        GameSettings.SetActive(true);
    }   
}