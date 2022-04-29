using System;
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

namespace AdminService.Tests.Controllers
{
    [TestClass]
    public class TokensControllerTest
    {
        private Mock<ITokensRepo> _mockTokensRepo;      
        private Mock<IPrincipal> _mockPrinciple;
        private Mock<IStandardHelper> _mockHelper;

        private TokensController _controller;
        private ControllerContext _controllerContext;
  
        private readonly string _tokenId = "1E51141F-852B-4157-A73A-6CBA4DF76B0D";
    
        private List<Claim> _claims;
        private ClaimsIdentity _claimsIdentity;
        private ClaimsPrincipal _claimsPrinciple;

        [TestInitialize]
        public void TestInitialize()        {
           
            _mockTokensRepo = new Mock<ITokensRepo>(); 
            _mockPrinciple = new Mock<IPrincipal>();
            _mockHelper = new Mock<IStandardHelper>();

            _claims = new List<Claim>() { new Claim(ClaimTypes.Name, _tokenId) };
            _claimsIdentity = new ClaimsIdentity(_claims);
            _claimsPrinciple = new ClaimsPrincipal(_claimsIdentity);
            
            // arrange
            _controller = new TokensController(_mockTokensRepo.Object, _mockHelper.Object);           
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
            IEnumerable<Tokens> list = new List<Tokens>() {
                new Tokens() { Id = "66d10d84-d450-4002-bb67-9fb0bb0f5a46" },
                new Tokens() { Id = "1a880bc6-8ad5-4532-aa02-e9b2d5f3db2a" },
                new Tokens() { Id = "f750ba46-4f87-45d0-9509-3cdd5bfffaa6" }
            };            

            _mockTokensRepo.Setup(x => x.List()).Returns(list);

            // act
            IActionResult result = _controller.List();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.AreEqual("Success", apiResponse.Message);
            Assert.IsTrue(apiResponse.Count == 3);

            _mockTokensRepo.Verify(x => x.List(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void List_NoResults()
        {
            // arrange            
            IEnumerable<Tokens> list = null;
          
            _mockTokensRepo.Setup(x => x.List()).Returns(list);

            // act
            IActionResult result = _controller.List();
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NoResults, apiResponse.MessageCode);
            Assert.AreEqual("No results found", apiResponse.Message);            

            _mockTokensRepo.Verify(x => x.List(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Delete_Success()
        {
            // arrange           
            Token token = new Token() { Id = _tokenId };

            _mockTokensRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(token);
            _mockTokensRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.Delete(_tokenId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status202Accepted, standardResponse.StatusCode);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Success, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Success"));

            _mockTokensRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockTokensRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Once);
            _mockTokensRepo.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Delete_EmptyOrNullId()
        {
            // arrange
            string id = "";

            // act
            IActionResult result = _controller.Delete(id);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NullValue, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Missing id value"));

            _mockTokensRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Never);
            _mockTokensRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Never);
            _mockTokensRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Delete_TokenNotFound()
        {
            // arrange           
            Token token = null;

            _mockTokensRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(token);
            _mockTokensRepo.Setup(x => x.SaveChanges()).Returns(1);

            // act
            IActionResult result = _controller.Delete(_tokenId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status200OK, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.NotFound, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Token not found"));

            _mockTokensRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockTokensRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Never);
            _mockTokensRepo.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void Delete_ZeroSaveResult()
        {
            // arrange           
            Token token = new Token() { Id = _tokenId };

            _mockTokensRepo.Setup(x => x.Fetch(It.IsAny<String>())).Returns(token);
            _mockTokensRepo.Setup(x => x.SaveChanges()).Returns(0);

            // act
            IActionResult result = _controller.Delete(_tokenId);
            var standardResponse = (StandardResponseObjectResult)result;
            var apiResponse = (ApiResponse)standardResponse.Value;

            // assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "'result' type must be of IActionResult");
            Assert.AreEqual(StatusCodes.Status500InternalServerError, standardResponse.StatusCode);
            Assert.IsFalse(apiResponse.Success);
            Assert.AreEqual(ApiMessageCodes.Failed, apiResponse.MessageCode);
            Assert.IsTrue(apiResponse.Message.Contains("Error deleting token"));

            _mockTokensRepo.Verify(x => x.Fetch(It.IsAny<String>()), Times.Once);
            _mockTokensRepo.Verify(x => x.Destroy(It.IsAny<String>()), Times.Once);
            _mockTokensRepo.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}
