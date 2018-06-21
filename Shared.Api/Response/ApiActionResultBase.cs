namespace Shared.Api.Response
{
    public abstract class ApiActionResultBase : IApiActionResult
    {
        public abstract int StatusCode { get; }
        public virtual object Data { get; set; }
        public virtual string Message { get; set; }

        public ApiActionResultBase(object data)
        {
            Data = data;
        }

        public abstract string GetDefaultMessage();
    }
}