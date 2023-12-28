using System;
using UnityEngine;

namespace MobaVR
{
    public class FireVoiceInputSpellBehaviour : VoiceInputSpellBehaviour
    {
        [Header("Fire Damage")]
        [SerializeField] private float m_DamageDelay = 0.25f;

        [SerializeField] private float m_OwnerDamage = 5f;
        [SerializeField] private float m_TeamDamage = 1f;
        [SerializeField] private float m_EnemyDamage = 5f;
        [SerializeField] private float m_MonsterDamage = 5f;

        private bool m_CanDamage = false;

        private HitData m_OwnerHitData;
        private HitData m_TeamHitData;
        private HitData m_EnemyHitData;
        private HitData m_MonsterHitData;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_CanDamage = true;
        }

        public override void Init(SpellHandler spellHandler, PlayerVR playerVR)
        {
            base.Init(spellHandler, playerVR);
            
            m_OwnerHitData = GetHitData(m_OwnerDamage);
            m_TeamHitData = GetHitData(m_TeamDamage);
            m_EnemyHitData = GetHitData(m_EnemyDamage);
            m_MonsterHitData = GetHitData(m_MonsterDamage);
        }

        protected override void ExecuteVoice()
        {
            base.ExecuteVoice();
            if (!m_CanDamage)
            {
                return;
            }

            if (m_PlayerVR == null)
            {
                return;
            }

            m_CanDamage = false;

            if (m_OwnerDamage > 0)
            {
                if (m_PlayerVR.WizardPlayer.CurrentHealth > m_OwnerDamage)
                {
                    m_PlayerVR.Damageable.Hit(m_OwnerHitData);
                }
            }

            if (m_GameSession.Mode.GameModeType == GameModeType.LOBBY)
            {
                ApplyForDummy();
            }

            if (m_GameSession.Mode.GameModeType is GameModeType.PVP or GameModeType.MOBA)
            {
                DamagePlayers();
            }
            
            if (m_GameSession.Mode.GameModeType is GameModeType.PVE or GameModeType.TD)
            {
                DamageMonsters();
            }

            Invoke(nameof(EnableVoice), m_DamageDelay);
        }

        private void ApplyForDummy()
        {
            Dummy dummy = FindObjectOfType<Dummy>();
            if (dummy != null)
            {
                dummy.ShowFire();
                dummy.Hit(m_EnemyHitData);
            }
        }

        private void DamagePlayers()
        {
            if (m_GameSession != null && m_TeamDamage > 0)
            {
                Team team = m_PlayerVR.Team.TeamType == TeamType.BLUE ? m_GameSession.BlueTeam : m_GameSession.RedTeam;

                foreach (PlayerVR player in team.Players)
                {
                    if (player != m_PlayerVR)
                    {
                        player.Damageable.Hit(m_TeamHitData);
                    }
                }
            }

            if (m_GameSession != null && m_EnemyDamage > 0)
            {
                Team team = m_PlayerVR.Team.TeamType == TeamType.BLUE ? m_GameSession.RedTeam : m_GameSession.BlueTeam;

                foreach (PlayerVR player in team.Players)
                {
                    player.Damageable.Hit(m_EnemyHitData);
                }
            }
        }

        private void DamageMonsters()
        {
            Monster[] monsters = FindObjectsOfType<Monster>();
            foreach (Monster monster in monsters)
            {
                if (monster.TryGetComponent(out Damageable damageable))
                {
                    damageable.Hit(m_MonsterHitData);
                }
            }
        }

        protected void EnableVoice()
        {
            m_CanDamage = true;
        }

        private HitData GetHitData(float damage)
        {
            HitData hitData = new HitData()
            {
                Action = HitActionType.Damage,
                Amount = damage,
                PlayerVR = m_PlayerVR,
                TeamType = m_PlayerVR.TeamType,
                CanApplyBySelf = true,
                CanApplyForTeammates = true
            };

            return hitData;
        }
    }
}