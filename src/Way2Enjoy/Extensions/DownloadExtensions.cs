using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Way2enjoy.Responses;

namespace Way2enjoy
{

    public static class DownloadExtensions
    {
        private const string JpegType = "image/jpeg";

        /// <summary>
        /// Downloads the result of a Way2enjoy Compression operation
        /// </summary>
        /// <param name="compressResponse"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async static Task<Way2enjoyImageResponse> Download(this Task<Way2enjoyCompressResponse> compressResponse, PreserveMetadata metadata = PreserveMetadata.None)
        {
            if (compressResponse == null)
                throw new ArgumentNullException(nameof(compressResponse));

            var compressResult = await compressResponse;

            return await Download(compressResult, metadata);

        }

        /// <summary>
        /// Downloads the result of a Way2enjoy Compression operation
        /// </summary>
        /// <param name="compressResponse"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async static Task<Way2enjoyImageResponse> Download(Way2enjoyCompressResponse compressResponse, PreserveMetadata metadata = PreserveMetadata.None)
        {
            if (compressResponse == null)
                throw new ArgumentNullException(nameof(compressResponse));

            var msg = new HttpRequestMessage(HttpMethod.Get, compressResponse.Output.Url)
            {
                Content = CreateContent(metadata, compressResponse.Output.Type)
            };

            var response = await compressResponse.HttpClient.SendAsync(msg).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return new Way2enjoyImageResponse(response);
            }

            var errorMsg = JsonConvert.DeserializeObject<ApiErrorResponse>(await response.Content.ReadAsStringAsync());
            throw new Way2enjoyApiException((int)response.StatusCode, response.ReasonPhrase, errorMsg.Error, errorMsg.Message);
        }

        private static HttpContent CreateContent(PreserveMetadata metadata, string type)
        {
            if (metadata == PreserveMetadata.None)
                return null;

            var preserve = new List<string>();

            if (metadata.HasFlag(PreserveMetadata.Copyright))
            {
                preserve.Add("copyright");
            }
            if (metadata.HasFlag(PreserveMetadata.Creation))
            {
                if (type != JpegType)
                    throw new InvalidOperationException($"Creation metadata can only be preserved with type {JpegType}");

                preserve.Add("creation");
            }
            if (metadata.HasFlag(PreserveMetadata.Location))
            {
                if (type != JpegType)
                    throw new InvalidOperationException($"Location metadata can only be preserved with type {JpegType}");

                preserve.Add("location");
            }

            string json = JsonConvert.SerializeObject(new { preserve });

            return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }
    }
}
