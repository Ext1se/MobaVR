using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MobaVR
{
    public class LocalApiProvider : BaseApiProvider
    {
        private const string MESSAGE_TOKEN_EMPTY = "Token is empty";

        private const string BASE_API_PATH_COMMON = "https://api.portal-vr.pro:5000/";
        private const string BASE_API_PATH_STATISTICS = "https://api.portal-vr.pro:5001/";

        private const string PATH_COMPANY = "company/";

        private LocalRepository m_LocalRepository;
        private string m_Token = null;

        private void Awake()
        {
            m_LocalRepository = new LocalRepository();
        }

        #region Token

        private bool IsEmptyToken()
        {
            return string.IsNullOrEmpty(m_Token);
        }

        public override void GetToken(string username, string password, RequestResultCallback<AccessToken> callback)
        {
            
        }

        #endregion

        #region License

        public override void ValidateLicense(string key, RequestResultCallback<bool> callback)
        {
        }

        public override void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback)
        {
            LicenseKeyResponse licenseKeyResponse = new LicenseKeyResponse();
            callback.OnSuccess?.Invoke(licenseKeyResponse);
            callback.OnFinish?.Invoke();
        }

        #endregion

        #region Statistics

        public override void SendGameSession(string key, RequestResultCallback<bool> callback)
        {
        }

        #endregion
    }
}