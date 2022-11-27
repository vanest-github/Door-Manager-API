using System.Net;

namespace DoorManager.Api.ApiResponses;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }

    public object Data { get; set; }
}
