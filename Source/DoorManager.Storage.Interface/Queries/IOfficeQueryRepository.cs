using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Queries;

public interface IOfficeQueryRepository : IReadable<int, Office>, ISearchable<Office>
{

}