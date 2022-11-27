using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Infrastructure;

public class Bus : IBus
{
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public Bus(IMediator mediator, ILogger<IBus> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation($"Invoked '{request.GetType().Name}.");
        return this.mediator.Send(request, cancellationToken);
    }
}
