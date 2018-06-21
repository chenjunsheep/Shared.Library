using System.Net;

namespace Shared.Api.Response
{
    public class ApiActionResultNotFound : ApiActionResultBase, IApiActionResult
    {
        public override int StatusCode { get { return (int)HttpStatusCode.NotFound; } }

        public ApiActionResultNotFound() : base(null)
        {
            Message = "not found";
        }

        public ApiActionResultNotFound(string msg) : base(msg)
        {
            Message = msg;
        }

        public override string GetDefaultMessage()
        {
            return "Not Found";
        }
    }
}
