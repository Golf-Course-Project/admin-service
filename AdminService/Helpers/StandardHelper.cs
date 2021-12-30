using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

using AdminService.Misc;
using System.Linq;

namespace AdminService.Helpers
{  
    public class StandardHelper : IStandardHelper
    {
        private IOptions<AppSettings> _appSettings;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly int _daysToExpire = 7;

        public int DaysToExpire
        {
            get { return _daysToExpire; }
        }

        public StandardHelper(IOptions<AppSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _httpContextAccessor = httpContextAccessor;
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

        public AppSettings AppSettings 
        { 
            get 
            { 
                return _appSettings.Value; 
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
        AppSettings AppSettings { get;  }
        string GetTokenFromIdentity(System.Security.Claims.ClaimsPrincipal user);
        string Base64Decode(string base64EncodedData);
        string Base64Encode(string clearText);
    }
    
}
