using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace MobaVR
{
    public class PigSound : MonoBehaviourPun
    {
        [SerializeField] private List<InputActionReference> m_Inputs = new();
        [SerializeField] private List<AudioClip> m_AudioClips = new();
        
        private AudioSource m_AudioSource;
        
        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            
            foreach (InputAction inputAction in m_Inputs)
            {
                inputAction.performed += OnInputPerformed;
            }
        }

        private void OnInputPerformed(InputAction.CallbackContext obj)
        {
            if (m_AudioSource.isPlaying)
            {
                return;
            }

            int position = Random.Range(0, m_AudioClips.Count);
            photonView.RPC(nameof(RpcPlaySoundClip), RpcTarget.All, position);
        }

        [PunRPC]
        private void RpcPlaySoundClip(int position)
        {
            if (m_AudioSource.isPlaying)
            {
                return;
            }
            
            m_AudioSource.PlayOneShot(m_AudioClips[position]);
        }
    }
}