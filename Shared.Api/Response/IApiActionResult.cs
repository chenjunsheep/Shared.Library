namespace Shared.Api.Response
{
    public interface IApiActionResult
    {
        int StatusCode { get; }
        object Data { get; set; }
        string Message { get; set; }
    }
}
