using System;
using System.ComponentModel.DataAnnotations;

using AdminService.Enums;

namespace AdminService.Entities.Identity
{   
    public class Token 
    {      
        [Key]
        public string Id { get; set; }
        public string UserId {  get; set; }
        public TokenType Type { get; set; } = TokenType.SessionToken;
        public DateTime TimeStamp { get; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? LastChecked { get; set;}        
    }
}
