using UnityEngine;

namespace MobaVR
{
    public abstract class BaseApiProvider : MonoBehaviour, RestPortalVR
    {
        public static BaseApiProvider Instance;
        
        [SerializeField] protected AppSetting m_AppSetting;

        protected virtual void Awake()
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

        public abstract void GetToken(string username, string password, RequestResultCallback<AccessToken> callback);
        public abstract void ValidateLicense(string key, RequestResultCallback<bool> callback);
        public abstract void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback);
        public abstract void ValidateLicense(string key, int idGame, int idClub, RequestResultCallback<LicenseKeyResponse> callback);
        public abstract void SendGameSession(string key, RequestResultCallback<bool> callback);
    }
}