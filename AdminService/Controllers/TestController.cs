using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

using AdminService.Helpers;
using AdminService.Misc;

using AdminService.Repos;
using AdminService.Entities;
using AdminService.Enums;

namespace IdentityService.Controllers
{
    [Route("api/test")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public class TestController : Controller
    {
        private IStandardHelper _helper;
        
        public TestController(IStandardHelper helper)
        {
            _helper = helper;           
        }

        [HttpGet]
        [Route("simple")]
        [AllowAnonymous]
        public IActionResult SimpleFetch()
        {
            ApiResponse response = new ApiResponse();

            response.MessageCode = ApiMessageCodes.Success;
            response.Success = true;
            response.Value = "Simple";
            response.Message = "Success";
            response.Count = 1;

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
        }

        [HttpGet]
        [Route("simpleauth")]     
        public IActionResult SimpleAuthFetch()
        {
            ApiResponse response = new ApiResponse();

            response.MessageCode = ApiMessageCodes.Success;
            response.Success = true;
            response.Value = "Simple Auth";
            response.Message = "Success";
            response.Count = 1;

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
        }
    }
}