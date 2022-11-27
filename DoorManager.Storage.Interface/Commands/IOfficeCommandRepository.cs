using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IOfficeCommandRepository : ICreatable<Office>, IUpdatable<int, Office>, IDeletable<Office>
{

}