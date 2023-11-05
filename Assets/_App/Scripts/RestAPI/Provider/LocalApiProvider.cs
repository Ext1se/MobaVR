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
        private float m_Delay = 3f;

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
        }

        #endregion

        #region Club

        public override void GetClubInfo(int idClub, RequestResultCallback<Club> callback)
        {
            Club club = new Club()
            {
                Title = "Development club",
                ShortTitle = "ARMA",
                Address = "Moscow",
                Id = 1
            };

            callback.OnSuccess?.Invoke(club);
            callback.OnFinish?.Invoke();
        }

        #endregion

        #region License

        public override void ValidateLicense(string key, RequestResultCallback<bool> callback)
        {
        }

        public override void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback)
        {
            ValidateLicense(key, m_AppSetting.IdGame, m_AppSetting.IdClub, callback);
        }

        public override void ValidateLicense(string key, int idGame, int idClub,
                                             RequestResultCallback<LicenseKeyResponse> callback)
        {
            StartCoroutine(SendRequest_ValidateLicense(key, idGame, idClub, callback));
        }

        private IEnumerator SendRequest_ValidateLicense(string key, int idGame, int idClub,
                                                        RequestResultCallback<LicenseKeyResponse> callback)
        {
            yield return new WaitForSeconds(m_Delay);
            if (key.Equals("123456"))
            {
                LicenseKeyResponse licenseKeyResponse = new LicenseKeyResponse();
                callback.OnSuccess?.Invoke(licenseKeyResponse);
            }
            else
            {
                callback.OnError?.Invoke("Invalid key");
            }

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