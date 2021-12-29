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

namespace AdminService.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTest
    {
        private Mock<IUsersRepo> _mockUsersRepo;        
        private Mock<IStandardHelper> _mockHelper;
        private Mock<IPrincipal> _mockPrinciple;
        private Mock<IOptions<AppSettings>> _mockAppSettings;

        private UsersController _controller;
        private ControllerContext _controllerContext;
  
        private readonly string _userId = "1E51141F-852B-4157-A73A-6CBA4DF76B0D";
        private readonly string _email = "joe@gmail.com";
        private readonly string _code = "93038f57-718f-4b62-be68-ae30ac1f5172";
        private readonly string _encoded = "am9lQGdtYWlsLmNvbTo5MzAzOGY1Ny03MThmLTRiNjItYmU2OC1hZTMwYWMxZjUxNzI=";
        private DateTime _now = DateTime.Now;

        private List<Claim> _claims;
        private ClaimsIdentity _claimsIdentity;
        private ClaimsPrincipal _claimsPrinciple;

        [TestInitialize]
        public void TestInitialize()        {
           
            _mockUsersRepo = new Mock<IUsersRepo>();           
            _mockHelper = new Mock<IStandardHelper>();           
            _mockPrinciple = new Mock<IPrincipal>();
            _mockAppSettings = new Mock<IOptions<AppSettings>>();

            _claims = new List<Claim>() { new Claim(ClaimTypes.Name, _userId) };
            _claimsIdentity = new ClaimsIdentity(_claims);
            _claimsPrinciple = new ClaimsPrincipal(_claimsIdentity);
            
            // arrange
            _controller = new UsersController(_mockUsersRepo.Object);

            _mockAppSettings.Setup(x => x.Value).Returns(new AppSettings() { });
            _mockPrinciple.Setup(x => x.Identity).Returns(_claimsIdentity);
            _mockPrinciple.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);           

            _controllerContext = new ControllerContext() 
            {
                HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrinciple
                }
            };

            // Then set it to controller before executing test
            _controller.ControllerContext = _controllerContext;
        }      
       
    }
}
