using System;
using System.ComponentModel.DataAnnotations;
using AdminService.Enums;
using Newtonsoft.Json;

namespace AdminService.ViewModels.Identity
{
    public class Users
    {              
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }      

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set;  }

        [JsonProperty(PropertyName = "status")]
        public UserStatus Status { get; set;  }

        [JsonProperty(PropertyName = "statusText")]
        public string StatusText { get; set; }

        [JsonProperty(PropertyName = "isLocked")]
        public Boolean IsLocked { get; set; }

        [JsonProperty(PropertyName = "loginAttempts")]
        public int LoginAttempts { get; set; }    

        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(PropertyName = "dateUpdated")]
        public DateTime? DateUpdated { get; set; }

        [JsonProperty(PropertyName = "dateLastAttempt")]
        public DateTime? DateLastAttempt { get; set; }

        [JsonProperty(PropertyName = "dateLocked")]
        public DateTime? DateLocked { get; set; }

        [JsonProperty(PropertyName = "dateDeleted")]
        public DateTime? DateDeleted { get; set; }

        [JsonProperty(PropertyName = "rowNumber")]
        public Int64 RowNumber { get; set; }

    }
}
