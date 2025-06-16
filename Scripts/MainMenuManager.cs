using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject crossfade;
    public GameObject instructionsPanel;
    public GameObject audiosPanel;

    public Animator transition;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            creditsPanel.SetActive(false);
            instructionsPanel.SetActive(false);
            audiosPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadLevel(1));
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CreditsShow()
    {
        creditsPanel.SetActive(true);
    }

    public void InstructionsShow()
    {
        instructionsPanel.SetActive(true);
    }

    public void AudioShow()
    {
        audiosPanel.SetActive(true);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        crossfade.SetActive(true);

        transition.SetTrigger("start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelIndex);
    }
}
