using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class BothHandTarget : MonoBehaviourPun
    {
        private enum AxisToRotate
        {
            Back,
            Down,
            Forward,
            Left,
            Right,
            Up,
            Zero
        }
        
        [SerializeField] private Transform m_LeftTarget;
        [SerializeField] private Transform m_RightTarget;

        [SerializeField] private AxisToRotate m_LeftForward = AxisToRotate.Forward;
        [SerializeField] private AxisToRotate m_RightForward = AxisToRotate.Forward;
        
        private void Update()
        {
            if (photonView.IsMine)
            {
                Vector3 position = (m_LeftTarget.position + m_RightTarget.position) / 2f;
                Vector3 leftEndPoint = m_LeftTarget.transform.position + (GetAxis(m_LeftTarget, m_LeftForward) * 10);
                Vector3 rightEndPoint = m_RightTarget.transform.position + (GetAxis(m_RightTarget, m_RightForward) * 10);
                Vector3 endPoint = (leftEndPoint + rightEndPoint) / 2f;
                
                transform.position = position;
                transform.LookAt(endPoint);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 position = (m_LeftTarget.position + m_RightTarget.position) / 2f;
            Vector3 leftEndPoint = m_LeftTarget.transform.position + (GetAxis(m_LeftTarget, m_LeftForward) * 10);
            Vector3 rightEndPoint = m_RightTarget.transform.position + (GetAxis(m_RightTarget, m_RightForward) * 10);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, leftEndPoint);
            Gizmos.DrawLine(position, rightEndPoint);

            Vector3 endPoint = (leftEndPoint + rightEndPoint) / 2f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, endPoint);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_LeftTarget.position, leftEndPoint);
            Gizmos.DrawLine(m_RightTarget.position, rightEndPoint);
        }
        
        private Vector3 GetAxis(Transform targetTransform, AxisToRotate axis)
        {
            switch (axis)
            {
                case AxisToRotate.Back:
                    return -targetTransform.forward;
                case AxisToRotate.Forward:
                    return targetTransform.forward;
                case AxisToRotate.Down:
                    return -targetTransform.up;
                case AxisToRotate.Up:
                    return targetTransform.up;
                case AxisToRotate.Left:
                    return -targetTransform.right;
                case AxisToRotate.Right:
                    return targetTransform.right;
                case AxisToRotate.Zero:
                default:
                    return Vector3.zero;
            }
        }
    }
}