using QuickType;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class CompanyHandler : MonoBehaviour
    {
        public static CompanyHandler Instance;
        
        [ReadOnly] public Club Club;
        [ReadOnly] public string LicenseKey;
        [ReadOnly] public LicenseKeyResponse LicenseKeyResponse;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}