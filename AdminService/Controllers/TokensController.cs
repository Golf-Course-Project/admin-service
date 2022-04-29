using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

using AdminService.Misc;
using AdminService.Repos.Identity;
using AdminService.ViewModels.Identity;
using System.Collections.Generic;
using System.Linq;
using AdminService.Enums;
using AdminService.Entities.Identity;
using AdminService.Helpers;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AdminService.Controllers
{
    [Route("api/tokens")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class TokensController : Controller
    {      
        private ITokensRepo _tokensRepo;
        private IStandardHelper _helper;

        public TokensController(ITokensRepo tokensRepo, IStandardHelper helper)
        {           
            _tokensRepo = tokensRepo;
            _helper = helper;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult List()
        {
            ApiResponse response = new ApiResponse();

            try 
            {
                IEnumerable<Tokens> list = _tokensRepo.List();

                if (list != null)
                {
                    response.Success = true; 
                    response.MessageCode = ApiMessageCodes.Success;
                    response.Message = "Success";
                    response.Count = list.Count();
                    response.Value = list;                    
                }
                else
                {
                    response.MessageCode = Enums.ApiMessageCodes.NoResults;
                    response.Message = "No results found";
                }
            }
            catch (Exception ex)
            {
                response.MessageCode = Enums.ApiMessageCodes.ExceptionThrown;
                response.Message = "Error getting list:" + ex.Message;
            }           

            return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(string id)
        {
            ApiResponse response = new ApiResponse();    

            if (String.IsNullOrEmpty(id))
            {
                response.MessageCode = ApiMessageCodes.NullValue;
                response.Message = "Missing id value";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            } 

            Token token = _tokensRepo.Fetch(id.ToLower());

            if (token == null)
            {
                response.Message = "Token not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }                  

            try
            {
                _tokensRepo.Destroy(id);
                int result = _tokensRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = "Error deleting token";
                    response.MessageCode = ApiMessageCodes.Failed;

                    return new StandardResponseObjectResult(response, StatusCodes.Status500InternalServerError);
                }

                response.Success = true;
                response.MessageCode = ApiMessageCodes.Success;
                response.Message = "Success";

                return new StandardResponseObjectResult(response, StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                response.Message = "Exception thrown: " + ex.Message;
                response.MessageCode = ApiMessageCodes.ExceptionThrown;

                return new StandardResponseObjectResult(response, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
