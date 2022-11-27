using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IRoleCommandRepository : ICreatable<Role>, IUpdatable<int, Role>, IDeletable<int>
{

}