using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetAssignment.Controllers;
using DotNetAssignment.DTOs;
using DotNetAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DotNetAssignment.Tests.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        async Task GetAll_ReturnsOkWithList()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.GetAllUsersAsync())
                   .ReturnsAsync(new List<UserDto> { new UserDto { Id = 1, Username = "john" } });

            var controller = new UsersController(service.Object);
            var result = await controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        async Task Create_Valid_ReturnsCreated()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
                   .ReturnsAsync((true, "UserCreated", new UserDto { Id = 2, Username = "mary" }));

            var controller = new UsersController(service.Object);
            var result = await controller.CreateUser(new CreateUserDto { Username = "mary", Email = "m@example.com", Password = "pass", RoleId = 2 });

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var body = Assert.IsType<UserDto>(created.Value);
            Assert.Equal(2, body.Id);
        }

        [Fact]
        async Task Create_Duplicate_ReturnsBadRequest()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
                   .ReturnsAsync((false, "UsernameExists", (UserDto?)null));

            var controller = new UsersController(service.Object);
            var result = await controller.CreateUser(new CreateUserDto { Username = "john" });

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            var messageProp = bad.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(bad.Value) as string;
            Assert.Equal("UsernameExists", msg);
        }

        [Fact]
        async Task Update_Valid_ReturnsOk()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.UpdateUserAsync(1, It.IsAny<UpdateUserDto>()))
                   .ReturnsAsync((true, "UserUpdated"));

            var controller = new UsersController(service.Object);
            var result = await controller.UpdateUser(1, new UpdateUserDto { Username = "johnny" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var messageProp = ok.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(ok.Value) as string;
            Assert.Equal("UserUpdated", msg);
        }

        [Fact]
        async Task Update_NotFound_ReturnsNotFound()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.UpdateUserAsync(99, It.IsAny<UpdateUserDto>()))
                   .ReturnsAsync((false, "User not found"));

            var controller = new UsersController(service.Object);
            var result = await controller.UpdateUser(99, new UpdateUserDto { Username = "ghost" });

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var messageProp = notFound.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(notFound.Value) as string;
            Assert.Equal("User not found", msg);
        }

        [Fact]
        async Task Delete_Valid_ReturnsOk()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.DeleteUserAsync(1))
                   .ReturnsAsync((true, "UserDeleted"));

            var controller = new UsersController(service.Object);
            var result = await controller.DeleteUser(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var messageProp = ok.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(ok.Value) as string;
            Assert.Equal("UserDeleted", msg);
        }

        [Fact]
        async Task Delete_NotFound_ReturnsNotFound()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.DeleteUserAsync(99))
                   .ReturnsAsync((false, "User not found"));

            var controller = new UsersController(service.Object);
            var result = await controller.DeleteUser(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var messageProp = notFound.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(notFound.Value) as string;
            Assert.Equal("User not found", msg);
        }

        [Fact]
        async Task GetById_Found_ReturnsOk()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.GetUserByIdAsync(1))
                   .ReturnsAsync(new UserDto { Id = 1, Username = "john" });

            var controller = new UsersController(service.Object);
            var result = await controller.GetUser(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal(1, user.Id);
        }

        [Fact]
        async Task GetById_NotFound_ReturnsNotFound()
        {
            var service = new Mock<IUserService>();
            service.Setup(s => s.GetUserByIdAsync(2))
                   .ReturnsAsync((UserDto?)null);

            var controller = new UsersController(service.Object);
            var result = await controller.GetUser(2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var messageProp = notFound.Value!.GetType().GetProperty("message");
            var msg = messageProp!.GetValue(notFound.Value) as string;
            Assert.Equal("User not found", msg);
        }
    }
}
