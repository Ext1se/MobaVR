using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class AnimalSkinTransformer : MonoBehaviourPun
    {
        [SerializeField] private bool m_CanCast = false;
        [SerializeField] private float m_Duration = 5f;
        [SerializeField] private ParticleSystem m_ParticleSystem;
        [SerializeField] private Transform m_ParticlePoint;
        //[SerializeField] private PlayerStateSO m_PlayerStateSO;

        private AnimalSkin m_AnimalSkin;
        private SkinCollection m_SkinCollection;
        private PlayerVR m_PlayerVR;

        private PlayerStateSO m_CurrentPlayerStateSO;
        private PlayerStateSO m_PrevPlayerStateSO;

        private bool m_IsAnimalSkin = true;

        private void OnDestroy()
        {
            m_AnimalSkin.OnActivated.RemoveListener(OnSkinActivated);
            m_AnimalSkin.OnDeactivated.RemoveListener(OnSkinDeactivated);

            m_PlayerVR.WizardPlayer.OnDie -= OnPlayerDie;
            m_PlayerVR.WizardPlayer.OnReborn -= OnPlayerReborn;
        }

        // TODO: подписываемся тут
        private void Awake()
        {
            m_CurrentPlayerStateSO = ScriptableObject.CreateInstance<PlayerStateSO>();
            
            m_AnimalSkin = GetComponent<AnimalSkin>();
            m_SkinCollection = GetComponentInParent<SkinCollection>();
            m_PlayerVR = GetComponentInParent<PlayerVR>();

            m_AnimalSkin.OnActivated.AddListener(OnSkinActivated);
            m_AnimalSkin.OnDeactivated.AddListener(OnSkinDeactivated);

            m_PlayerVR.WizardPlayer.OnDie += OnPlayerDie;
            m_PlayerVR.WizardPlayer.OnReborn += OnPlayerReborn;
        }

        private void OnPlayerDie()
        {
            if (!m_IsAnimalSkin)
            {
                return;
            }

            m_IsAnimalSkin = false;

            CancelInvoke(nameof(HideSkin));
            ShowParticle();
        }

        private void OnPlayerReborn()
        {
            if (!m_IsAnimalSkin)
            {
                return;
            }

            m_IsAnimalSkin = false;

            CancelInvoke(nameof(HideSkin));
            ShowParticle();
        }

        private void OnSkinActivated()
        {
            CancelInvoke(nameof(HideSkin));

            m_IsAnimalSkin = true;
            ActivateAnimalState();
            
            ShowParticle();
            Invoke(nameof(HideSkin), m_Duration);
        }

        private void OnSkinDeactivated()
        {
            m_IsAnimalSkin = false;

            RestorePrevState();
            CancelInvoke(nameof(HideSkin));
            ShowParticle();
        }

        private void ActivateAnimalState()
        {
            if (m_CurrentPlayerStateSO == null)
            {
                m_CurrentPlayerStateSO = ScriptableObject.CreateInstance<PlayerStateSO>();
            }
            
            m_PrevPlayerStateSO = m_PlayerVR.WizardPlayer.CurrentPlayerState;
            m_CurrentPlayerStateSO.PasteCopyValue(m_PrevPlayerStateSO);
            m_CurrentPlayerStateSO.CanCast = m_CanCast;

            // TODO: могут быть пробелмы с синхронизацией, но этот метод вызывается через RPC у SkinCollection, поэтому вряд ли
            m_PlayerVR.WizardPlayer.PlayerState.SetState(m_CurrentPlayerStateSO);
            //m_PlayerVR.SetState(m_CurrentPlayerStateSO);
        }

        private void RestorePrevState()
        {
            if (m_PrevPlayerStateSO == null || m_CurrentPlayerStateSO == null)
            {
                return;
            }

            //if (m_PlayerVR.WizardPlayer.CurrentPlayerState != m_PrevPlayerStateSO)
            if (m_PlayerVR.WizardPlayer.CurrentPlayerState != m_CurrentPlayerStateSO)
            {
                return;
            }

            m_PlayerVR.SetState(m_PrevPlayerStateSO);
        }

        private void HideSkin()
        {
            /*
            if (photonView.IsMine)
            {
                m_SkinCollection.RestoreSkin();
            }
            */

            CancelInvoke(nameof(HideSkin));
            m_IsAnimalSkin = false;

            ShowParticle();
            m_SkinCollection.RpcRestoreSkin();
        }

        private void ShowParticle()
        {
            //m_ParticleSystem.transform.position = m_AnimalSkin.transform.position;
            m_ParticleSystem.transform.position = m_ParticlePoint.transform.position;
            m_ParticleSystem.Play();
        }

        #region Debug

        [ContextMenu("Set Animal Skin")]
        private void SetAnimalSkin()
        {
            m_SkinCollection.SetAnimalDefaultSkin();
        }

        #endregion
    }
}