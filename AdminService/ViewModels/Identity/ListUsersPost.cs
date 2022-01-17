using System;
using System.ComponentModel.DataAnnotations;
using AdminService.Enums;
using Newtonsoft.Json;

namespace AdminService.ViewModels.Identity
{
    public class ListUsersPost
    {
        public ListUsersPost()
        {
            Name = null;
            Email = null;
            Role = null;
            Status = -1;
            IsActive = true;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int? Status { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }        
    }
}
