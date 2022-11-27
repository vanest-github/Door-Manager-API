using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Api.ApiResponses;
using DoorManager.Api.Controllers;
using DoorManager.Entity;
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
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DoorManager.Api.Test.Controllers;

public class AccessProvisionControllerTest
{
    private IBus bus;
    private AccessProvisionController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IDoorQueryRepository> doorQueryRepoMock;
    private Mock<IRoleQueryRepository> roleQueryRepoMock;
    private Mock<IDoorAccessCommandRepository> doorAccessCommandRepoMock;
    private Mock<IActivityCommandRepository> activityCommandRepoMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        doorQueryRepoMock = new Mock<IDoorQueryRepository>();
        roleQueryRepoMock = new Mock<IRoleQueryRepository>();
        doorAccessCommandRepoMock = new Mock<IDoorAccessCommandRepository>();
        activityCommandRepoMock = new Mock<IActivityCommandRepository>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => doorQueryRepoMock.Object);
        services.AddScoped(sp => roleQueryRepoMock.Object);
        services.AddScoped(sp => doorAccessCommandRepoMock.Object);
        services.AddScoped(sp => activityCommandRepoMock.Object);

        services.AddMediatR(typeof(Service.DoorAccess.Create.Handler));
        services.AddMediatR(typeof(Service.Role.Get.ByName.Handler));
        services.AddMediatR(typeof(Service.Activity.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new AccessProvisionController(bus);
    }

    [Test]
    public async Task Pass_AccessProvision_With_ValidDoorType_ValidRole_ValidTime()
    {
        var dto = new AccessProvisionDto
        {
            OfficeId = randomizer.Next(),
            RoleName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            AccessTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var doorType = new DoorType { DoorTypeId = randomizer.Next(), DoorTypeName = dto.DoorType };
        var roleByName = new Role { RoleId = randomizer.Next(), RoleName = dto.RoleName, IsActive = true };
        var doorAccessRole = new DoorAccessRole { DoorAccessId = randomizer.NextInt64(), OfficeId = dto.OfficeId };

        doorQueryRepoMock.Setup(r => r.GetDoorType(dto.DoorType, It.IsAny<CancellationToken>())).ReturnsAsync(doorType).Verifiable();
        roleQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { roleByName }).Verifiable();
        doorAccessCommandRepoMock.Setup(r => r.CreateAsync(It.IsAny<DoorAccessRole>(), It.IsAny<CancellationToken>())).ReturnsAsync(doorAccessRole).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(dto, doorAccessRole), It.IsAny<CancellationToken>())).ReturnsAsync(new ActivityLog()).Verifiable();

        var result = await controller.CreateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<DoorAccessRole>;
        Assert.AreEqual(apiMessage.Data, doorAccessRole);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessProvision_With_InvalidDoorType()
    {
        var dto = new AccessProvisionDto
        {
            OfficeId = randomizer.Next(),
            RoleName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            AccessTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };

        doorQueryRepoMock.Setup(r => r.GetDoorType(dto.DoorType, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<DoorType>()).Verifiable();

        var result = await controller.CreateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<DoorAccessRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid door type.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessProvision_With_InvalidRoleName()
    {
        var dto = new AccessProvisionDto
        {
            OfficeId = randomizer.Next(),
            RoleName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            AccessTo = DateTimeOffset.UtcNow.AddTicks(randomizer.Next())
        };
        var doorType = new DoorType { DoorTypeId = randomizer.Next(), DoorTypeName = dto.DoorType };

        doorQueryRepoMock.Setup(r => r.GetDoorType(dto.DoorType, It.IsAny<CancellationToken>())).ReturnsAsync(doorType).Verifiable();
        roleQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<Role>()).Verifiable();

        var result = await controller.CreateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<DoorAccessRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid role name.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_AccessProvision_With_InvalidDuration()
    {
        var dto = new AccessProvisionDto
        {
            OfficeId = randomizer.Next(),
            RoleName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            AccessTo = DateTimeOffset.UtcNow.AddTicks(-randomizer.Next())
        };

        var result = await controller.CreateDoorAccess(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<DoorAccessRole>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Invalid date range.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    private ActivityLog MatchActivityLog(AccessProvisionDto dto, DoorAccessRole doorAccessRole)
    {
        return Match.Create<ActivityLog>(
            x => x.UserId == 1 &&
            x.OfficeId == doorAccessRole.OfficeId &&
            x.Action == $"{ActivityLogType.AccessProvision}" &&
            x.Description.Equals($"{ActivityLogType.AccessProvision} - {nameof(doorAccessRole.DoorAccessId)} - {doorAccessRole.DoorAccessId} - {dto.DoorType} - {dto.RoleName} - Success"));
    }

    [TearDown]
    public void TearDown()
    {
        doorQueryRepoMock.Verify();
        roleQueryRepoMock.Verify();
        doorAccessCommandRepoMock.Verify();
        activityCommandRepoMock.Verify();
    }
}
