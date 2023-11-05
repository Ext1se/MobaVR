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
        private const string AUTH_ERROR = "Auth error";

        //private const string BASE_API_PATH_COMMON = "https://api.portal-vr.pro:5000/";
        private const string BASE_API_PATH_COMMON = "http://51.250.54.116:8000/";
        //private const string BASE_API_PATH_STATISTICS = "https://api.portal-vr.pro:5001/";
        private const string BASE_API_PATH_STATISTICS = "http://51.250.54.116:8001/";

        private const string PATH_COMPANY = "company/";

        private LocalRepository m_LocalRepository;
        private string m_Token = null;

        protected override void Awake()
        {
            base.Awake();
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

        #region Club

        public override void GetClubInfo(int idClub, RequestResultCallback<Club> callback)
        {
            StartCoroutine(SendRequest_GetClubInfo(idClub, callback));

        }

        private IEnumerator SendRequest_GetClubInfo(int idClub,
                                                    RequestResultCallback<Club> callback)
        {
            string url = $"{BASE_API_PATH_COMMON}clubs/{idClub}";
            UnityWebRequest www = UnityWebRequest.Get(url);
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
                        Club club = JsonConvert.DeserializeObject<Club>(www.downloadHandler.text);
                        if (club != null)
                        {
                            callback.OnSuccess?.Invoke(club);
                        }
                        else
                        {
                            callback.OnSuccess?.Invoke(null);
                        }

                        break;
                    case (404):
                    {
                        callback.OnError?.Invoke("Club is not exist");
                        break;
                    }
                    case (422):
                    {
                        callback.OnError?.Invoke("Validation Error");
                        break;
                    }
                    default:
                        callback.OnSuccess?.Invoke(null);
                        break;
                }
            }

            callback.OnFinish?.Invoke();
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
            ValidateLicense(key, m_AppSetting.IdGame, m_AppSetting.IdClub, callback);
        }

        public override void ValidateLicense(string key, int idGame, int idClub, RequestResultCallback<LicenseKeyResponse> callback)
        {
            /*
            if (IsEmptyToken())
            {
                callback.OnError?.Invoke(MESSAGE_TOKEN_EMPTY);
                callback.OnFinish?.Invoke();
                return;
            }
            */

            StartCoroutine(SendRequest_ValidateLicense(key, idGame, idClub, callback));
        }

        private IEnumerator SendRequest_ValidateLicense(string key, int idGame, int idClub, RequestResultCallback<LicenseKeyResponse> callback)
        {
            //string url = $"{BASE_API_PATH_COMMON}/{PATH_COMPANY}/company";
            //url = $"{url}?license_key={key}";
            string url = $"{BASE_API_PATH_COMMON}verify_keys";
            url = $"{url}?key={key}&game_id={idGame}&club_id={idClub}";

            UnityWebRequest www = UnityWebRequest.Get(url);
            //www.SetRequestHeader("Authorization", "Bearer " + m_Token);
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
                    case (404):
                    {
                        callback.OnError?.Invoke("A key with such id is not exist");
                        break;
                    }
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