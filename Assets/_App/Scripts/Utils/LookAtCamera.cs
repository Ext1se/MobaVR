using System;
using UnityEngine;

namespace MobaVR.Utils
{
    public class LookAtCamera: MonoBehaviour
    {
        private Camera m_Camera;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        private void Update()
        {
            if (m_Camera != null)
            {
                transform.LookAt(m_Camera.transform);
            }
            else
            {
                m_Camera = Camera.main;
            }
        }
    }
}