using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            creditsPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CreditsShow()
    {
        creditsPanel.SetActive(true);
    }
}
