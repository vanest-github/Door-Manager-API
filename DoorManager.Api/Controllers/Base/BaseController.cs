using System.Net;
using DoorManager.Api.ApiResponses;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using Microsoft.AspNetCore.Mvc;

namespace DoorManager.Api.Controllers.Base;

public class BaseController : ControllerBase
{
    public async Task<IActionResult> CreateResponse<T>(Task<ServiceResult<T>> task, IDictionary<string, object> messageKeys)
    {
        var result = await task.ConfigureAwait(false);
        var apiResponse = CreateApiResponse(result, messageKeys);
        return this.StatusCode((int)apiResponse.StatusCode, apiResponse.Data);
    }

    private static ApiResponse CreateApiResponse<T>(ServiceResult<T> result, IDictionary<string, object> messageKeys)
    {
        HttpStatusCode statusCode;
        string messageKey = MessageKeyType.ErrorKey.ToString();
        if (!result.Success)
        {
            statusCode = result.Exceptions?.FirstOrDefault() switch
            {
                null => HttpStatusCode.Forbidden,
                ResourceConflictException => HttpStatusCode.Conflict,
                BadRequestException => HttpStatusCode.BadRequest,
                ResourceNotFoundException => HttpStatusCode.NotFound,
                NotImplementedException => HttpStatusCode.NotImplemented,
                UnauthorizedOperationException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError,
            };
        }
        else if (!result.HasData || result.Data == null)
        {
            statusCode = HttpStatusCode.NotFound;
        }
        else
        {
            statusCode = result.StatusCode;
            messageKey = MessageKeyType.SuccessKey.ToString();
        }

        var apiResponse = new ApiResponse
        {
            StatusCode = statusCode,
            Data = Value(result.ErrorMessage ?? (string)messageKeys[messageKey], result.Data),
        };
        return apiResponse;
    }

    private static ApiMessage<T> Value<T>(string message, T data)
    {
        var apiMessage = new ApiMessage<T>()
        {
            Message = message,
            Data = data,
        };

        return apiMessage;
    }
}
