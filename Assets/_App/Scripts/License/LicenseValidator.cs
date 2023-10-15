using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class LicenseValidator : MonoBehaviour
    {
        [Header("Dependencies")]
        
        [Header("View")]
        [SerializeField] private Button m_ValidateButton;
        [SerializeField] private TMP_InputField m_InputField;

        private void Awake()
        {
            
        }

        public void Validate()
        {
            m_ValidateButton.interactable = false;
            string key = m_InputField.text;
        }
    }
}