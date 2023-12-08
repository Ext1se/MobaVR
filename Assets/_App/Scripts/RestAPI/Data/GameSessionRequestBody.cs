using System;
using Newtonsoft.Json;

namespace MobaVR
{
    public  class GameSessionRequestBody
    {
        [JsonProperty("game_version")]
        public string GameVersion { get; set; }

        [JsonProperty("game_id")]
        public long GameId { get; set; }

        [JsonProperty("count_players")]
        public long CountPlayers { get; set; }

        [JsonProperty("club_id")]
        public long ClubId { get; set; }
        
        [JsonProperty("start_time")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTimeOffset EndTime { get; set; }
    }
}
