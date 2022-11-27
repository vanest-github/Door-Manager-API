using System;

namespace DoorManager.Service.ServiceResponse;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException()
        : base("The requested resource doesn't exists.")
    {
    }

    public ResourceNotFoundException(Type resourceType)
        : base("The requested resource doesn't exists.")
    {
        this.ResourceType = resourceType;
    }

    public Type ResourceType { get; }
}
