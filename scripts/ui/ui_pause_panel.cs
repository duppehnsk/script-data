using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ui_pause_panel : MonoBehaviour
{
    public GameObject PausePanel;

    public GameObject ContinueGame;
    public GameObject LastSave;
    public GameObject SaveGame;
    public GameObject LoadGame;
    public GameObject Settings;
    public GameObject GoToMain;
    public GameObject HelpPanel;
    public GameObject GoExitfromGame;
    public GameObject PauseMenu;

    public GameObject StatsPanel;
    public GameObject ControlsPanel;

    public GameObject[] Video = new GameObject[3];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ContinueGame_l()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        StatsPanel.SetActive(true);
        ControlsPanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("00_mainmenu", LoadSceneMode.Single);
        Time.timeScale = 1; 
    }

    public void OpenHelpPanel()
    {
        PauseMenu.SetActive(false);
        HelpPanel.SetActive(true);
        //PausePanel.SetActive(false);
    }

    public void CloseHelpMenu()
    {
        HelpPanel.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void GoToOptions()
    {
        PauseMenu.SetActive(false);
        HelpPanel.SetActive(false);
        PausePanel.SetActive(true);
        SaveGame.SetActive(false);
        LoadGame.SetActive(false);
        Settings.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
