using System;
using System.ComponentModel.DataAnnotations;
using AdminService.Enums;
using Newtonsoft.Json;

namespace AdminService.ViewModels.Identity
{
    public class DeletedUsersSearchPost
    {
        public DeletedUsersSearchPost()
        {
            Name = null;              
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }                 
    }
}
