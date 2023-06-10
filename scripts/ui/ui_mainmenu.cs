using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ui_mainmenu : MonoBehaviour
{
    [Header("Главное меню")]
    [Tooltip("GameObject Главного меню")]
    public GameObject MainMenu;

    public string _name_load_scene;
    //public GameObject AddonsManager;
    public GameObject ChooseMap;
    public GameObject ChooseDifficult;
    public GameObject LoadPanel;
    public GameObject Options;
    public GameObject TitriPanel;

    public GameObject HelpPanel;
    public GameObject exitPanel;
    public GameObject Loading;
    private  AsyncOperation LoadingScreen;
    public Image loadstat;
    public Text loadtext;
    public GameObject StartButton;
    private float ProgressLoad;
    private bool load1 = false;
    public AudioSource Music;

    
    public GameObject[] Video = new GameObject[3];
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("load1") == 0)
        {
            PlayerPrefs.SetFloat("DistanceView", 200f); //устанавливаем дальность прорисовки по умолчанию
            PlayerPrefs.SetFloat("FOV", 68f);//устанавливаем угол обзора по умолчанию
            PlayerPrefs.SetFloat("Sensivity", 0.25f);//устанавливаем чувствительность сенсора по умолчанию
            PlayerPrefs.SetFloat("VolumeMaster", 1);//устанавливаем канал звука МАСТЕР по умолчанию
            PlayerPrefs.SetFloat("VolumeMusic", 1);//устанавливаем канал звука МУЗЫКА по умолчанию
            PlayerPrefs.SetFloat("VolumeSound", 1);//устанавливаем канал звука ЗВУКИ по умолчанию
            PlayerPrefs.SetInt("Render", 0);//устанавливаем тип рендера минимальный (ПОКА НЕРЕАЛИЗОВАНО)
            PlayerPrefs.SetInt("Resolution", 0);//устанавливаем разрешение экрана минимальное
            PlayerPrefs.SetInt("Quality", 0);//устанавливаем качество графики минимальное
            PlayerPrefs.SetInt("load1", 1); //?
            //PlayerPrefs.SetFloat("TimeSpeed",1);
            //PlayerPrefs.SetFloat("brightnessFonarik",1);
            PlayerPrefs.SetFloat("TControl",0); //устанавливаем тип управления по умолчанию на сенсор
            PlayerPrefs.SetFloat("DistanceMeter",0); //устанавливаем расстояние до цели по умолчанию ВЫКЛ
            PlayerPrefs.SetFloat("DrawCrosshair",1); //устанавливаем отрисовку прицела по умолчанию ВКЛ
            PlayerPrefs.SetFloat("IDStalkers",1); //устанавливаем распознавание сталкеров по умолчанию ВКЛ
            PlayerPrefs.SetFloat("showpfs",0); //устанавливаем счетчик кадров по умолчанию ВЫКЛ
        }
        //AddonsManager.SetActive(false);
        LoadPanel.SetActive(false);
        Loading.SetActive(false);
        MainMenu.SetActive(true);
        Options.SetActive(false);
        HelpPanel.SetActive(false);
        TitriPanel.SetActive(false);
        exitPanel.SetActive(false);
        ChooseMap.SetActive(false);
        int R = Random.Range(0, 3);
        //Video[R].SetActive(true);
        StartButton.SetActive(false);
        ChooseDifficult.SetActive(false);
    }

    public void openExitPanel()
    {
        MainMenu.SetActive(true);
        exitPanel.SetActive(true);

    }

    public void OpenHelpPanel()
    {
        HelpPanel.SetActive(true);
        MainMenu.SetActive(false);
    }
    public void openTitriPanel()
    {
        MainMenu.SetActive(false);
        TitriPanel.SetActive(true);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        //ProgressLoad = instance.LoadingScreen.progress;
        if(load1 == true)
        {
            UpdateUI();
        }
    }

    public void StartLevel()
    {
        LoadingScreen.allowSceneActivation = true;
    }

    public void OpenAddonsManager()
    {
        LoadPanel.SetActive(false);
        //AddonsManager.SetActive(true);
        Loading.SetActive(false);
        MainMenu.SetActive(false);
        Options.SetActive(false);
        int R = Random.Range(0, 3);
        //Video[R].SetActive(false);
        StartButton.SetActive(false);
        ChooseMap.SetActive(false);
        ChooseDifficult.SetActive(false);
    }

void UpdateUI()
{
    StartButton.SetActive(true);
    loadstat.fillAmount = LoadingScreen.progress;
    if(LoadingScreen.progress == 0.1f)
    {
        loadtext.text = "Загрузка игры...";
    }
    if(LoadingScreen.progress == 0.4f)
    {
        loadtext.text = "Загрузка моделей...";
    }
    if(LoadingScreen.progress == 0.6f)
    {
        loadtext.text = "Загрузка текстур...";
    }
    if(LoadingScreen.progress == 0.8f)
    {
        loadtext.text = "Синхронизация клиента...";
    }
    if(LoadingScreen.progress >= 0.9f)
    {
        loadtext.text = "Нажмите, для продолжения...";
    }
    
}

    public void GoToOptions()
    {
        MainMenu.SetActive(false);
        Options.SetActive(true);  
    }
    public void GoToMain()
    {
        HelpPanel.SetActive(false);
        LoadPanel.SetActive(false);
        //AddonsManager.SetActive(false);
        Loading.SetActive(false);
        MainMenu.SetActive(true);
        Options.SetActive(false);
        TitriPanel.SetActive(false);
        exitPanel.SetActive(false);
        ChooseMap.SetActive(false);
        ChooseDifficult.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartNewGame()
    {
        LoadPanel.SetActive(false);
        _name_load_scene = "marsh";
        Music.Stop();
        load1 = true;
        Loading.SetActive(true);
        loadstat.fillAmount = 0f;
        //LoadingScreen = SceneManager.LoadSceneAsync("deadcity_sa2_map"); 
        LoadingScreen = SceneManager.LoadSceneAsync(_name_load_scene); 
        LoadingScreen.allowSceneActivation = false;
  
    }
    public void StartTestMap()
    {
        _name_load_scene = "testlevel";
        Music.Stop();
        load1 = true;
        Loading.SetActive(true);
        loadstat.fillAmount = 0f;
        //LoadingScreen = SceneManager.LoadSceneAsync("deadcity_sa2_map"); 
        LoadingScreen = SceneManager.LoadSceneAsync("00_cs_tir"); 
        LoadingScreen.allowSceneActivation = false;
    }

    public void OpenChooseDif()
    {
        MainMenu.SetActive(false);
        ChooseDifficult.SetActive(true);
    }
    public void ChooseMapManager()
    {
        MainMenu.SetActive(false);
        ChooseMap.SetActive(true);
        ChooseDifficult.SetActive(false);
    }
    public void OpenLoadPanel()
    {
        LoadPanel.SetActive(true);
        //AddonsManager.SetActive(false);
        Loading.SetActive(false);
        MainMenu.SetActive(false);
        Options.SetActive(false);
        TitriPanel.SetActive(false);
        exitPanel.SetActive(false);
        ChooseMap.SetActive(false);
        ChooseDifficult.SetActive(false); 
    }
}
