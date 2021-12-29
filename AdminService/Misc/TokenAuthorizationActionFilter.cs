using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

using AdminService.Repos.Identity;
using AdminService.Entities.Identity;

namespace AdminService.Misc
{
    public class TokenAuthorizationActionFilter : IActionFilter
    {
        private IAuthRepo _authRepo;

        public TokenAuthorizationActionFilter(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ApiResponse response = new ApiResponse() { Success = false };

            if (MethodIsAllowAnonymousAuthorization(context))
            {
                return;
            };

            List<Claim> claims = context.HttpContext.User.Identities.First().Claims.ToList();
            string token = claims?.FirstOrDefault(x => x.Type.Equals("token", StringComparison.OrdinalIgnoreCase))?.Value;

            if (String.IsNullOrEmpty(token))
            {
                response.Message = "Missing token";
                context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
                return;
            }

            // get token record by token
            Token userTokenEntity = _authRepo.GetToken(token);            
                        
            if (userTokenEntity == null)
            {
                response.Message = "Invalid token";
                context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
                return;
            }

            if (userTokenEntity.IsRevoked)
            {
                response.Message = "Token has been revoked";
                context.Result = new StandardResponseObjectResult(response, StatusCodes.Status401Unauthorized);
                return;
            }
            
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
        
        /// <summary>
        /// check to see of the AllowAnonymous attribute has been set to the method
        /// </summary>
        /// <param name="context"></param>
        /// <returns>boolean</returns>
        private static bool MethodIsAllowAnonymousAuthorization(ActionExecutingContext context)
        {  
            if (context == null) { return false; }

            ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            bool results = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any();
                
            return results;
        }
    }
}
