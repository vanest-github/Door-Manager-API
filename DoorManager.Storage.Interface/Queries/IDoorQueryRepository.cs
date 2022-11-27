using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Queries;

public interface IDoorQueryRepository : IReadable<Guid, Door>, ISearchable<Door>
{
    Task<DoorType> GetDoorType(string doorTypeName, CancellationToken cancellationToken = default);

    Task<IEnumerable<DoorType>> GetDoorTypes(CancellationToken cancellationToken = default);
}