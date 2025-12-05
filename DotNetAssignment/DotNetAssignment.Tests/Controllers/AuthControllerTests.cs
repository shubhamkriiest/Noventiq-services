using System.Threading.Tasks;
using DotNetAssignment.Controllers;
using DotNetAssignment.DTOs;
using DotNetAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace DotNetAssignment.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var loginDto = new LoginDto { Username = "john", Password = "pass" };
            var expected = new LoginResponseDto
            {
                Token = "fake-jwt",
                RefreshToken = "fake-refresh",
                Username = "john",
                Email = "john@example.com",
                Role = "User"
            };

            var authService = new Mock<IAuthService>();
            authService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                       .ReturnsAsync((true, string.Empty, expected));

               var env = new Mock<IWebHostEnvironment>();
               env.SetupGet(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
               var localizer = new SimpleLocalizer(env.Object);
               var controller = new AuthController(authService.Object, localizer);

            var result = await controller.Login(loginDto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var body = Assert.IsType<LoginResponseDto>(ok.Value);
            Assert.Equal("fake-jwt", body.Token);
            Assert.Equal("fake-refresh", body.RefreshToken);
        }

        [Fact]
        async Task Login_InvalidCredentials_ReturnsUnauthorizedWithMessage()
        {
            var loginDto = new LoginDto { Username = "john", Password = "wrong" };
            var authService = new Mock<IAuthService>();
            authService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                       .ReturnsAsync((false, "InvalidCredentials", (LoginResponseDto?)null));

               var env = new Mock<IWebHostEnvironment>();
               env.SetupGet(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
               var localizer = new SimpleLocalizer(env.Object);
               var controller = new AuthController(authService.Object, localizer);

            var result = await controller.Login(loginDto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var messageProp = unauthorized.Value!.GetType().GetProperty("message");
            var message = messageProp!.GetValue(unauthorized.Value) as string;
            Assert.Equal("Invalid username or password", message);
        }

        [Fact]
        async Task Refresh_ValidToken_ReturnsOkWithNewTokens()
        {
            var request = new RefreshRequestDto { RefreshToken = "old-refresh" };
            var expected = new RefreshResponseDto { Token = "new-jwt", RefreshToken = "new-refresh" };

            var authService = new Mock<IAuthService>();
            authService.Setup(s => s.RefreshAsync(It.IsAny<string>()))
                       .ReturnsAsync((true, string.Empty, expected));

               var env = new Mock<IWebHostEnvironment>();
               env.SetupGet(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
               var localizer = new SimpleLocalizer(env.Object);
               var controller = new AuthController(authService.Object, localizer);

            var result = await controller.Refresh(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<RefreshResponseDto>(ok.Value);
            Assert.Equal("new-jwt", body.Token);
            Assert.Equal("new-refresh", body.RefreshToken);
        }

        [Fact]
        async Task Refresh_InvalidToken_ReturnsBadRequest()
        {
            var request = new RefreshRequestDto { RefreshToken = "bad-refresh" };
            var authService = new Mock<IAuthService>();
            authService.Setup(s => s.RefreshAsync(It.IsAny<string>()))
                       .ReturnsAsync((false, "InvalidOrExpiredRefreshToken", (RefreshResponseDto?)null));

               var env = new Mock<IWebHostEnvironment>();
               env.SetupGet(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
               var localizer = new SimpleLocalizer(env.Object);
               var controller = new AuthController(authService.Object, localizer);

            var result = await controller.Refresh(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var messageProp = badRequest.Value!.GetType().GetProperty("message");
            var message = messageProp!.GetValue(badRequest.Value) as string;
            Assert.Equal("InvalidOrExpiredRefreshToken", message);
        }
    }
}
