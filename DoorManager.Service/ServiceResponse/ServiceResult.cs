using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace DoorManager.Service.ServiceResponse;

public class ServiceResult<T>
{
    public string ErrorMessage { get; set; }

    public bool Success { get; set; }

    public bool HasData { get; set; }

    public T Data { get; set; }

    public bool IsErrorMessage { get; set; }

    public IEnumerable<Exception> Exceptions { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public static ServiceResult<T> Created()
    {
        return new ServiceResult<T>()
        {
            Success = true,
            HasData = false,
            StatusCode = HttpStatusCode.Created,
        };
    }

    public static ServiceResult<T> Created(T data, bool isSuccess = true) => new()
    {
        Success = isSuccess,
        HasData = true,
        Data = data,
        Exceptions = null,
        StatusCode = HttpStatusCode.Created,
    };

    public static ServiceResult<T> CreateSuccess(T data = default, bool isSuccess = true)
    {
        return new ServiceResult<T>()
        {
            Success = isSuccess,
            HasData = true,
            Data = data,
            StatusCode = HttpStatusCode.OK
        };
    }

    public static ServiceResult<T> CreateError(params Exception[] exceptions)
    {
        return new ServiceResult<T>()
        {
            Success = false,
            HasData = false,
            Data = default,
            Exceptions = exceptions.AsEnumerable(),
        };
    }

    public static ServiceResult<T> CreateError(T data, string errorMessage, bool isErrorMessage = false)
    {
        return new ServiceResult<T>()
        {
            ErrorMessage = errorMessage,
            Success = false,
            HasData = true,
            Data = data,
            Exceptions = null,
            IsErrorMessage = isErrorMessage,
        };
    }

    public static ServiceResult<T> CreateErrorUnauthorized(string errorMessage = null)
    {
        return new ServiceResult<T>()
        {
            ErrorMessage = errorMessage,
            Success = false,
            HasData = false,
            Data = default,
            IsErrorMessage = !string.IsNullOrEmpty(errorMessage),
            Exceptions = new[]
            {
                new UnauthorizedOperationException(),
            },
        };
    }

    public static ServiceResult<T> CreateErrorResourceNotFound()
    {
        return new ServiceResult<T>()
        {
            Success = false,
            HasData = false,
            Data = default,
            Exceptions = new[]
            {
                new ResourceNotFoundException(typeof(T)),
            },
        };
    }

    public static ServiceResult<T> CreateBadRequest(string errorMessage = null)
    {
        return new ServiceResult<T>()
        {
            Success = false,
            HasData = false,
            Data = default,
            ErrorMessage = errorMessage,
            IsErrorMessage = !string.IsNullOrEmpty(errorMessage),
            Exceptions = new[]
            {
                new BadRequestException(),
            },
        };
    }

    public static ServiceResult<T> CreateResourceConflict(T data = default)
    {
        return new ServiceResult<T>
        {
            Success = false,
            HasData = false,
            Data = data,
            Exceptions = new[]
            {
                new ResourceConflictException(),
            },
        };
    }
}
