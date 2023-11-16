using System;
using Newtonsoft.Json;

namespace MobaVR
{

    public partial class AccessToken
    {
        [JsonProperty("access_token")]
        public string AccessTokenAccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("refresh_token_expired_at")]
        public DateTimeOffset RefreshTokenExpiredAt { get; set; }

        [JsonProperty("access_token_expired_at")]
        public DateTimeOffset AccessTokenExpiredAt { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
