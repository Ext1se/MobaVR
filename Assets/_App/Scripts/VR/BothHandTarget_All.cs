using System;
using Photon.Pun;
using UnityEngine;

namespace MobaVR
{
    public class BothHandTarget_All : MonoBehaviourPun
    {
        [SerializeField] private InputVR m_InputVR;
        [SerializeField] private Transform m_LeftTarget;
        [SerializeField] private Transform m_RightTarget;


        public bool useRotation = true;
        public bool useSimpleRotation = true;
        public bool useAverageVector = true;
        public bool useAverageVector2 = true;
        public bool yseAngle = true;
        public float kValue = 0.5f;
        public bool isNormal = false;
        public bool isInvert = false;
        public bool useLerpUnclamped = true;
        public bool useDot = true;
        public bool blockInverse = true;

        public float angle = 0f;
        public float dot = 0f; //use it

        public bool useVector = true;

        public bool inverseX;
        public bool inverseY;
        public bool inverseZ;

        public float deltaX;
        public float deltaY;
        public float deltaZ;

        private void Update()
        {
            if (photonView.IsMine)
            {
                Vector3 position = (m_LeftTarget.position + m_RightTarget.position) / 2f;

                transform.position = position;

                angle = Quaternion.Angle(m_LeftTarget.transform.rotation, m_RightTarget.transform.rotation);
                dot = Quaternion.Dot(m_LeftTarget.transform.rotation, m_RightTarget.transform.rotation);


                //Vector3 end1 = m_LeftTarget.transform.position + m_LeftTarget.forward * 10;
                //Vector3 end2 = m_RightTarget.transform.position + m_RightTarget.forward * 10;
                
                Vector3 end1 = m_LeftTarget.transform.position + (GetAxis(m_LeftTarget, leftForward) * 10);
                Vector3 end2 = m_RightTarget.transform.position + (GetAxis(m_RightTarget, rightForward) * 10);

                Vector3 end3 = (end1 + end2) / 2f;
                Gizmos.color = Color.red;

                transform.LookAt(end3);


                if (useRotation)
                {
                    Quaternion rotation = Quaternion.identity;
                    if (useLerpUnclamped)
                    {
                        rotation = Quaternion.LerpUnclamped(m_LeftTarget.transform.rotation,
                                                            m_RightTarget.transform.rotation,
                                                            kValue);
                    }
                    else
                    {
                        rotation = Quaternion.Lerp(m_LeftTarget.transform.rotation,
                                                   m_RightTarget.transform.rotation,
                                                   kValue);
                    }


                    if (isInvert)
                    {
                        rotation = Quaternion.Inverse(rotation);
                    }

                    if (isNormal)
                    {
                        {
                            rotation = Quaternion.Normalize(rotation);
                        }
                    }

                    if (blockInverse)
                    {
                        if (dot > 0)
                        {
                            transform.rotation = rotation;
                        }
                    }
                    else
                    {
                        transform.rotation = rotation;
                    }

                    if (useDot)
                    {
                        int k = dot > 0 ? 1 : -1;
                        //transform.rotation = Quaternion.Euler(k * rotation.eulerAngles);

                        transform.forward *= k;
                    }
                }

                if (useSimpleRotation)
                {
                    deltaX = (m_LeftTarget.transform.rotation.eulerAngles.x +
                              m_RightTarget.transform.rotation.eulerAngles.x) / 2f;

                    if (inverseX)
                    {
                        deltaX *= -1;
                    }

                    deltaY = (m_LeftTarget.transform.rotation.eulerAngles.y +
                              m_RightTarget.transform.rotation.eulerAngles.y) / 2f;

                    if (inverseY)
                    {
                        deltaY *= -1;
                    }

                    deltaZ = (m_LeftTarget.transform.rotation.eulerAngles.z +
                              m_RightTarget.transform.rotation.eulerAngles.z) / 2f;

                    if (inverseZ)
                    {
                        deltaZ *= -1;
                    }

                    Vector3 eulerAngles = new Vector3(deltaX, deltaY, deltaZ);
                    transform.rotation = Quaternion.Euler(eulerAngles);
                }

                if (useVector)
                {
                    Vector3 vector3 = (m_LeftTarget.forward + m_RightTarget.forward) / 2f;
                    transform.forward = vector3;
                }

                if (useAverageVector)
                {
                    var midway = m_LeftTarget.forward + m_RightTarget.forward;
                    midway = midway.normalized;
                    transform.forward = midway;
                }

                if (useAverageVector2)
                {
                    var point1 = (m_LeftTarget.position + m_LeftTarget.forward * 10) - m_LeftTarget.position;
                    var point2 = (m_RightTarget.position + m_RightTarget.forward * 10) - m_RightTarget.position;

                    //var perp = Vector3.Cross(point1, point2);
                    var perp = Vector3.Cross(point1, point2);
                    var midway = (point1 + point2) / 2f;
                    transform.forward = midway;
                }

                if (yseAngle)
                {
                    float angle = Vector2.Angle(m_LeftTarget.position, m_RightTarget.position);
                }
            }
        }

        public enum AxisToRotate
        {
            Back,
            Down,
            Forward,
            Left,
            Right,
            Up,
            Zero
        };

        public Vector3 GetAxis(Transform targetTransform, AxisToRotate axis)
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

        public AxisToRotate leftForward = AxisToRotate.Forward;
        public AxisToRotate rightForward = AxisToRotate.Forward;


        //public Vector3 leftForward = Vector3.forward;
        //public Vector3 rightForward = Vector3.forward;

        private void OnDrawGizmos()
        {
            Vector3 position = (m_LeftTarget.position + m_RightTarget.position) / 2f;
            //Vector3 end1 = m_LeftTarget.transform.position + m_LeftTarget.forward * 10;
            Vector3 end1 = m_LeftTarget.transform.position + (GetAxis(m_LeftTarget, leftForward) * 10);
            //Vector3 end2 = m_RightTarget.transform.position + m_RightTarget.forward * 10;
            Vector3 end2 = m_RightTarget.transform.position + (GetAxis(m_RightTarget, rightForward) * 10);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, end1);
            Gizmos.DrawLine(position, end2);

            Vector3 end3 = (end1 + end2) / 2f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, end3);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_LeftTarget.position, end1);
            Gizmos.DrawLine(m_RightTarget.position, end2);
        }
    }
}