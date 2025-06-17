using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneManager : MonoBehaviour
{
    [Header("Time To Start")]
    public float startDelay;

    [Header("Final Audio")]
    public AudioClip finalAudio;
    public AudioSource audioSource;
    public AudioSource explosionAudio;

    [Header("Objects Actives")]
    public GameObject[] objectsToActive;

    [Header("Objects Deactives")]
    public GameObject[] objectsToDeactive;

    [Header("End Panel")]
    public GameObject endPanel;
    public float panelDuration;

    [Header("Movement Player End")]
    public Transform objectToMove;
    public Transform targetPoint;
    public float moveDuration;

    [Header("Delay for Load Main Menu")]
    public float delayLoadMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunFinalScene());
    }

    IEnumerator RunFinalScene()
    {
        yield return new WaitForSeconds(startDelay);

        if(audioSource && finalAudio)
        {
            audioSource.PlayOneShot(finalAudio);
        }

        foreach(var obj in objectsToActive)
        {
            if(obj != null)
            {
                obj.SetActive(true);
                explosionAudio.Play();
            }
        }

        foreach(var obj in objectsToDeactive)
        {
            if(obj != null)
            {
                obj.SetActive(false);
            }
        }

        if(endPanel != null)
        {
            endPanel.SetActive(true);
            yield return new WaitForSeconds(panelDuration);
            endPanel.SetActive(false);
        }

        if(objectToMove != null && targetPoint != null)
        {
            Vector3 start = objectToMove.position;
            Vector3 end = targetPoint.position;
            float elapsed = 0f;

            while(elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveDuration;
                objectToMove.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(delayLoadMainMenu);
        SceneManager.LoadScene(0);
    }
}
