using System;
using Newtonsoft.Json;

namespace AdminService.ViewModels.Identity
{
    public class Tokens
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }      

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public DateTime? Expiration { get; set; }

        [JsonProperty(PropertyName = "lastChecked")]
        public DateTime? LastChecked { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }      

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "rowNumber")]
        public Int64 RowNumber { get; set; }
    }
}
