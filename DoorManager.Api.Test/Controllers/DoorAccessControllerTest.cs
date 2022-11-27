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

public class DoorAccessControllerTest
{
    private IBus bus;
    private DoorAccessController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IDoorAccessQueryRepository> doorAccessQueryRepoMock;
    private Mock<IActivityCommandRepository> activityCommandRepoMock;
    private Mock<IConfiguration> configurationMock;
    private Mock<IOptions<GlobalConfiguration>> globalConfigurationMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        doorAccessQueryRepoMock = new Mock<IDoorAccessQueryRepository>();
        activityCommandRepoMock = new Mock<IActivityCommandRepository>();
        configurationMock = new Mock<IConfiguration>();
        globalConfigurationMock = new Mock<IOptions<GlobalConfiguration>>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => doorAccessQueryRepoMock.Object);
        services.AddScoped(sp => activityCommandRepoMock.Object);
        services.AddScoped(sp => configurationMock.Object);
        services.AddScoped(sp => globalConfigurationMock.Object);

        services.AddMediatR(typeof(Service.DoorAccess.Unlock.Handler));
        services.AddMediatR(typeof(Service.DoorAccess.Self.Handler));
        services.AddMediatR(typeof(Service.DoorAccess.Delegated.Handler));
        services.AddMediatR(typeof(Service.Activity.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new DoorAccessController(bus);
    }

    [Test]
    public async Task UnlockDoor_SelfAccess_PassAndUnlock()
    {
        var doorUnlockDto = new DoorUnlockDto
        {
            UserId = randomizer.NextInt64(),
            OfficeId = randomizer.Next(),
            DoorId = randomizer.NextGuid(),
            DoorAccessMode = DoorAccessMode.SelfAccess
        };

        doorAccessQueryRepoMock.Setup(r => r.GetSelfAccessDoorsAsync(doorUnlockDto.UserId, doorUnlockDto.OfficeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new[] { doorUnlockDto.DoorId }.AsEnumerable())).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(doorUnlockDto, true), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ActivityLog())).Verifiable();

        var result = await controller.UnlockDoor(doorUnlockDto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<bool>;
        Assert.IsTrue(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task UnlockDoor_SelfAccess_DenyDoorAccess()
    {
        var doorUnlockDto = new DoorUnlockDto
        {
            UserId = randomizer.NextInt64(),
            OfficeId = randomizer.Next(),
            DoorId = randomizer.NextGuid(),
            DoorAccessMode = DoorAccessMode.SelfAccess
        };

        doorAccessQueryRepoMock.Setup(r => r.GetSelfAccessDoorsAsync(doorUnlockDto.UserId, doorUnlockDto.OfficeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new[] { randomizer.NextGuid() }.AsEnumerable())).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(doorUnlockDto, false), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ActivityLog())).Verifiable();

        var result = await controller.UnlockDoor(doorUnlockDto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<bool>;
        Assert.IsFalse(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Access Denied.");
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Test]
    public async Task UnlockDoor_DelegatedAccess_PassAndUnlock()
    {
        var doorUnlockDto = new DoorUnlockDto
        {
            UserId = randomizer.NextInt64(),
            OfficeId = randomizer.Next(),
            DoorId = randomizer.NextGuid(),
            DoorAccessMode = DoorAccessMode.DelegatedAccess
        };

        doorAccessQueryRepoMock.Setup(r => r.GetProxyAccessDoorsAsync(doorUnlockDto.UserId, doorUnlockDto.OfficeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new[] { doorUnlockDto.DoorId }.AsEnumerable())).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(doorUnlockDto, true), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ActivityLog())).Verifiable();

        var result = await controller.UnlockDoor(doorUnlockDto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<bool>;
        Assert.IsTrue(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task UnlockDoor_DelegatedAccess_DenyDoorAccess()
    {
        var doorUnlockDto = new DoorUnlockDto
        {
            UserId = randomizer.NextInt64(),
            OfficeId = randomizer.Next(),
            DoorId = randomizer.NextGuid(),
            DoorAccessMode = DoorAccessMode.DelegatedAccess
        };

        doorAccessQueryRepoMock.Setup(r => r.GetProxyAccessDoorsAsync(doorUnlockDto.UserId, doorUnlockDto.OfficeId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new[] { randomizer.NextGuid() }.AsEnumerable())).Verifiable();
        activityCommandRepoMock.Setup(r => r.CreateAsync(MatchActivityLog(doorUnlockDto, false), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ActivityLog())).Verifiable();

        var result = await controller.UnlockDoor(doorUnlockDto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<bool>;
        Assert.IsFalse(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Access Denied.");
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    private ActivityLog MatchActivityLog(DoorUnlockDto dto, bool isValid)
    {
        var outcomeMessage = isValid ? "Success" : "Denied";
        return Match.Create<ActivityLog>(
            x => x.UserId == dto.UserId && x.OfficeId == dto.OfficeId && x.Action == $"{dto.DoorAccessMode}" && x.Description.Equals($"{dto.DoorAccessMode} - {nameof(dto.DoorId)} - {dto.DoorId} - {outcomeMessage}"));
    }

    [TearDown]
    public void TearDown()
    {
        doorAccessQueryRepoMock.Verify();
        activityCommandRepoMock.Verify();
    }
}
