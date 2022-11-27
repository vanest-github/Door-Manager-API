using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DoorManager.Service.Infrastructure.Interfaces;

public interface IBus
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
