using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MobaVR
{
    public abstract class BaseApiProvider : MonoBehaviour, RestPortalVR
    {
        public abstract void GetToken(string username, string password, RequestResultCallback<AccessToken> callback);
        public abstract void ValidateLicense(string key, RequestResultCallback<bool> callback);
        public abstract void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback);
        public abstract void SendGameSession(string key, RequestResultCallback<bool> callback);
    }
}