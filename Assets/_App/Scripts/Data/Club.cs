using System;
using Newtonsoft.Json;

namespace MobaVR
{
    public class Club
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("inst_url")]
        public Uri InstUrl { get; set; }

        [JsonProperty("company_id")]
        public long CompanyId { get; set; }

        [JsonProperty("vk_url")]
        public Uri VkUrl { get; set; }

        [JsonProperty("tg_url")]
        public Uri TgUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("created_datetime")]
        public DateTimeOffset CreatedDatetime { get; set; }

        [JsonProperty("schedule_weekdays")]
        public string ScheduleWeekdays { get; set; }

        [JsonProperty("phone_url")]
        public Uri PhoneUrl { get; set; }

        [JsonProperty("short_title")]
        public string ShortTitle { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("schedule_weekend")]
        public string ScheduleWeekend { get; set; }

        [JsonProperty("city_id")]
        public long CityId { get; set; }

        [JsonProperty("reservation_url")]
        public Uri ReservationUrl { get; set; }

        [JsonProperty("whatsapp_link")]
        public Uri WhatsappLink { get; set; }
    }
}
