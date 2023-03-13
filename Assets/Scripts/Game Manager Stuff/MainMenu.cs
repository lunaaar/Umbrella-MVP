using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject titleMenu, settingMenu;

    public GameObject pauseFirstButton, settingsFirstButton;

    public void startButton()
    {
        SceneManager.LoadScene(sceneName: "Platforming");
    }

    public void quitButton()
    {
        Application.Quit();
    }

    public void settingsButton()
    {
        titleMenu.SetActive(false);
        settingMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
    }

    public void backButton()
    {
        settingMenu.SetActive(false);
        titleMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }
}
