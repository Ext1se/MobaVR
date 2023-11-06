using System;
using DG.Tweening;
using Michsky.MUIP;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MobaVR
{
    public class BigShield : Spell
    {
        [SerializeField] private Renderer m_Renderer;
        [SerializeField] private Collider m_Collider;
        [SerializeField] private Collider m_PhysicCollider;
        [SerializeField] private NavMeshObstacle m_NavMeshObstacle;
        [SerializeField] private SliderManager m_Slider;
        [SerializeField] private float m_PlaceAlpha = 0.25f;
        [SerializeField] private float m_SecondsBeforeHide = 1.0f;

        public UnityEvent OnPrepare;
        public UnityEvent OnReadyShow;
        public UnityEvent OnReadyHide;
        public UnityEvent OnShow;
        public UnityEvent OnCast;
        public UnityEvent OnHide;

        private void Awake()
        {
            m_Renderer.enabled = false;
            m_Collider.enabled = false;
            m_PhysicCollider.enabled = false;
            if (m_NavMeshObstacle != null)
            {
                m_NavMeshObstacle.enabled = false;
            }
            m_Slider.gameObject.SetActive(false);
        }

        public void Prepare()
        {
            m_Slider.gameObject.SetActive(false);
            
            m_Renderer.enabled = true;
            m_Collider.enabled = false;
            if (m_PhysicCollider != null)
            {
                m_PhysicCollider.enabled = false;
            }

            if (m_NavMeshObstacle != null)
            {
                m_NavMeshObstacle.enabled = false;
            }

            Color color = m_Renderer.material.color;
            color.a = 0.1f;
            m_Renderer.material.color = color;
            
            OnPrepare?.Invoke();
            OnReadyShow?.Invoke();
        }

        public void Place()
        {
            //photonView.RPC(nameof(RpcPlace), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcPlace), RpcTarget.All);
        }

        [PunRPC]
        private void RpcPlace()
        {
            OnShow?.Invoke();
            OnCast?.Invoke();
            
            transform.parent = null;

            m_Renderer.enabled = true;
            m_Collider.enabled = true;
            
            if (m_PhysicCollider != null)
            {
                m_PhysicCollider.enabled = true;

                gameObject.layer = m_TeamType == TeamType.RED
                    ? LayerMask.NameToLayer("RedTeam_DetectOnlyEnemyCollision")
                    : LayerMask.NameToLayer("BlueTeam_DetectOnlyEnemyCollision");
            }
            
            if (m_NavMeshObstacle != null)
            {
                m_NavMeshObstacle.enabled = true;
            }

            Color color = m_Renderer.material.color;
            color.a = m_PlaceAlpha;
            m_Renderer.material.color = color;

            m_Slider.gameObject.SetActive(true);
            float sliderValue = 1f;
            DOTween
                .To(() => sliderValue, x => sliderValue = x, 0, m_DestroyLifeTime)
                .OnUpdate(() =>
                {
                    m_Slider.mainSlider.value = sliderValue;
                })
                .OnComplete(() => { m_Slider.gameObject.SetActive(false); });
            
            //TODO: READY HIDE???
            float timeBeforeDestroy = m_DestroyLifeTime - m_SecondsBeforeHide;
            Invoke(nameof(ReadyHide), timeBeforeDestroy);

            if (photonView.IsMine)
            {
                Invoke(nameof(DestroySpell), m_DestroyLifeTime);
            }
        }

        public void ReadyHide()
        {
            OnReadyHide?.Invoke();
        }

        public void DestroySpell()
        {
            if (gameObject.activeSelf)
            {
                photonView.RPC(nameof(RpcDestroyShield), RpcTarget.All);
                StopAllCoroutines();
            }
        }

        [PunRPC]
        private void RpcDestroyShield()
        {
            OnDestroySpell?.Invoke();
            OnHide?.Invoke();

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        public void Hit(HitData hitData)
        {
            
        }
    }
}