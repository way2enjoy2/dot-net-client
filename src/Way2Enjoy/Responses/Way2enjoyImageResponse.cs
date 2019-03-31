using System.Net.Http;

namespace Way2enjoy.Responses
{
    /// <summary>
    /// This is a response which contains actual image data
    /// </summary>
    public class Way2enjoyImageResponse : Way2enjoyResponse
    {
        public Way2enjoyImageResponse(HttpResponseMessage msg) : base(msg)
        {
        }
    }
}
