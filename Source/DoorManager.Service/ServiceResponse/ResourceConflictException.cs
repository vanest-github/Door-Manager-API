using System;

namespace DoorManager.Service.ServiceResponse;

public class ResourceConflictException : Exception
{
    public ResourceConflictException()
        : base("An exisitng resource conflicts with this one.")
    {
    }
}
