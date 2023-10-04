using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR
{
    [RequireComponent(typeof(PhotonView))]
    public class HitPlayerInfo : MonoBehaviourPun
    {
        [SerializeField] private float m_HitCooldown = 20f;
        private WizardPlayer m_Wizard;

        private GameStatistics m_GameStatistics;

        [SerializeField] [ReadOnly] private List<HitData> m_Hits = new();
        private List<PlayerVR> m_HitPlayers = new List<PlayerVR>();
        private PlayerVR m_LastHitPlayer;
        private PlayerVR m_Killer;
        private float m_CurrentTime = 0f;

        private void OnEnable()
        {
            if (m_Wizard != null)
            {
                m_Wizard.OnPlayerHit += OnPlayerHit;
                m_Wizard.OnPlayerDie += OnPlayerDie;
                m_Wizard.OnReborn += OnPlayerReborn;
            }
        }

        private void OnDisable()
        {
            if (m_Wizard != null)
            {
                m_Wizard.OnPlayerHit -= OnPlayerHit;
                m_Wizard.OnPlayerDie -= OnPlayerDie;
                m_Wizard.OnReborn -= OnPlayerReborn;
            }
        }

        private void Awake()
        {
            m_Wizard = GetComponent<WizardPlayer>();
            m_GameStatistics = FindObjectOfType<GameStatistics>();
            Reset();
        }

        private void OnPlayerHit(HitData hitData)
        {
            if (hitData.PlayerVR == null)
            {
                return;
            }

            m_LastHitPlayer = hitData.PlayerVR;

            HitData containHitData = m_Hits.Find(data => data.PlayerVR == hitData.PlayerVR);
            if (containHitData != null)
            {
                containHitData.DateTime = DateTime.Now;
            }
            else
            {
                hitData.DateTime = DateTime.Now;
                m_Hits.Add(hitData);
            }
            
            /*
            if (!m_HitPlayers.Contains(hitData.PlayerVR))
            {
                m_HitPlayers.Add(hitData.PlayerVR);
            }
            */
        }

        private void OnPlayerDie(HitData hitData)
        {
            m_Killer = hitData.PlayerVR != null ? hitData.PlayerVR : m_LastHitPlayer;

            if (m_Killer != null)
            {
                //m_Wizard.PlayerVR.DieView.SetDieInfo(m_Killer.photonView.Owner.NickName);
                m_Wizard.PlayerVR.DieView.SetDieInfo(m_Killer.PlayerData.NickName);
                //SendDeathData();
            }
            
            SendDeathData();
        }

        private void OnPlayerReborn()
        {
            Reset();
        }

        private void SendDeathData()
        {
            m_Hits.RemoveAll(data =>
            {
                DateTime dateTimeNow = DateTime.Now;
                TimeSpan delta = dateTimeNow - data.DateTime;

                return delta.TotalSeconds > m_HitCooldown;
            });
            
            m_HitPlayers = m_Hits.Select(data => data.PlayerVR).ToList();
            if (m_Killer != null)
            {
                m_HitPlayers.Remove(m_Killer);
            }

            DeathPlayerData deathPlayerData = new DeathPlayerData()
            {
                DeadPlayer = m_Wizard.PlayerVR,
                KillPlayer = m_Killer,
                AssistPlayers = m_HitPlayers
            };

            if (m_GameStatistics != null)
            {
                m_GameStatistics.SendDeathData(deathPlayerData);
            }
        }

        private void Reset()
        {
            m_HitPlayers.Clear();
            m_Hits.Clear();
            m_Killer = null;
            m_LastHitPlayer = null;
            m_CurrentTime = 0f;
        }
    }
}