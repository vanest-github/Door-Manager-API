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

public class OfficeControllerTest
{
    private IBus bus;
    private OfficeController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IOfficeCommandRepository> officeCommandRepoMock;
    private Mock<IOfficeQueryRepository> officeQueryRepoMock;
    private Randomizer randomizer;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        officeCommandRepoMock = new Mock<IOfficeCommandRepository>();
        officeQueryRepoMock = new Mock<IOfficeQueryRepository>();
        randomizer = TestContext.CurrentContext.Random;

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => officeCommandRepoMock.Object);
        services.AddScoped(sp => officeQueryRepoMock.Object);

        services.AddMediatR(typeof(Service.Office.Create.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new OfficeController(bus);
    }

    [Test]
    public async Task Pass_OfficeCreation()
    {
        var dto = new OfficeDto
        {
            OfficeName = randomizer.GetString(),
            Country = randomizer.GetString(),
            Latitude = randomizer.NextDecimal(),
            Longitude = randomizer.NextDecimal()
        };
        var office = new Office { OfficeId = randomizer.Next(), OfficeName = dto.OfficeName, Country = dto.Country, Latitude = dto.Latitude, Longitude = dto.Longitude, IsActive = true };

        officeCommandRepoMock.Setup(r => r.CreateAsync(It.IsAny<Office>(), It.IsAny<CancellationToken>())).ReturnsAsync(office).Verifiable();

        var result = await controller.CreateOffice(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Office>;
        Assert.AreEqual(office, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
    }

    [Test]
    public async Task Fail_OfficeCreation_With_Exception()
    {
        var dto = new OfficeDto
        {
            OfficeName = randomizer.GetString(),
            Country = randomizer.GetString(),
            Latitude = randomizer.NextDecimal(),
            Longitude = randomizer.NextDecimal()
        };
        officeCommandRepoMock.Setup(r => r.CreateAsync(It.IsAny<Office>(), It.IsAny<CancellationToken>())).Throws(new Exception()).Verifiable();

        var result = await controller.CreateOffice(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<Office>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetAllOffices()
    {
        var offices = new[]
        {
            new Office { OfficeName = randomizer.GetString(), Country = randomizer.GetString(), IsActive = true },
            new Office { OfficeName = randomizer.GetString(), Country = randomizer.GetString(), IsActive = true },
            new Office { OfficeName = randomizer.GetString(), Country = randomizer.GetString(), IsActive = true },
        };

        officeQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<Office, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(offices).Verifiable();

        var result = await controller.GetAllOffices() as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<Office>>;
        Assert.AreEqual(offices, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [TearDown]
    public void TearDown()
    {
        officeCommandRepoMock.Verify();
        officeQueryRepoMock.Verify();
    }
}
