using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Queries;

public interface IRoleQueryRepository : IReadable<int, Role>, ISearchable<Role>
{

}