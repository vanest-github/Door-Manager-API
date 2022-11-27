using System;
using System.Collections.Generic;
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

public class DoorControllerTest
{
    private IBus bus;
    private DoorController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IOfficeQueryRepository> officeQueryRepoMock;
    private Mock<IDoorCommandRepository> doorCommandRepoMock;
    private Mock<IDoorQueryRepository> doorQueryRepoMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        officeQueryRepoMock = new Mock<IOfficeQueryRepository>();
        doorCommandRepoMock = new Mock<IDoorCommandRepository>();
        doorQueryRepoMock = new Mock<IDoorQueryRepository>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => officeQueryRepoMock.Object);
        services.AddScoped(sp => doorCommandRepoMock.Object);
        services.AddScoped(sp => doorQueryRepoMock.Object);

        services.AddMediatR(typeof(Service.Door.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new DoorController(bus);
    }

    [Test]
    public async Task Pass_DoorCreation_With_ValidOfficeName_ValidType()
    {
        var dto = new DoorDto
        {
            OfficeName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            DoorStatus = DoorStatus.Closed
        };
        var office = new Office { OfficeId = randomizer.Next(), OfficeName = dto.OfficeName, Country = randomizer.GetString(), IsActive = true };
        var doorType = new DoorType { DoorTypeId = randomizer.Next(), DoorTypeName = dto.DoorType };
        var door = new Door { DoorId = randomizer.NextGuid(), DoorTypeId = doorType.DoorTypeId, OfficeId = office.OfficeId };

        officeQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Office, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { office }).Verifiable();
        doorQueryRepoMock.Setup(r => r.GetDoorType(dto.DoorType, It.IsAny<CancellationToken>())).ReturnsAsync(doorType).Verifiable();
        doorCommandRepoMock.Setup(r => r.CreateAsync(It.IsAny<Door>(), It.IsAny<CancellationToken>())).ReturnsAsync(door).Verifiable();

        var result = await controller.CreateDoor(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Door>;
        Assert.AreEqual(door, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [Test]
    public async Task Fail_DoorCreation_With_InvalidOfficeName()
    {
        var dto = new DoorDto
        {
            OfficeName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            DoorStatus = DoorStatus.Closed
        };

        officeQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Office, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<Office>()).Verifiable();

        var result = await controller.CreateDoor(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Door>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, $"Invalid office name: {dto.OfficeName}");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Fail_DoorCreation_With_InvalidDoorType()
    {
        var dto = new DoorDto
        {
            OfficeName = randomizer.GetString(),
            DoorType = randomizer.GetString(),
            DoorStatus = DoorStatus.Closed
        };
        var office = new Office { OfficeId = randomizer.Next(), OfficeName = dto.OfficeName, Country = randomizer.GetString(), IsActive = true };

        officeQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Office, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { office }).Verifiable();
        doorQueryRepoMock.Setup(r => r.GetDoorType(dto.DoorType, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<DoorType>()).Verifiable();

        var result = await controller.CreateDoor(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Door>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, $"Invalid door type: {dto.DoorType}");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetAllDoors_ByOfficeId()
    {
        int officeId = randomizer.Next();
        var doors = new[]
        {
            new Door { DoorId = randomizer.NextGuid(), DoorTypeId = randomizer.Next(), OfficeId = officeId },
            new Door { DoorId = randomizer.NextGuid(), DoorTypeId = randomizer.Next(), OfficeId = officeId },
            new Door { DoorId = randomizer.NextGuid(), DoorTypeId = randomizer.Next(), OfficeId = officeId },
        };

        doorQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Door, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(doors).Verifiable();

        var result = await controller.GetAllDoors(officeId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<Door>>;
        Assert.AreEqual(doors, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_GetAllDoors_ByOfficeId_When_NoRecords()
    {
        int officeId = randomizer.Next();
        doorQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Door, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<IEnumerable<Door>>()).Verifiable();

        var result = await controller.GetAllDoors(officeId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<Door>>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetAllDoorTypes()
    {
        var doorTypes = new[]
        {
            new DoorType { DoorTypeId = randomizer.Next(), DoorTypeName = randomizer.GetString(), IsActive = true },
            new DoorType { DoorTypeId = randomizer.Next(), DoorTypeName = randomizer.GetString(), IsActive = true },
        };

        doorQueryRepoMock.Setup(r => r.GetDoorTypes(It.IsAny<CancellationToken>())).ReturnsAsync(doorTypes).Verifiable();

        var result = await controller.GetDoorTypes() as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<DoorType>>;
        Assert.AreEqual(doorTypes, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetDoor_ById()
    {
        var doorId = randomizer.NextGuid();
        var door = new Door { DoorId = doorId, DoorTypeId = randomizer.Next(), OfficeId = randomizer.Next() };

        doorQueryRepoMock.Setup(r => r.GetAsync(doorId, It.IsAny<CancellationToken>())).ReturnsAsync(door).Verifiable();

        var result = await controller.GetDoorDetails(randomizer.Next(), doorId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Door>;
        Assert.AreEqual(door, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_GetDoor_ById_When_NoData()
    {
        var doorId = randomizer.NextGuid();
        var door = new Door { DoorId = doorId, DoorTypeId = randomizer.Next(), OfficeId = randomizer.Next() };

        doorQueryRepoMock.Setup(r => r.GetAsync(doorId, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Door>()).Verifiable();

        var result = await controller.GetDoorDetails(randomizer.Next(), doorId) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Door>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [TearDown]
    public void TearDown()
    {
        officeQueryRepoMock.Verify();
        doorQueryRepoMock.Verify();
        doorCommandRepoMock.Verify();
    }
}
