using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaVR
{
    public class PigSpellBehaviour : InputSpellBehaviour
    {
        [SerializeField] private PigSpell m_PigSpellPrefab;

        private PigSpell m_PigSpell;

        protected override void OnPerformedCast(InputAction.CallbackContext context)
        {
            base.OnPerformedCast(context);

            if (!CanCast()
                || HasBlockingSpells()
                || HasBlockingInputs()
               )
            {
                return;
            }

            CreateSpell(m_MainHandInputVR.FingerPoint);
            Shoot(m_MainHandInputVR.Grabber.transform.forward);
            WaitCooldown();
        }

        private void CreateSpell(Transform point)
        {
            GameObject networkPell = PhotonNetwork.Instantiate($"Spells/{m_PigSpellPrefab.name}",
                                                                   point.position,
                                                                   point.rotation);

            if (networkPell.TryGetComponent(out m_PigSpell))
            {
                Transform fireBallTransform = m_PigSpell.transform;
                fireBallTransform.parent = null;
                fireBallTransform.position = point.transform.position;
                fireBallTransform.rotation = Quaternion.identity;

                m_PigSpell.OnInitSpell = () => { m_IsPerformed = true; };
                m_PigSpell.OnDestroySpell = () =>
                {
                    m_IsPerformed = false;
                    m_PigSpell = null;
                    OnCompleted?.Invoke();
                };

                m_PigSpell.Init(m_PlayerVR.WizardPlayer, m_PlayerVR.TeamType);
            }
        }

        private void Shoot(Vector3 direction)
        {
            if (m_PigSpell != null)
            {
                m_PigSpell.Shoot(direction);
            }
        }
    }
}