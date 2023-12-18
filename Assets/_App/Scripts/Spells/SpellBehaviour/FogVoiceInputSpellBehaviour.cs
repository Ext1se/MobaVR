using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class FogVoiceInputSpellBehaviour : VoiceInputSpellBehaviour
    {
        [Header("Healing")]
        [SerializeField] private FogSpell m_FogSpell;
        [SerializeField] private float m_FogDelay = 1f;

        private bool m_CanFog = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_CanFog = true;
        }

        protected override void ExecuteVoice()
        {
            base.ExecuteVoice();
            if (!m_CanFog)
            {
                return;
            }

            if (m_PlayerVR == null)
            {
                return;
            }

            m_CanFog = false;
            
            if (m_GameSession.Mode.GameModeType is GameModeType.PVP or GameModeType.MOBA or GameModeType.LOBBY)
            {
                CreateFog();
                Invoke(nameof(EnableFog), m_FogDelay);
            }

            if (m_GameSession.Mode.GameModeType is GameModeType.PVE or GameModeType.TD)
            {
                BlindMonsters();
            }
        }

        private void CreateFog()
        {
            GameObject networkSpell = PhotonNetwork.Instantiate($"Spells/{m_FogSpell.name}",
                                                                Vector3.zero,
                                                                Quaternion.identity);

            if (networkSpell.TryGetComponent(out Spell spell))
            {
                m_CanFog = false;

                string spellName = $"{m_FogSpell.name}";
                networkSpell.name = spellName;

                spell.OnInitSpell += () => OnInitSpell(spell);
                spell.OnDestroySpell += () => OnDestroySpell(spell);

                spell.Init(m_PlayerVR.WizardPlayer, m_PlayerVR.TeamType);
            }
        }

        private void BlindMonsters()
        {
            Monster[] monsters = FindObjectsOfType<Monster>();
            foreach (Monster monster in monsters)
            {
                monster.Blind();
            }
        }

        private void OnDestroySpell()
        {
            throw new System.NotImplementedException();
        }

        private void OnInitSpell(Spell spell)
        {
            m_CanFog = false;
        }

        private void OnDestroySpell(Spell spell)
        {
            if (spell != null)
            {
                spell.OnInitSpell -= () => OnInitSpell(spell);
                spell.OnDestroySpell -= () => OnDestroySpell(spell);
            }

            m_CanFog = true;
        }

        protected void EnableFog()
        {
            m_CanFog = true;
        }
    }
}