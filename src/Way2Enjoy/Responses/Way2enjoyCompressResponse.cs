using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Way2enjoy.Responses
{
    public class Way2enjoyCompressResponse : Way2enjoyResponse
    {
        public Way2enjoyApiInput Input { get; private set; }
        public Way2enjoyApiOutput Output { get; private set; }
        public Way2enjoyApiResult ApiResult { get; private set; }

        internal readonly HttpClient HttpClient;

        public Way2enjoyCompressResponse(HttpResponseMessage msg, HttpClient httpClient) : base(msg)
        {
            HttpClient = httpClient;

            //this is a cute trick to handle async in a ctor and avoid deadlocks
            ApiResult = Task.Run(() => Deserialize(msg)).GetAwaiter().GetResult();
            Input = ApiResult.Input;
            Output = ApiResult.Output;

        }
        private async Task<Way2enjoyApiResult> Deserialize(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<Way2enjoyApiResult>(await response.Content.ReadAsStringAsync(), Way2enjoyClient.JsonSettings);
        }
    }
}
