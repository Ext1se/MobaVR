using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobaVR
{
    public class PigSpellBehaviour : InputSpellBehaviour
    {
        [SerializeField] private PigSpell m_PigSpellPrefab;
        [SerializeField] private GameObject m_PigLine;

        private PigSpell m_PigSpell;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_PigLine != null)
            {
                m_PigLine.SetActive(false);
            }
        }

        protected override void OnDisable()
        {
            if (m_PigLine != null)
            {
                m_PigLine.SetActive(false);
            }
        }

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

            if (m_PigLine != null)
            {
                m_PigLine.SetActive(true);
            }

            m_IsPerformed = true;
        }

        protected override void OnCanceledCast(InputAction.CallbackContext context)
        {
            base.OnCanceledCast(context);
            
            if (m_PigLine != null)
            {
                m_PigLine.SetActive(false);
            }

            if (!m_IsPerformed)
            {
                return;
            }
            
            CreateSpell(m_MainHandInputVR.FingerPoint);
            Shoot(m_MainHandInputVR.Grabber.transform.forward);
            WaitCooldown();
            
            m_IsPerformed = false;
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

        protected override void Interrupt()
        {
            base.Interrupt();
            if (m_PigLine != null)
            {
                m_PigLine.SetActive(false);
            }

            m_IsPerformed = false;
        }

        protected override void SetAvailable()
        {
            base.SetAvailable();
            m_IsPerformed = false;
        }
    }
}