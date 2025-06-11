using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AlternatingAudios : MonoBehaviour
{
    [Header("Audios Clips")]
    public AudioClip audioClip01;
    public AudioClip audioClip02;

    [Header("Settings")]
    public float intervalBetweenAudios;

    private AudioSource audioSource;
    private bool playFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(PlayAlternatingAudios());
    }

    IEnumerator PlayAlternatingAudios()
    {
        while (true)
        {
            AudioClip clipToPlay = playFirst ? audioClip01 : audioClip02;

            if(clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();

                yield return new WaitForSeconds(clipToPlay.length);
            }

            yield return new WaitForSeconds(intervalBetweenAudios);

            playFirst = !playFirst;
        }
    }
}
