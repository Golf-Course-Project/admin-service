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

namespace AdminService.Tests.Controllers
{
    [TestClass]
    public class CodesControllerTest
    {
        private Mock<ICodesRepo> _mockCodesRepo;  
        private Mock<IPrincipal> _mockPrinciple;    

        private CodesController _controller;
        private ControllerContext _controllerContext;
  
        private readonly string _userId = "1E51141F-852B-4157-A73A-6CBA4DF76B0D";
    
        private List<Claim> _claims;
        private ClaimsIdentity _claimsIdentity;
        private ClaimsPrincipal _claimsPrinciple;

        [TestInitialize]
        public void TestInitialize()        {
           
            _mockCodesRepo = new Mock<ICodesRepo>(); 
            _mockPrinciple = new Mock<IPrincipal>();
          
            _claims = new List<Claim>() { new Claim(ClaimTypes.Name, _userId) };
            _claimsIdentity = new ClaimsIdentity(_claims);
            _claimsPrinciple = new ClaimsPrincipal(_claimsIdentity);
            
            // arrange
            _controller = new CodesController(_mockCodesRepo.Object);           
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
            IEnumerable<CodesListWithUser> list = new List<CodesListWithUser>() {
                new CodesListWithUser() { Id = "66d10d84-d450-4002-bb67-9fb0bb0f5a46" },
                new CodesListWithUser() { Id = "1a880bc6-8ad5-4532-aa02-e9b2d5f3db2a" },
                new CodesListWithUser() { Id = "f750ba46-4f87-45d0-9509-3cdd5bfffaa6" }
            };

            _mockCodesRepo.Setup(x => x.List()).Returns(list);

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

            _mockCodesRepo.Verify(x => x.List(), Times.Once);
        }

        [TestMethod]
        [TestCategory("Controllers")]
        [Priority(0)]
        public void List_NoResults()
        {
            // arrange            
            IEnumerable<CodesListWithUser> list = null;

            _mockCodesRepo.Setup(x => x.List()).Returns(list);

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

            _mockCodesRepo.Verify(x => x.List(), Times.Once);
        }
    }
}
