using System;
using System.ComponentModel.DataAnnotations;
using AdminService.Enums;
using Newtonsoft.Json;

namespace AdminService.ViewModels.Identity
{
    public class UserUpdatePatch
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
          
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set;  }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }           
    }
}
