using UnityEngine;

namespace MobaVR
{
    public class HealingVoiceInputSpellBehaviour : VoiceInputSpellBehaviour
    {
        [Header("Healing")] [SerializeField] private float m_HealLocalPlayer = 20f;
        [SerializeField] private float m_HealTeammate = 10f;
        [SerializeField] private float m_HealEnemyTeam = 10f;
        [SerializeField] private float m_HealDelay = 1f;
        [SerializeField] private bool m_IsHealTeammates = true;
        [SerializeField] private bool m_IsHealEnemyTeam = true;

        private bool m_CanHealing = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_CanHealing = true;
        }

        protected override void ExecuteVoice()
        {
            base.ExecuteVoice();
            if (!m_CanHealing)
            {
                return;
            }

            if (m_PlayerVR == null)
            {
                return;
            }

            m_CanHealing = false;
            if (!m_IsHealTeammates)
            {
                m_PlayerVR.WizardPlayer.Heal(m_HealLocalPlayer);
            }
            else
            {
                Team friendTeam = m_PlayerVR.Team;
                foreach (PlayerVR playerVR in friendTeam.Players)
                {
                    if (playerVR == m_PlayerVR)
                    {
                        playerVR.WizardPlayer.Heal(m_HealLocalPlayer);
                    }
                    else
                    {
                        playerVR.WizardPlayer.Heal(m_HealTeammate);
                    }
                }

                if (m_IsHealEnemyTeam && m_GameSession.Mode.GameModeType is GameModeType.PVE or GameModeType.TD)
                {
                    Team enemyTeam = m_PlayerVR.Team.TeamType == TeamType.RED
                        ? m_GameSession.BlueTeam
                        : m_GameSession.RedTeam;

                    foreach (PlayerVR playerVR in enemyTeam.Players)
                    {
                        playerVR.WizardPlayer.Heal(m_HealEnemyTeam);
                    }
                }
            }

            if (m_GameSession.Mode.GameModeType == GameModeType.LOBBY)
            {
                ApplyForDummy();
            }
            
            Invoke(nameof(EnableHealing), m_HealDelay);
        }


        private void ApplyForDummy()
        {
            Dummy dummy = FindObjectOfType<Dummy>();
            if (dummy != null)
            {
                dummy.ShowHeal();
            }
        }

        protected void EnableHealing()
        {
            m_CanHealing = true;
        }
    }
}