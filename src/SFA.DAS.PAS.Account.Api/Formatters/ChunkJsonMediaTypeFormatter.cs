using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Account.Api.Formatters
{
    public class ChunkJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            content.Headers.ContentLength = (content.Headers.ContentLength == 0) ? null : content.Headers.ContentLength;

            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }
    }
}