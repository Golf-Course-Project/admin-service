using Newtonsoft.Json;
using System;

namespace AdminService.ViewModels.Internal
{
    public class ValidateTokenResponse
    {
        public ValidateTokenResponse()
        {
            Success = false;
            Message = String.Empty;
        }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
