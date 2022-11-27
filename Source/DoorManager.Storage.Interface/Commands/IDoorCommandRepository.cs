using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IDoorCommandRepository : ICreatable<Door>, IUpdatable<Guid, Door>, IDeletable<Guid>
{

}