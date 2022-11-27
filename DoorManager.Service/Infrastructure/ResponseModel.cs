using DoorManager.Service.ServiceResponse;

namespace DoorManager.Service.Infrastructure;

public class ResponseModel<T> : ServiceResult<T>
{
    public ResponseModel(ServiceResult<T> serviceResult)
    {
        this.ServiceResult = serviceResult;
    }

    public ServiceResult<T> ServiceResult { get; }
}
