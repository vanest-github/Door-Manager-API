using System;

namespace DoorManager.Service.ServiceResponse;

public class BadRequestException : Exception
{
    public BadRequestException()
        : base("A wrong value was provided for one of the properties in the request.")
    {
    }
}
