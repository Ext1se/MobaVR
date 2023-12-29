using System.Collections;
using UnityEngine;
using Photon.Pun;

public class perdeg_grusha : MonoBehaviourPunCallbacks
{
    public AudioClip[] perdeg;
    public AudioClip otrigka;

    private void OnEnable()
    {
        //StartCoroutine(PlaySounds());
    }

    public void Play()
    {
        StartCoroutine(PlaySounds());
    }

    IEnumerator PlaySounds()
    {
        yield return new WaitForSeconds(2f);
        //photonView.RPC("PlayOtrigkaSound", RpcTarget.All);
        PlayOtrigkaSound();

        yield return new WaitForSeconds(7f);
        int randomIndex = Random.Range(0, perdeg.Length);
        //photonView.RPC("PlayRandomSound", RpcTarget.All, randomIndex);
        PlayRandomSound(randomIndex);
    }

    [PunRPC]
    void PlayOtrigkaSound()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source != null && otrigka != null)
        {
            source.clip = otrigka;
            source.Play();
        }
    }

    [PunRPC]
    void PlayRandomSound(int index)
    {
        if (index >= 0 && index < perdeg.Length)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source != null)
            {
                source.clip = perdeg[index];
                source.Play();
            }
        }
    }
}