using AdminService.ViewModels.Internal;
using System;

namespace AdminService.Helpers
{
    public static class EncodingHelper
    {       
        internal static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.ASCIIEncoding.ASCII.GetString(base64EncodedBytes);
        }

        internal static string Base64Encode(string clearText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(clearText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
