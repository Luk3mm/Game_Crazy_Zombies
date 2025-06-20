using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused { get; private set; }

    public GameObject pausePanel;
    public GameObject audioPanel;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if(pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        GameIsPaused = isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            CloseAllPanels(); 
        }
    }

    public void Retry()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        CloseAllPanels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        CloseAllPanels();
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        CloseAllPanels();
        pausePanel.SetActive(false);
    }

    public void AudioSettingShow()
    {
        //GameIsPaused = false;
        audioPanel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        if(audioPanel != null)
        {
            audioPanel.SetActive(false);
        }
    }
}
