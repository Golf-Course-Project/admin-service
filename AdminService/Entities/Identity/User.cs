using AdminService.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdminService.Entities.Identity
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; } = "basic";
        public string IPAddress {get; set;}
        public Enums.UserStatus Status { get; set; } = Enums.UserStatus.NotConfirmed;
        public int LoginAttempts { get; set; }       
        public bool IsDeleted { get; set; } = false;     
        public DateTime? DateLastAttempt { get; set; }
        public DateTime? DateReset { get; set; }
        public DateTime? DateDeleted { get; set; }
        public DateTime DateCreated { get; }
        public DateTime? DateUpdated { get; }
    }
}
