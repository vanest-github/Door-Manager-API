using System;

namespace DoorManager.Service.ServiceResponse;

public class UnauthorizedOperationException : Exception
{
    public UnauthorizedOperationException()
        : base("The client is not authorized to perform this operation.")
    {
    }
}
