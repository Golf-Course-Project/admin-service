using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;

using AdminService.Repos.Identity;
using AdminService.Enums;
using AdminService.ViewModels.Internal;

namespace AdminService.Misc
{
    public class TokenAuthorizationActionFilter : IActionFilter
    {
        private IIdentityRepo _identityRepo;

        public TokenAuthorizationActionFilter(IIdentityRepo identityRepo)
        {
            _identityRepo = identityRepo;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ApiResponse response = new ApiResponse()
            {
                Success = false,
                MessageCode = ApiMessageCodes.AuthFailed
            };

            // if the method allows anonoymous access then return success and skip token validation logic
            if (MethodIsAllowAnonymousAuthorization(context)) return;

            // get x-authorization header value
            string header_value = context.HttpContext.Request.Headers?.FirstOrDefault(x => x.Key.Equals("X-Authorization", StringComparison.OrdinalIgnoreCase)).Value;

            // if the header valu is empty, somethign went wrong, return a 401
            if (string.IsNullOrEmpty(header_value))
            {
                response.Message = "Missing bearer token from X-Authorization header";
                context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);

                return;
            }

            // if we have a bearer, then treat it as a jwt token
            // this is the most common entry point for web users
            if (header_value.ToLower().Contains("bearer"))
            {
                string[] splitToken = header_value.Split(' ');
                string jwt = splitToken[1];

                //make sure jwt is not empty
                if (String.IsNullOrEmpty(jwt))
                {
                    response.Message = "Missing bearer token from X-Authorization header";
                    context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);

                    return;
                }

                // validate jwt against the identity service
                ValidateTokenResponse validateResponse = _identityRepo.ValidateJwt(jwt);

                // if it is not successully validated, return 401
                if (validateResponse == null || !validateResponse.Success)
                {
                    response.Message = "Error validating jwt: " + response.Message + "'";
                    context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);

                    return;
                }

                return;
            }

            // if not a bearer then assume we have a personal access token from a device
            else
            {
                string token = header_value;

                if (String.IsNullOrEmpty(token))
                {
                    response.Message = "Missing token from X-Authorization header";
                    context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
                    return;
                }
            }

            //var handler = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            //var userProfileId = handler.Claims.First(x => x.Type == "unique_name").Value;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }

        private static bool MethodIsAllowAnonymousAuthorization(ActionExecutingContext context)
        {
            if (context == null) { return false; }

            ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            bool results = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any();

            return results;
        }

    }
}
