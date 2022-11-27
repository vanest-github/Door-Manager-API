using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IUserOfficeRoleCommandRepository : ICreatable<UserOfficeRole>, IUpdatable<long, UserOfficeRole>, IDeletable<long>
{

}