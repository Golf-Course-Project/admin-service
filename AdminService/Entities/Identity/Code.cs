using System;
using System.ComponentModel.DataAnnotations;

using AdminService.Enums;

namespace AdminService.Entities.Identity
{
    public class Code
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }   
        public UserStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpires { get; set; }
    }
}
