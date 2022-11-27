using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoorManager.Api.ApiResponses;
using DoorManager.Api.Controllers;
using DoorManager.Entity.Configurations;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Storage.Interface.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using DoorManager.Storage.Interface.Commands;
using DoorManager.Entity;
using System.Threading;

namespace DoorManager.Api.Test.Controllers;

public class AccessDelegationControllerTest
{
    private IBus bus;
    private AccessDelegationController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IUserQueryRepository> userQueryRepoMock;
    private Mock<IRoleQueryRepository> roleQueryRepoMock;
    private Mock<IUserOfficeRoleCommandRepository> userOfficeRoleCommandRepoMock;
    private Mock<IActivityCommandRepository> activityCommandRepoMock;
    private Mock<IConfiguration> configurationMock;
    private Mock<IOptions<GlobalConfiguration>> globalConfigurationMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        userQueryRepoMock = new Mock<IUserQueryRepository>();
        roleQueryRepoMock = new Mock<IRoleQueryRepository>();
        userOfficeRoleCommandRepoMock = new Mock<IUserOfficeRoleCommandRepository>();
        activityCommandRepoMock = new Mock<IActivityCommandRepository>();
        configurationMock = new Mock<IConfiguration>();
        globalConfigurationMock = new Mock<IOptions<GlobalConfiguration>>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => userQueryRepoMock.Object);
        services.AddScoped(sp => roleQueryRepoMock.Object);
        services.AddScoped(sp => userOfficeRoleCommandRepoMock.Object);
        services.AddScoped(sp => activityCommandRepoMock.Object);
        services.AddScoped(sp => configurationMock.Object);
        services.AddScoped(sp => globalConfigurationMock.Object);

        services.AddMediatR(typeof(Service.UserOfficeRole.Delegate.Handler));
        services.AddMediatR(typeof(Service.UserOfficeRole.Get.ByUserIds.Handler));
        services.AddMediatR(typeof(Service.Role.Get.ById.Handler));
        services.AddMediatR(typeof(Service.UserOfficeRole.Create.Handler));
        services.AddMediatR(typeof(Service.Activity.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new AccessDelegationController(bus);
    }

    [Test]
    public async Task Pass_AccessDelegation_With_LowerPriorityRole_ValidTargetUser_ValidTime()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var toRole = new Role { RoleId = dto.RoleId, RolePriority = randomizer.Next(), IsActive = true };
        var activeUserRoleDto = new[]
        {
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.IssuingUserId, RoleId = randomizer.Next(), RolePriority = toRole.RolePriority + 1 },
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.TargetUserId, RoleId = randomizer.Next(), RolePriority = toRole.RolePriority - 1 },
        };
        var userOfficeRole = new UserOfficeRole { UserOfficeRoleId = randomizer.NextInt64(), OfficeId = dto.OfficeId, UserId = dto.IssuingUserId };

        userQueryRepoMock.Setup(r => r.GetActiveUserRoles(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }, It.IsAny<CancellationToken>())).ReturnsAsync(activeUserRoleDto.AsEnumerable()).Verifiable();
        roleQueryRepoMock.Setup(r => r.GetAsync(dto.RoleId, It.IsAny<CancellationToken>())).ReturnsAsync(toRole).Verifiable();
        userOfficeRoleCommandRepoMock.Setup(r => r.CreateAsync(It.IsAny<UserOfficeRole>(), It.IsAny<CancellationToken>())).ReturnsAsync(userOfficeRole).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(userOfficeRole), It.IsAny<CancellationToken>())).ReturnsAsync(new ActivityLog()).Verifiable();

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.AreEqual(apiMessage.Data, userOfficeRole);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessDelegation_With_HigherPriorityRole()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var toRole = new Role { RoleId = dto.RoleId, RolePriority = randomizer.Next(), IsActive = true };
        var activeUserRoleDto = new[]
        {
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.IssuingUserId, RoleId = randomizer.Next(), RolePriority = toRole.RolePriority - 1 },
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.TargetUserId, RoleId = randomizer.Next(), RolePriority = toRole.RolePriority - 1 },
        };

        userQueryRepoMock.Setup(r => r.GetActiveUserRoles(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }, It.IsAny<CancellationToken>())).ReturnsAsync(activeUserRoleDto.AsEnumerable()).Verifiable();
        roleQueryRepoMock.Setup(r => r.GetAsync(dto.RoleId, It.IsAny<CancellationToken>())).ReturnsAsync(toRole).Verifiable();

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid role to delegate.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessDelegation_With_InvalidIssuer()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var activeUserRoleDto = new[]
        {
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.TargetUserId, RoleId = randomizer.Next(), RolePriority = randomizer.Next() },
        };

        userQueryRepoMock.Setup(r => r.GetActiveUserRoles(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }, It.IsAny<CancellationToken>())).ReturnsAsync(activeUserRoleDto.AsEnumerable()).Verifiable();

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid or inactive user.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessDelegation_With_InvalidTargetUser()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var activeUserRoleDto = new[]
        {
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.IssuingUserId, RoleId = randomizer.Next(), RolePriority = randomizer.Next() },
        };

        userQueryRepoMock.Setup(r => r.GetActiveUserRoles(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }, It.IsAny<CancellationToken>())).ReturnsAsync(activeUserRoleDto.AsEnumerable()).Verifiable();

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid or inactive target user.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessDelegation_When_RoleExisting()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var toRole = new Role { RoleId = dto.RoleId, RolePriority = randomizer.Next(), IsActive = true };
        var activeUserRoleDto = new[]
        {
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.IssuingUserId, RoleId = randomizer.Next(), RolePriority = toRole.RolePriority },
            new ActiveUserRoleDto { OfficeId = dto.OfficeId, UserId = dto.TargetUserId, RoleId = toRole.RoleId, RolePriority = toRole.RolePriority },
        };

        userQueryRepoMock.Setup(r => r.GetActiveUserRoles(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }, It.IsAny<CancellationToken>())).ReturnsAsync(activeUserRoleDto.AsEnumerable()).Verifiable();

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Target user already has the role.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessDelegation_With_Invalid_Duration()
    {
        var dto = new AccessDelegationDto
        {
            OfficeId = randomizer.Next(),
            IssuingUserId = randomizer.Next(),
            RoleId = randomizer.Next(),
            TargetUserId = randomizer.Next(),
            ValidTo = DateTimeOffset.UtcNow.AddTicks(-randomizer.Next())
        };

        var result = await controller.DelegateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<UserOfficeRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid date range.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    private ActivityLog MatchActivityLog(UserOfficeRole userOfficeRole)
    {
        return Match.Create<ActivityLog>(
            x => x.UserId == userOfficeRole.UserId &&
            x.OfficeId == userOfficeRole.OfficeId &&
            x.Action == $"{ActivityLogType.AccessDelegation}" &&
            x.Description.Equals($"{ActivityLogType.AccessDelegation} - {nameof(userOfficeRole.UserOfficeRoleId)} - {userOfficeRole.UserOfficeRoleId} - Success"));
    }

    [TearDown]
    public void TearDown()
    {
        userQueryRepoMock.Verify();
        roleQueryRepoMock.Verify();
        userOfficeRoleCommandRepoMock.Verify();
        activityCommandRepoMock.Verify();
    }
}
