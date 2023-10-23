using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MobaVR
{
    public class LicenseKeyResponse
    {
        [JsonProperty("club_id")]
        public long ClubId { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("created_datetime")]
        public DateTimeOffset CreatedDatetime { get; set; }

        [JsonProperty("game_id")]
        public long GameId { get; set; }
    }
}