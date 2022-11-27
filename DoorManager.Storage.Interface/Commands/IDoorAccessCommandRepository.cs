using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IDoorAccessCommandRepository : ICreatable<DoorAccessRole>, IUpdatable<long, DoorAccessRole>, IDeletable<long>
{

}