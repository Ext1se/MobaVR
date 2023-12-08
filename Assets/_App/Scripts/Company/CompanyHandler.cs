using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class CompanyHandler : MonoBehaviour
    {
        public static CompanyHandler Instance;

        [SerializeField] private AppSetting m_AppSetting;
        
        [ReadOnly] public Club Club;
        [ReadOnly] public string LicenseKey;
        [ReadOnly] public LicenseKeyResponse LicenseKeyResponse;

        public AppSetting AppSetting => m_AppSetting;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}