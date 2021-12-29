using AdminService.Misc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminService.Helpers
{  
    public class StandardHelper : IStandardHelper
    {
        private IOptions<AppSettings> _appSettings;
        private readonly int _daysToExpire = 7;

        public int DaysToExpire
        {
            get { return _daysToExpire; }
        }

        public StandardHelper(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public string GetNewId
        {
            get
            {
                return System.Guid.NewGuid().ToString().ToLower();
            }
        }

        public DateTime GetDateTime
        {
            get { return DateTime.Now; }
        }

        public IOptions<AppSettings> AppSettings 
        { 
            get 
            { 
                return _appSettings; 
            } 
        }

        public string GetTokenFromIdentity(System.Security.Claims.ClaimsPrincipal user)
        {           
            IEnumerable<System.Security.Claims.Claim> claims = user.Identities.First().Claims.ToList();
            string token = claims?.FirstOrDefault(x => x.Type.Equals("token", StringComparison.OrdinalIgnoreCase))?.Value;

            return token;            
        }

        public string Base64Decode(string base64EncodedData)
        {
            return EncodingHelper.Base64Decode(base64EncodedData);         
        }

        public string Base64Encode(string clearText)
        {
            return EncodingHelper.Base64Encode(clearText);           
        }
    }

    public interface IStandardHelper
    {
        int DaysToExpire { get; }
        string GetNewId { get; }
        DateTime GetDateTime { get; }
        IOptions<AppSettings> AppSettings { get;  }
        string GetTokenFromIdentity(System.Security.Claims.ClaimsPrincipal user);
        string Base64Decode(string base64EncodedData);
        string Base64Encode(string clearText);
    }
    
}
