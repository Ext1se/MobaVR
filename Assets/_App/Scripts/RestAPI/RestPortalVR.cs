namespace MobaVR
{
    public interface RestPortalVR
    {
        #region Auth

        public void GetToken(string username, string password, RequestResultCallback<AccessToken> callback);

        #endregion

        #region License

        //TODO
        public void ValidateLicense(string key, RequestResultCallback<bool> callback);
        public void ValidateLicense(string key, RequestResultCallback<LicenseKeyResponse> callback);

        public void ValidateLicense(string key,
                                    int idGame,
                                    int idClub,
                                    RequestResultCallback<LicenseKeyResponse> callback);

        #endregion

        #region Statistics

        //TODO
        public void SendGameSession(GameSessionStat gameSessionStat, RequestResultCallback<GameSessionStat> callback);

        #endregion
    }
}