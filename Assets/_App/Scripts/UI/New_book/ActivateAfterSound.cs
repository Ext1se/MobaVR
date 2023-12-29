using UnityEngine;

using System.Collections;

public class ActivateAfterSound : MonoBehaviour
{
    public AudioSource audioSource;  // Ссылка на источник звука
    public GameObject objectToActivate;  // GameObject для активации
    public GameObject What;  // GameObject для активации
    public GameObject WhatEnd;  // GameObject для активации

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Воспроизвести звук
        audioSource.Play();

        // Запустить корутину для активации объ[екта после воспроизведения зsвука
        StartCoroutine(ActivateObjectAfterSoundRun());
    }

    IEnumerator ActivateObjectAfterSoundRun()
    {
        // Ожидать завершения звука
        yield return new WaitForSeconds(audioSource.clip.length);

        // Активировать объект
        objectToActivate.SetActive(true);
        WhatEnd.SetActive(true);
        What.SetActive(false);
    }
}