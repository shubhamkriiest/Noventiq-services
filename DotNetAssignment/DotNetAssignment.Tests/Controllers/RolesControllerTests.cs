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
    public class RolesControllerTests
    {
        [Fact]
        async Task GetAll_ReturnsOkWithList()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.GetAllRolesAsync())
                   .ReturnsAsync(new List<RoleDto> { new RoleDto { Id = 1, Name = "User" } });

            var controller = new RolesController(service.Object);
            var result = await controller.GetAllRoles();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<RoleDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        async Task GetById_Found_ReturnsOk()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.GetRoleByIdAsync(1))
                   .ReturnsAsync(new RoleDto { Id = 1, Name = "User" });

            var controller = new RolesController(service.Object);
            var result = await controller.GetRole(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var role = Assert.IsType<RoleDto>(ok.Value);
            Assert.Equal(1, role.Id);
        }

        [Fact]
        async Task GetById_NotFound_ReturnsNotFound()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.GetRoleByIdAsync(2))
                   .ReturnsAsync((RoleDto?)null);

            var controller = new RolesController(service.Object);
            var result = await controller.GetRole(2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var messageProp = notFound.Value!.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var msg = messageProp!.GetValue(notFound.Value) as string;
            Assert.Equal("Role not found", msg);
        }

        [Fact]
        async Task Create_Valid_ReturnsCreated()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.CreateRoleAsync(It.IsAny<CreateRoleDto>()))
                   .ReturnsAsync((true, "RoleCreated", new RoleDto { Id = 2, Name = "Admin" }));

            var controller = new RolesController(service.Object);
            var result = await controller.CreateRole(new CreateRoleDto { Name = "Admin" });

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var body = Assert.IsType<RoleDto>(created.Value);
            Assert.Equal(2, body.Id);
        }

        [Fact]
        async Task Update_NotFound_ReturnsNotFound()
        {
            var service = new Mock<IRoleService>();
             service.Setup(s => s.UpdateRoleAsync(99, It.IsAny<UpdateRoleDto>()))
                 .ReturnsAsync((false, "Role not found"));

            var controller = new RolesController(service.Object);
            var result = await controller.UpdateRole(99, new UpdateRoleDto { Name = "Ghost" });

             var notFound = Assert.IsType<NotFoundObjectResult>(result);
             var messageProp = notFound.Value!.GetType().GetProperty("message");
             Assert.NotNull(messageProp);
             var msg = messageProp!.GetValue(notFound.Value) as string;
             Assert.Equal("Role not found", msg);
        }

        [Fact]
        async Task Delete_HasUsers_ReturnsBadRequest()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.DeleteRoleAsync(1))
                   .ReturnsAsync((false, "RoleHasUsers"));

            var controller = new RolesController(service.Object);
            var result = await controller.DeleteRole(1);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var messageProp = bad.Value!.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var msg = messageProp!.GetValue(bad.Value) as string;
            Assert.Equal("RoleHasUsers", msg);
        }

        [Fact]
        async Task Delete_Valid_ReturnsOk()
        {
            var service = new Mock<IRoleService>();
            service.Setup(s => s.DeleteRoleAsync(2))
                   .ReturnsAsync((true, "RoleDeleted"));

            var controller = new RolesController(service.Object);
            var result = await controller.DeleteRole(2);

            var ok = Assert.IsType<OkObjectResult>(result);
            var messageProp = ok.Value!.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var msg = messageProp!.GetValue(ok.Value) as string;
            Assert.Equal("RoleDeleted", msg);
        }
    }
}
