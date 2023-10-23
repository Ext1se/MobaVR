using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class DistanceMonitorLocal : MonoBehaviourPun
    {
        [SerializeField] private bool m_UseLocalPosition = true;
        [SerializeField] private float m_Delay = 0.1f;

        [SerializeField] [ReadOnly] private float m_Sum = 0;
        private Vector3 m_PreviousPosition = Vector3.zero;

        public float Sum => m_Sum;

        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                m_PreviousPosition = m_UseLocalPosition ? transform.localPosition : transform.position;
                InvokeRepeating(nameof(CheckDistance), 0, m_Delay);
            }
        }

        private void OnDisable()
        {
            if (photonView.IsMine)
            {
                CancelInvoke(nameof(CheckDistance));
            }
        }

        private void CheckDistance()
        {
            Vector3 currentPosition = m_UseLocalPosition ? transform.localPosition : transform.position;
            m_Sum += Vector3.Distance(currentPosition, m_PreviousPosition);
            m_PreviousPosition = currentPosition;
        }
    }
}