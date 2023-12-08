using System;
using Newtonsoft.Json;

namespace MobaVR
{
    public  class GameSessionStat
    {
        [JsonProperty("game_version")]
        public string GameVersion { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("club_id")]
        public long ClubId { get; set; }

        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        [JsonProperty("game_id")]
        public long GameId { get; set; }

        [JsonProperty("count_players")]
        public long CountPlayers { get; set; }
    }
}