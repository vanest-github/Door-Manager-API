using DoorManager.Service.ServiceResponse;

namespace DoorManager.Service.Infrastructure.Interfaces;

public interface IResponseModel<T>
{
    ServiceResult<T> ServiceResult { get; }
}
