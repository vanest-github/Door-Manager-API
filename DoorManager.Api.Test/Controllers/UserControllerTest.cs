using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Api.ApiResponses;
using DoorManager.Api.Controllers;
using DoorManager.Entity;
using DoorManager.Entity.Configurations;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Storage.Interface.Commands;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DoorManager.Api.Test.Controllers;

public class UserControllerTest
{
    private IBus bus;
    private UserController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IUserCommandRepository> userCommandRepoMock;
    private Mock<IUserQueryRepository> userQueryRepoMock;
    private Mock<IRoleQueryRepository> roleQueryRepoMock;
    private Mock<IOptions<GlobalConfiguration>> configurationMock;
    private Mock<IActivityCommandRepository> activityCommandRepoMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        userCommandRepoMock = new Mock<IUserCommandRepository>();
        userQueryRepoMock = new Mock<IUserQueryRepository>();
        roleQueryRepoMock = new Mock<IRoleQueryRepository>();
        configurationMock = new Mock<IOptions<GlobalConfiguration>>();
        configurationMock.Setup(c => c.Value).Returns(new GlobalConfiguration { UserRoleValidityMonths = 12 });
        activityCommandRepoMock = new Mock<IActivityCommandRepository>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => userQueryRepoMock.Object);
        services.AddScoped(sp => roleQueryRepoMock.Object);
        services.AddScoped(sp => userCommandRepoMock.Object);
        services.AddScoped(sp => configurationMock.Object);
        services.AddScoped(sp => activityCommandRepoMock.Object);

        services.AddMediatR(typeof(Service.User.Create.Handler));
        services.AddMediatR(typeof(Service.User.Get.Handler));
        services.AddMediatR(typeof(Service.Role.Get.ByName.Handler));
        services.AddMediatR(typeof(Service.Activity.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new UserController(bus);
    }

    [Test]
    public async Task Pass_UserCreation_With_ValidCreatingUser_ValidRoleName()
    {
        int officeId = randomizer.Next();
        var dto = new UserDto
        {
            FirstName = randomizer.GetString(),
            LastName = randomizer.GetString(),
            RoleName = randomizer.GetString(),
            IsActive = true,
            LastModifiedBy = randomizer.Next()
        };
        var roleByName = new Role { RoleId = randomizer.Next(), RoleName = dto.RoleName, IsActive = true };
        var user = new User { UserId = randomizer.Next(), FirstName = dto.FirstName, LastName = dto.LastName, IsActive = dto.IsActive };

        userQueryRepoMock.Setup(r => r.GetAsync(dto.LastModifiedBy, It.IsAny<CancellationToken>())).ReturnsAsync(new User()).Verifiable();
        roleQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { roleByName }).Verifiable();
        userCommandRepoMock.Setup(r => r.CreateNewUserAsync(It.IsAny<User>(), It.IsAny<UserOfficeRole>(), It.IsAny<CancellationToken>())).ReturnsAsync(user).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(officeId, dto.LastModifiedBy, user), It.IsAny<CancellationToken>())).ReturnsAsync(new ActivityLog()).Verifiable();

        var result = await controller.CreateUser(officeId, dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.AreEqual(user, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [Test]
    public async Task Fail_UserCreation_With_InvalidValidCreatingUser()
    {
        int officeId = randomizer.Next();
        var dto = new UserDto
        {
            FirstName = randomizer.GetString(),
            LastName = randomizer.GetString(),
            RoleName = randomizer.GetString(),
            IsActive = true,
            LastModifiedBy = randomizer.Next()
        };

        userQueryRepoMock.Setup(r => r.GetAsync(dto.LastModifiedBy, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<User>()).Verifiable();

        var result = await controller.CreateUser(officeId, dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid modifying user id.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_UserCreation_With_InvalidRoleName()
    {
        int officeId = randomizer.Next();
        var dto = new UserDto
        {
            FirstName = randomizer.GetString(),
            LastName = randomizer.GetString(),
            RoleName = randomizer.GetString(),
            IsActive = true,
            LastModifiedBy = randomizer.Next()
        };

        userQueryRepoMock.Setup(r => r.GetAsync(dto.LastModifiedBy, It.IsAny<CancellationToken>())).ReturnsAsync(new User()).Verifiable();
        roleQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<Role>()).Verifiable();

        var result = await controller.CreateUser(officeId, dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid user role.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_UserCreation_With_Existing_UserName()
    {
        int officeId = randomizer.Next();
        var dto = new UserDto
        {
            FirstName = randomizer.GetString(),
            LastName = randomizer.GetString(),
            RoleName = randomizer.GetString(),
            IsActive = true,
            LastModifiedBy = randomizer.Next()
        };
        var roleByName = new Role { RoleId = randomizer.Next(), RoleName = dto.RoleName, IsActive = true };
        var user = new User { UserId = randomizer.NextInt64(), FirstName = dto.FirstName, LastName = dto.LastName, IsActive = dto.IsActive };

        userQueryRepoMock.Setup(r => r.GetAsync(dto.LastModifiedBy, It.IsAny<CancellationToken>())).ReturnsAsync(new User()).Verifiable();
        roleQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { roleByName }).Verifiable();
        userQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new User() }).Verifiable();

        var result = await controller.CreateUser(officeId, dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "First and last name combo exists.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetUserDetails()
    {
        long userId = randomizer.NextInt64();
        var user = new User { UserId = userId, FirstName = randomizer.GetString(), LastName = randomizer.GetString(), IsActive = true };

        userQueryRepoMock.Setup(r => r.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user).Verifiable();

        var result = await controller.GetUserDetails(userId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.AreEqual(user, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_GetUserDetails_When_NoData()
    {
        long userId = randomizer.NextInt64();
        userQueryRepoMock.Setup(r => r.GetAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<User>()).Verifiable();

        var result = await controller.GetUserDetails(userId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<User>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    private ActivityLog MatchActivityLog(int officeId, long creatingUserId, User user)
    {
        return Match.Create<ActivityLog>(
            x => x.UserId == creatingUserId &&
            x.OfficeId == officeId &&
            x.Action == $"{ActivityLogType.CreateUser}" &&
            x.Description.Equals($"{ActivityLogType.CreateUser} - {nameof(user.UserId)} - {user.UserId} - Success"));
    }

    [TearDown]
    public void TearDown()
    {
        userQueryRepoMock.Verify();
        roleQueryRepoMock.Verify();
        userCommandRepoMock.Verify();
        activityCommandRepoMock.Verify();
    }
}
