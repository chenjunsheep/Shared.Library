using System.Net;

namespace Shared.Api.Response
{
    public class ApiActionResultOK : ApiActionResultBase, IApiActionResult
    {
        public override int StatusCode { get { return (int)HttpStatusCode.OK; } }

        public ApiActionResultOK() : base(null)
        {
        }

        public ApiActionResultOK(object data) : base(data)
        {
        }

        public override string GetDefaultMessage()
        {
            return "Sucess";
        }
    }
}