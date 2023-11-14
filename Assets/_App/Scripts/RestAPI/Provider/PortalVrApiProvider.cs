using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MobaVR
{
    public class PortalVrApiProvider : BaseApiProvider
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
            StartCoroutine(SendRequest_GetToken(username, password, callback));

            //JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        }

        private IEnumerator SendRequest_GetToken(string username, string password,
                                                 RequestResultCallback<AccessToken> callback)
        {
            //TODO
            yield break;
        }

        #endregion

        #region License

        public override void ValidateLicense(string key, RequestResultCallback<bool> callback)
        {
            /*
            if (IsEmptyToken())
            {
                callback.OnError?.Invoke(MESSAGE_TOKEN_EMPTY);
                callback.OnFinish?.Invoke();
                return;
            } 
            */
        }

        public override void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback)
        {
            if (IsEmptyToken())
            {
                callback.OnError?.Invoke(MESSAGE_TOKEN_EMPTY);
                callback.OnFinish?.Invoke();
                return;
            }

            StartCoroutine(SendRequest_ValidateLicense(key, callback));
        }

        private IEnumerator SendRequest_ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback)
        {
            string url = $"{BASE_API_PATH_COMMON}/{PATH_COMPANY}/company";
            url = $"{url}?license_key={key}";

            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Authorization", "Bearer " + m_Token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                callback.OnError?.Invoke(www.error);
            }
            else
            {
                switch (www.responseCode)
                {
                    case (200):
                        LicenseKeyResponse licenseKeyResponse =
                            JsonConvert.DeserializeObject<LicenseKeyResponse>(www.downloadHandler.text);
                        if (licenseKeyResponse != null)
                        {
                            callback.OnSuccess?.Invoke(licenseKeyResponse);
                        }
                        else
                        {
                            callback.OnSuccess?.Invoke(null);
                        }

                        break;
                    default:
                        callback.OnSuccess?.Invoke(null);
                        break;
                }
            }

            callback.OnFinish?.Invoke();
        }

        #endregion

        #region Statistics

        public override void SendGameSession(string key, RequestResultCallback<bool> callback)
        {
        }

        private IEnumerator SendRequest_SendGameSession(string key, RequestResultCallback<bool> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://api.z-boom.ru/balance/monets-list");
            www.SetRequestHeader("Authorization", "Bearer " + m_Token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield break;
        }

        #endregion
    }
}