using System.Net.Http;

namespace Way2enjoy.Responses
{
    public class Way2enjoyResizeResponse : Way2enjoyImageResponse
    {
        public Way2enjoyResizeResponse(HttpResponseMessage msg) : base(msg)
        {

        }
    }
}
