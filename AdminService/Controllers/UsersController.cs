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

namespace AdminService.Controllers
{
    [Route("api/users")]
    [ApiController]
    [ServiceFilter(typeof(TokenAuthorizationActionFilter))]
    public partial class UsersController : Controller
    {      
        private IUsersRepo _usersRepo;
        private IStandardHelper _helper;

        public UsersController(IUsersRepo usersRepo, IStandardHelper helper)
        {           
            _usersRepo = usersRepo;
            _helper = helper;
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
                Status = body.Status == null ? -1 : body.Status               
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

        [HttpPatch]
        [Route("delete/{id}")]
        public IActionResult Delete (string id)
        {
            ApiResponse response = new ApiResponse();

            if (string.IsNullOrEmpty(id))
            {
                response.MessageCode = Enums.ApiMessageCodes.EmptyValue;
                response.Message = "Id cannot be empty";
                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }         
            
            try
            {
                _usersRepo.Delete(id, _helper.GetDateTime);
                int result = _usersRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = "Error updating user";
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

        [HttpPatch]
        [Route("activate/{id}")]
        public IActionResult Activate(string id)
        {
            ApiResponse response = new ApiResponse();

            if (string.IsNullOrEmpty(id))
            {
                response.MessageCode = Enums.ApiMessageCodes.EmptyValue;
                response.Message = "Id cannot be empty";
                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            User user = _usersRepo.Fetch(id);

            if (user == null)
            {
                response.Message = "Id not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            try
            {
                user.Status = UserStatus.Okay;
                user.DateReset = _helper.GetDateTime;

                _usersRepo.Update(user, "Status,DateReset");
                int result = _usersRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = "Error updating user";
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

        [HttpPatch]
        [Route("deactivate/{id}")]
        public IActionResult Block(string id)
        {
            ApiResponse response = new ApiResponse();

            if (string.IsNullOrEmpty(id))
            {
                response.MessageCode = Enums.ApiMessageCodes.EmptyValue;
                response.Message = "Id cannot be empty";
                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            User user = _usersRepo.Fetch(id);

            if (user == null)
            {
                response.Message = "Id not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            try
            {
                user.Status = UserStatus.InActive;
                user.DateReset = _helper.GetDateTime;

                _usersRepo.Update(user, "Status,DateReset");
                int result = _usersRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = "Error updating user";
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
