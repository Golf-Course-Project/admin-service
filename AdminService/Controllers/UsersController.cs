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

namespace AdminService.Controllers
{
    [Route("api/users")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class UsersController : Controller
    {      
        private IUsersRepo _usersRepo;         

        public UsersController(IUsersRepo usersRepo)
        {           
            _usersRepo = usersRepo;                     
        }

        [HttpPost]
        [Route("list")]
        public IActionResult List([FromBody] ListUsersPost body)
        {
            if (body == null)
            {
                body = new ListUsersPost();
            }

            ListUsersPost newBody = new ListUsersPost()
            {
                Name = String.IsNullOrEmpty(body.Name) ? null : body.Name.Trim(),
                Email = String.IsNullOrEmpty(body.Email) ? null : body.Email.Trim(),
                Role = String.IsNullOrEmpty(body.Role) ? null : body.Role.Trim(),
                Status = body.Status == null ? -1 : body.Status,
                IsActive = body.IsActive
            };

            body = null;

            ApiResponse response = new ApiResponse();

            try 
            {
                IEnumerable<UserList> list = _usersRepo.List(newBody);

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

    }
}
