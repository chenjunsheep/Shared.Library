using System.Net;

namespace Shared.Api.Response
{
    public class ApiActionResultBadRequest : ApiActionResultBase, IApiActionResult
    {
        public override int StatusCode { get { return (int)HttpStatusCode.BadRequest; } }

        public ApiActionResultBadRequest() : base(null)
        {
            Message = "bad request";
        }

        public ApiActionResultBadRequest(string msg) : base(msg)
        {
            Message = msg;
        }

        public override string GetDefaultMessage()
        {
            return "Bad Request";
        }
    }
}