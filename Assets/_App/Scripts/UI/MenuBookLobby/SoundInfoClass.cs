using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInfoClass : MonoBehaviour
{
    //[Header("MenuClass")]
    //public AudioClip Defender; // ���� 1
    //public AudioClip Ranger; // ���� 2
    //public AudioClip Wizard; // ���� 3

    //[Header("MenuName")]
    //public AudioClip Name; // ���� 1

    //[Header("MenuHands")]
    //public AudioClip Hands; // ���� 1    
    
    //[Header("MenuGoTir")]
    //public AudioClip GoTir; // ���� 1

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) // ���� �� ������� ��� AudioSource, ��������� ���
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

   
    public void PlaySpecificSound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }


    public void StopAllSounds()
    {
                audioSource.Stop();

    }


}
