using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ui_hud_actor : MonoBehaviour
{
    public GameObject Actor;
    public Image HealthStat;
    public Image StaminaStat;
    public Image RadiationStat;
    public float hp;
    //public Text TimeText;
    public GameObject StaminaTextDesc;

    [Header("Панели")]
    public GameObject StatsPanel;
    public GameObject ControlsPanel;
    public GameObject InventoryPanel;
    public GameObject Selector;
    public GameObject PauseMenu;
    public GameObject PdaPanel;
    //public GameObject DialogPanel;

    [Header("Кнопки")]
    public GameObject Joy;
    public GameObject inv;
    public GameObject jump;
    public GameObject crouch;
    public GameObject aim;
    public GameObject crosshair_dynamic;
    public GameObject crosshair_static;
    public GameObject AllCrosshair;
    public GameObject reload;
    public GameObject usekey;
    public GameObject firebutton;
    public GameObject sel_button;
    public GameObject spotlight_button;
    public bool click;
    // Start is called before the first frame update
    void Start()
    {
        GetPrefs();
        if(PlayerPrefs.GetInt("TControl") == 0)
        {
        StatsPanel.SetActive(true);
        ControlsPanel.SetActive(true);
        InventoryPanel.SetActive(false);
        PauseMenu.SetActive(false);
        Selector.SetActive(false);
        //DialogPanel.SetActive(false); 
        //Debug.Log("TControl=0");
        }
        if(PlayerPrefs.GetInt("TControl") == 1)
        {
            StatsPanel.SetActive(true);
            ControlsPanel.SetActive(false);
             InventoryPanel.SetActive(false);
             PauseMenu.SetActive(false);
             Selector.SetActive(false);
                //DialogPanel.SetActive(false);
                Debug.Log("TControl=1");
        }
    }

    void Update()
    {
    }
    void GetPrefs()
    {
       if(PlayerPrefs.GetInt("DrawCrosshair") == 0)
       {
           AllCrosshair.SetActive(false);
       }else AllCrosshair.SetActive(true);


    }
    void UI_Control()
    {
       if(PlayerPrefs.GetInt("TControl") == 0)
        {
            StatsPanel.SetActive(true);
            ControlsPanel.SetActive(true);
             InventoryPanel.SetActive(false);
             PauseMenu.SetActive(false);
             Selector.SetActive(false);
            //DialogPanel.SetActive(false);
            }
            if (PlayerPrefs.GetInt("TControl") == 1)
            {
                StatsPanel.SetActive(true);
                ControlsPanel.SetActive(false);
                InventoryPanel.SetActive(false);
                PauseMenu.SetActive(false);
                Selector.SetActive(false);
                //DialogPanel.SetActive(false);}
                PdaPanel.SetActive(false);
            }
    }


    void PCControl()
    {
        if(Input.GetKey(KeyCode.I))
        {
            OpenInventory();
        }
        if(Input.GetKey(KeyCode.P))
        {
            OpenPDAPanel();
        }
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            FindObjectOfType<actor_controller>().crouch = true;
        }
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            FindObjectOfType<actor_controller>().crouch = false;
        }
        if(Input.GetKey(KeyCode.L))
        {
            FindObjectOfType<actor_controller>().ToggleLight();
        }
        if(!Input.GetKey(KeyCode.LeftShift))
        {   
            FindObjectOfType<actor_controller>().horizontal = FindObjectOfType<actor_controller>().horizontal*1.5f;
            FindObjectOfType<actor_controller>().vertical = FindObjectOfType<actor_controller>().vertical*1.5f;
        }
    }

    public void OpenPDAPanel()
    {
        PauseMenu.SetActive(false);
        StatsPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        Selector.SetActive(false);
        PdaPanel.SetActive(true);
        //DialogPanel.SetActive(false);
    }

    public void ClosePDAPanel()
    {
        UI_Control();
    }
    public void PauseMenuPanel()
    {
        PauseMenu.SetActive(true);
        StatsPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        Selector.SetActive(false);
       // DialogPanel.SetActive(false);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InitializeStats();
        CheckBackBut();
        PCControl();
    }
    void CheckBackBut()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            PauseMenuPanel();
        }
    }

    public void click_use()
    {
        click = true;
    }

    public void OpenInventory()
    {
        StatsPanel.SetActive(false);
        //DialogPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        InventoryPanel.SetActive(true);
        Selector.SetActive(false);
    }

    public void CloseInventory()
    {
        UI_Control();
    }

    public void OpenSelector()
    {
        PauseMenu.SetActive(false);
        StatsPanel.SetActive(true);
        ControlsPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        Selector.SetActive(true);
        //DialogPanel.SetActive(false);
    }
    public void CloseSelector()
    {
        UI_Control();
    }

    void InitializeStats()
    {
        HealthStat.fillAmount = Actor.GetComponent<actor_stats>().Health/100;
        StaminaStat.fillAmount = Actor.GetComponent<actor_stats>().Stalmina/100;
        RadiationStat.fillAmount = Actor.GetComponent<actor_stats>().Radiation/100;
        if(Actor.GetComponent<actor_controller>().CanWalk == false )
        {
            StaminaTextDesc.SetActive(true);
        }
        else StaminaTextDesc.SetActive(false);
    }
}
