using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Api.ApiResponses;
using DoorManager.Api.Controllers;
using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure;
using DoorManager.Service.Infrastructure.Interfaces;
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

public class ActivityLogControllerTest
{
    private IBus bus;
    private ActivityLogController controller;
    private Mock<ILogger<Bus>> logger;
    private Mock<ILogger<IBus>> loggerHandler;
    private Mock<IActivityQueryRepository> activityQueryRepoMock;
    private Mock<IRBACQueryRepository> rbacQueryRepository;
    private Mock<ClaimsPrincipal> claimsPrincipalMock;
    private Randomizer randomizer;
    private int loggedInRoleId;

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<Bus>>();
        loggerHandler = new Mock<ILogger<IBus>>();
        activityQueryRepoMock = new Mock<IActivityQueryRepository>();
        rbacQueryRepository = new Mock<IRBACQueryRepository>();
        claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        randomizer = TestContext.CurrentContext.Random;
        loggedInRoleId = randomizer.Next();

        var services = new ServiceCollection();
        services.AddScoped(sp => bus);
        services.AddScoped(sp => logger.Object);
        services.AddScoped(sp => loggerHandler.Object);
        services.AddScoped(sp => activityQueryRepoMock.Object);
        services.AddScoped(sp => rbacQueryRepository.Object);
        services.AddScoped(sp => claimsPrincipalMock.Object);

        services.AddMediatR(typeof(Service.RBAC.Validate.Handler));
        services.AddMediatR(typeof(Service.RBAC.Get.ByFeatureId.Handler));
        services.AddMediatR(typeof(Service.Activity.Query.Handler));

        var mediator = services.BuildServiceProvider().GetService<IMediator>();
        bus = new Bus(mediator, logger.Object);
        controller = new ActivityLogController(bus);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, $"{loggedInRoleId}") }));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }

    [Test]
    public async Task Pass_GetActivities_ByUserId()
    {
        var dto = new ActivityLogQueryDto
        {
            UserId = randomizer.NextInt64(),
        };
        var activityLogs = new[]
        {
            new ActivityLog { UserId = dto.UserId.Value, Action = ActivityLogType.SelfAccess.ToString() },
            new ActivityLog { UserId = dto.UserId.Value, Action = ActivityLogType.AccessDelegation.ToString() },
            new ActivityLog { UserId = dto.UserId.Value, Action = ActivityLogType.CreateUser.ToString() }
        };

        activityQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(activityLogs).Verifiable();
        activityQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(activityLogs).Verifiable();
        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = loggedInRoleId } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<ActivityLog>>;
        Assert.AreEqual(activityLogs, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetActivities_ByOfficeId()
    {
        var dto = new ActivityLogQueryDto
        {
            OfficeId = randomizer.Next(),
        };
        var activityLogs = new[]
        {
            new ActivityLog { OfficeId = dto.OfficeId.Value, Action = ActivityLogType.SelfAccess.ToString() },
            new ActivityLog { OfficeId = dto.OfficeId.Value, Action = ActivityLogType.AccessDelegation.ToString() },
            new ActivityLog { OfficeId = dto.OfficeId.Value, Action = ActivityLogType.CreateUser.ToString() }
        };

        activityQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(activityLogs).Verifiable();
        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = loggedInRoleId } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<ActivityLog>>;
        Assert.AreEqual(activityLogs, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetActivities_ByTime()
    {
        var dto = new ActivityLogQueryDto
        {
            FromTime = DateTimeOffset.UtcNow.AddTicks(-randomizer.Next()),
            ToTime = DateTimeOffset.UtcNow,
        };
        var activityLogs = new[]
        {
            new ActivityLog { OfficeId = randomizer.Next(), UserId = randomizer.Next(), Action = ActivityLogType.SelfAccess.ToString() },
            new ActivityLog { OfficeId = randomizer.Next(), UserId = randomizer.Next(), Action = ActivityLogType.AccessDelegation.ToString() },
            new ActivityLog { OfficeId = randomizer.Next(), UserId = randomizer.Next(), Action = ActivityLogType.CreateUser.ToString() }
        };

        activityQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(activityLogs).Verifiable();
        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = loggedInRoleId } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<ActivityLog>>;
        Assert.AreEqual(activityLogs, apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_GetActivities_NoCriteria()
    {
        var dto = new ActivityLogQueryDto();

        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = loggedInRoleId } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<ActivityLog>>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "No query criterion has been set.");
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Test]
    public async Task Pass_GetActivities_When_NoData()
    {
        var dto = new ActivityLogQueryDto
        {
            OfficeId = randomizer.Next(),
            UserId = randomizer.Next(),
            FromTime = DateTimeOffset.UtcNow.AddTicks(-randomizer.Next()),
            ToTime = DateTimeOffset.UtcNow,
        };

        activityQueryRepoMock.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<ActivityLog, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<ActivityLog>()).Verifiable();
        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = loggedInRoleId } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<IEnumerable<ActivityLog>>;
        Assert.AreEqual(Enumerable.Empty<ActivityLog>(), apiMessage.Data);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }

    [Test]
    public async Task Fail_GetActivities_When_Unauthorized()
    {
        var dto = new ActivityLogQueryDto
        {
            OfficeId = randomizer.Next(),
            UserId = randomizer.Next(),
            FromTime = DateTimeOffset.UtcNow.AddTicks(-randomizer.Next()),
            ToTime = DateTimeOffset.UtcNow,
        };

        rbacQueryRepository.Setup(r => r.SearchByPredicateAsync(It.IsAny<Expression<Func<RoleFeature, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new RoleFeature { RoleId = randomizer.Next() } }).Verifiable();

        var result = await controller.QueryUserActivites(dto) as ObjectResult;
        var apiMessage = result.Value as ApiMessage<ActivityLog>;
        Assert.IsNull(apiMessage.Data);
        Assert.AreEqual(apiMessage.Message, "Permission Denied.");
        Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [TearDown]
    public void TearDown()
    {
        activityQueryRepoMock.Verify();
        rbacQueryRepository.Verify();
    }
}
