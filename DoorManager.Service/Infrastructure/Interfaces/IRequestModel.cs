using System.Collections.Generic;

namespace DoorManager.Service.Infrastructure.Interfaces;

public interface IRequestModel
{
    Dictionary<string, object> Decoratives { get; }
}
