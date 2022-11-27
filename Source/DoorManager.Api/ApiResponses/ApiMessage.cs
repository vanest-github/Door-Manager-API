namespace DoorManager.Api.ApiResponses;

public class ApiMessage<T>
{
    public string Message { get; set; }

    public T Data { get; set; }
}
