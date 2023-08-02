using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRunSound : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        PlayRandomSound();
    }

    public void PlayRandomSound()
    {
        if (sounds.Length > 0)
        {
            // ���������� ��������� ������ ��� ������ ���������� ����� �� �������
            int randomIndex = Random.Range(0, sounds.Length);

            // ������������� ����, ��������������� ���������� �������
            AudioClip soundToPlay = sounds[randomIndex];
            audioSource.PlayOneShot(soundToPlay);
        }

    }

}
