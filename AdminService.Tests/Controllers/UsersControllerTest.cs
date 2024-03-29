﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Security.Claims;

using Moq;

using AdminService.Controllers;
using AdminService.Helpers;
using AdminService.Repos.Identity;
using AdminService.Misc;
using AdminService.ViewModels.Identity;
using AdminService.Enums;
using AdminService.Entities.Identity;
using System.Security.Cryptography.X509Certificates;

namespace AdminService.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTest
    {
        private Mock<IUsersRepo> _mockUsersRepo;      
        private Mock<IPrincipal> _mockPrinciple;
        private Mock<IStandardHelper> _mockHelper;

        private UsersController _controller;
        private ControllerContext _controllerContext;
  
        private readonly string _userId = "1E51141F-852B-4157-A73A-6CBA4DF76B0D";
    
        private List<Claim> _claims;
        private ClaimsIdentity _claimsIdentity;
        private ClaimsPrincipal _claimsPrinciple;

        [TestInitialize]
        public void TestInitialize()        {
           
            _mockUsersRepo = new Mock<IUsersRepo>(); 
            _mockPrinciple = new Mock<IPrincipal>();
            _mockHelper = new Mock<IStandardHelper>();

            _claims = new List<Claim>() { new Claim(ClaimTypes.Name, _userId) };
            _claimsIdentity = new ClaimsIdentity(_claims);
            _claimsPrinciple = new ClaimsPrincipal(_claimsIdentity);
            
            // arrange
            _controller = new UsersController(_mockUsersRepo.Object, _mockHelper.Object);           
            _mockPrinciple.Setup(x => x.Identity).Returns(_claimsIdentity);
            _mockPrinciple.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);           

            _controllerContext = new ControllerContext() 
            {
                HttpContext = new DefaultHttpContext { User = _claimsPrinciple } 
            };

            // Then set it to controller before executing test
            _controller.ControllerContext = _controllerContext;
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void List_Success()
        {
            // arrange            
            IEnumerable<Users> list = new List<Users>() {
                new Users() { Id = "66d10d84-d450-4002-bb67-9fb0bb0f5a46" },
                new Users() { Id = "1a880bc6-8ad5-4532-aa02-e9b2d5f3db2a" },
                new Users() { Id = "f750ba46-4f87-45d0-9509-3cdd5bfffaa6" }
            };

            UsersSearchPost body = new UsersSearchPost();

            _mockUsersRepo.Setup(x => x.List(It.IsAny<UsersSearchPost>())).Returns(list);

            // act
            IActionResult result = _controller.List(body);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.IsTrue(apiResponse.Count == 3);

            _mockUsersRepo.Verify(x => x.List(It.IsAny<UsersSearchPost>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void List_NoResults()
        {
            // arrange            
            IEnumerable<Users> list = null;
            UsersSearchPost body = new UsersSearchPost();

            _mockUsersRepo.Setup(x => x.List(It.IsAny<UsersSearchPost>())).Returns(list);

            // act
            IActionResult result = _controller.List(body);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NoResults, apiResponse.MessageCode);
            Assert.AreEqual("No results found", apiResponse.Message);            

            _mockUsersRepo.Verify(x => x.List(It.IsAny<UsersSearchPost>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_Delete_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "delete" };
            User user = new User();
            
            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsTrue(user.IsDeleted);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_Restore_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "restore" };
            User user = new User();

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsFalse(user.IsDeleted);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_Reset_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "reset" };
            User user = new User() {  Status = UserStatus.ExceededLoginAttempts };

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(UserStatus.Okay, user.Status);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_ChangeRole_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "changerole", Role = "basic" };
            User user = new User() { Status = UserStatus.Okay };

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(UserStatus.Okay, user.Status);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_Lock_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "lock" };
            User user = new User();

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_UnLock_Success()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "unlock" };
            User user = new User();

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);            
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_Reset_InvalidCurrentStatus()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "reset" };
            User user = new User() { Status = UserStatus.NotConfirmed };

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);           
            Assert.AreEqual(ApiMessageCodes.Failed, apiResponse.MessageCode);
            Assert.AreEqual("Users current status is not eligible for reset", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_ChangeRole_InvalidRole()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "changerole", Role="bad value" };
            User user = new User() { Status = UserStatus.Okay };

            _mockHelper.Setup(x => x.GetDateTime).Returns(DateTime.Now);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);
            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.InvalidParamValue, apiResponse.MessageCode);
            Assert.AreEqual("Invalid role value", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_InvalidModelState()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch();

            _controller.ModelState.AddModelError("test", "test");

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);           
            Assert.AreEqual(ApiMessageCodes.InvalidModelState, apiResponse.MessageCode);
            Assert.AreEqual("Invalid model state", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_BodyIsNull()
        {
            // arrange
            UserUpdatePatch patch = null;

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NullValue, apiResponse.MessageCode);
            Assert.AreEqual("Body cannot be null", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_EmptyPatchValues()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch();

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NullValue, apiResponse.MessageCode);
            Assert.AreEqual("Empty patch values", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_InvalidAction()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "badvalue" };
           
            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.InvalidParamValue, apiResponse.MessageCode);
            Assert.AreEqual("Invalid action method", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_UserFetchIsNull()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "delete" };
            User user = null;

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NotFound, apiResponse.MessageCode);
            Assert.AreEqual("User not found", apiResponse.Message);

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Update_ErrorSaveChanges()
        {
            // arrange
            UserUpdatePatch patch = new UserUpdatePatch() { Id = _userId, Action = "delete" };
            User user = new User();

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(0);

            // act
            IActionResult result = _controller.Update(patch);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status500InternalServerError, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Failed, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Error updating user"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Update(It.IsAny<User>(), It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void DeleteForever_Success()
        {
            // arrange           
            User user = new User() { Id = _userId, IsDeleted = true };

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.DeleteForever(_userId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Success"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void DeleteForever_EmptyOrNullId()
        {
            // arrange
            string id = "";

            // act
            IActionResult result = _controller.DeleteForever(id);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NullValue, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Missing id value"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void DeleteForever_UserNotFound()
        {
            // arrange           
            User user = null;

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.DeleteForever(_userId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NotFound, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("User not found"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void DeleteForever_UserIsNotDeleted()
        {
            // arrange           
            User user = new User() { Id = _userId, IsDeleted = false };

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.DeleteForever(_userId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Failed, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("User cannot be permentatly deleted at this time"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Never);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Never);
        }


        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void DeleteForever_ZeroSaveResult()
        {
            // arrange           
            User user = new User() { Id = _userId, IsDeleted = true };

            _mockUsersRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(user);
            _mockUsersRepo.Setup(x => x.SaveChanges()).Returns(0);

            // act
            IActionResult result = _controller.DeleteForever(_userId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status500InternalServerError, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Failed, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Error destroying user"));

            _mockUsersRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Once);
            _mockUsersRepo.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
