using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Queries;

public interface IDoorAccessQueryRepository : ISearchable<DoorAccessRole>
{
    Task<IEnumerable<Guid>> GetSelfAccessDoorsAsync(long userId, int officeId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> GetProxyAccessDoorsAsync(long userId, int officeId, CancellationToken cancellationToken = default);
}