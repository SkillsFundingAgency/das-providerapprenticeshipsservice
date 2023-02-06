using System.Net;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions
{
    public class HttpException : Exception
    {
        private readonly int httpStatusCode;
        public int StatusCode { get { return this.httpStatusCode; } }

        public HttpException(int httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }

        public HttpException(int httpStatusCode, string message) : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }
    }
}
