using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using BNG;

public class GitaraRun : MonoBehaviourPunCallbacks
{
    public AudioClip[] guitarSounds;
    private int currentSoundIndex = 0;
    private bool canPlaySound = true;

    private void OnTriggerEnter(Collider other)
    {
        Grabber grabber = other.GetComponent<Grabber>();
        if (grabber != null && canPlaySound)
        {
           // PlayNextSound();
            //photonView.RPC("PlayNextSoundRPC", RpcTarget.AllBuffered);
            photonView.RPC("PlayNextSoundRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PlayNextSoundRPC()
    {
        PlayNextSound();
    }

    private void PlayNextSound()
    {
        if (currentSoundIndex < guitarSounds.Length)
        {
            AudioSource.PlayClipAtPoint(guitarSounds[currentSoundIndex], transform.position);
            currentSoundIndex++;
            canPlaySound = false;
            StartCoroutine(ResetSoundCooldown());
        }
        else
        {
            // Если все звуки в массиве воспроизведены, начните сначала
            currentSoundIndex = 0;
        }
    }

    private IEnumerator ResetSoundCooldown()
    {
        yield return new WaitForSeconds(0.1f); // Время задержки между звуками (в секундах)
        canPlaySound = true;
    }
}