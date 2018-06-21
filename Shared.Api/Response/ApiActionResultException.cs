using System;
using System.Net;

namespace Shared.Api.Response
{
    public class ApiActionResultException : ApiActionResultBase, IApiActionResult
    {
        public override int StatusCode { get { return (int)HttpStatusCode.InternalServerError; } }

        public ApiActionResultException() : base(null)
        {
        }

        public ApiActionResultException(Exception ex) : base(ex?.ToString())
        {
            Message = ex?.Message;
        }

        public override string GetDefaultMessage()
        {
            return "Server Interanl Error";
        }
    }
}