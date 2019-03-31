using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Way2enjoy.Responses
{
    public class Way2enjoyResponse
    {
        public HttpResponseMessage HttpResponseMessage { get; }

        private int compressionCount;

        public int CompressionCount => compressionCount;

        protected Way2enjoyResponse(HttpResponseMessage msg)
        {
            if (msg.Headers.TryGetValues("Compression-Count", out IEnumerable<string> compressionCountHeaders))
            {
                int.TryParse(compressionCountHeaders.First(), out compressionCount);
            }
            HttpResponseMessage = msg;
        }
    }
}
