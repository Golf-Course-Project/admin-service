﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult List([FromBody] UsersSearchPost body)
        {
            if (body == null)
            {
                body = new UsersSearchPost();
            }

            UsersSearchPost newBody = new UsersSearchPost()
            {
                Name = String.IsNullOrEmpty(body.Name) ? null : body.Name.Trim(),
                Email = String.IsNullOrEmpty(body.Email) ? null : body.Email.Trim(),
                Role = String.IsNullOrEmpty(body.Role) ? null : body.Role.Trim(),
                Status = body.Status == null ? -1 : body.Status,
                IsDeleted = body.IsDeleted ? true : false
            };

            body = null;

            ApiResponse response = new ApiResponse();

            try 
            {
                IEnumerable<Users> list = _usersRepo.List(newBody);

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

        [HttpPost]
        [Route("update")]
        public IActionResult Update ([FromBody] UserUpdatePatch body)
        {
            ApiResponse response = new ApiResponse();
            string[] actions = { "delete", "unlock", "lock", "reset", "changerole", "restore" };
            string[] roles = { "basic", "site admin" };

            if (!ModelState.IsValid)
            {
                response.MessageCode = ApiMessageCodes.InvalidModelState;
                response.Message = "Invalid model state";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            if (body == null)
            {
                response.MessageCode = ApiMessageCodes.NullValue;
                response.Message = "Body cannot be null";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            if (String.IsNullOrEmpty(body.Id) || String.IsNullOrEmpty(body.Action))
            {
                response.MessageCode = ApiMessageCodes.NullValue;
                response.Message = "Empty patch values";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            if (! actions.Contains(body.Action))
            {
                response.MessageCode = ApiMessageCodes.InvalidParamValue;
                response.Message = "Invalid action method";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            if (body.Action.ToLower() == "changerole")
            {
                if (!roles.Contains(body.Role))
                {
                    response.MessageCode = ApiMessageCodes.InvalidParamValue;
                    response.Message = "Invalid role value";

                    return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
                }
            }

            User user = _usersRepo.Fetch(body.Id.ToLower());

            if (user == null)
            {
                response.Message = "User not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            // only a status of exceeded login attempts can be reset
            if (user.Status != UserStatus.ExceededLoginAttempts && body.Action == "reset")
            {
                response.Message = "Users current status is not eligible for reset";
                response.MessageCode = ApiMessageCodes.Failed;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            string fields = "";
            DateTime date = _helper.GetDateTime;
            user.DateUpdated = date;

            switch (body.Action.ToLower())
            {
                case "delete":
                    fields = "DateDeleted,IsDeleted,DateUpdated";
                    user.DateDeleted = date;                   
                    user.IsDeleted = true; 
                    break;
                case "unlock":
                    fields = "IsLocked,DateLocked";
                    user.IsLocked = false;
                    user.DateLocked = null;
                    break;
                case "lock":
                    fields = "DateLocked,IsLocked";
                    user.IsLocked = true;
                    user.DateLocked = date;
                    break;
                case "reset":
                    fields = "Status,LoginAttempts,DateUpdated";
                    user.Status = UserStatus.Okay;
                    user.LoginAttempts = 0;
                    break;
                case "changerole":
                    fields = "DateUpdated,Role";
                    user.Role = body.Role.ToLower();
                    break;
                case "restore":
                    fields = "IsDeleted,DateDeleted,DateUpdated";
                    user.IsDeleted = false;
                    user.DateDeleted = null;
                    break;
            }

            try
            {
                _usersRepo.Update(user, fields);
                int result = _usersRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = $"Error updating user (${body.Action.ToLower()})";
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

        [HttpDelete]
        [Route("deleteforever/{id}")]
        public IActionResult DeleteForever(string id)
        {
            ApiResponse response = new ApiResponse();    

            if (String.IsNullOrEmpty(id))
            {
                response.MessageCode = ApiMessageCodes.NullValue;
                response.Message = "Missing id value";

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            } 

            User user = _usersRepo.Fetch(id.ToLower());

            if (user == null)
            {
                response.Message = "User not found";
                response.MessageCode = ApiMessageCodes.NotFound;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }

            // only a status of exceeded login attempts can be reset
            if (! user.IsDeleted)
            {
                response.Message = "User cannot be permentatly deleted at this time";
                response.MessageCode = ApiMessageCodes.Failed;

                return new StandardResponseObjectResult(response, StatusCodes.Status200OK);
            }          

            try
            {
                _usersRepo.Destroy(id);
                int result = _usersRepo.SaveChanges();

                if (result == 0)
                {
                    response.Message = $"Error destroying user";
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
