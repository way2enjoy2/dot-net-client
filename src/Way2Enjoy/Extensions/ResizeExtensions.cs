using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Way2enjoy.ResizeOperations;
using Way2enjoy.Responses;

namespace Way2enjoy
{
    public static class ResizeExtensions
    {

        /// <summary>
        /// Uses the Way2enjoy API to create a resized version of your uploaded image.
        /// </summary>
        /// <param name="result">This is the previous result of running a compression <see cref="Way2enjoyClient.Compress(string)"/></param>
        /// <param name="resizeOperation">Supply a strongly typed Resize Operation. See <typeparamref name="CoverResizeOperation"/>, <typeparamref name="FitResizeOperation"/>, <typeparamref name="ScaleHeightResizeOperation"/>, <typeparamref name="ScaleWidthResizeOperation"/></param>
        /// <returns></returns>
        public async static Task<Way2enjoyResizeResponse> Resize(this Task<Way2enjoyCompressResponse> result, ResizeOperation resizeOperation)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (resizeOperation == null)
                throw new ArgumentNullException(nameof(resizeOperation));

            var compressResponse = await result;

            var requestBody = JsonConvert.SerializeObject(new { resize = resizeOperation }, Way2enjoyClient.JsonSettings);

            var msg = new HttpRequestMessage(HttpMethod.Post, compressResponse.Output.Url)
            {
                Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json")
            };
            var response = await compressResponse.HttpClient.SendAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                return new Way2enjoyResizeResponse(response);
            }

            var errorMsg = JsonConvert.DeserializeObject<ApiErrorResponse>(await response.Content.ReadAsStringAsync());
            throw new Way2enjoyApiException((int)response.StatusCode, response.ReasonPhrase, errorMsg.Error, errorMsg.Message);
        }

        /// <summary>
        /// Uses the Way2enjoy API to create a resized version of your uploaded image.
        /// </summary>
        /// <param name="result">This is the previous result of running a compression <see cref="Way2enjoyClient.Compress(string)"/></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="resizeType"></param>
        /// <returns></returns>
        public async static Task<Way2enjoyResizeResponse> Resize(this Task<Way2enjoyCompressResponse> result, int width, int height, ResizeType resizeType = ResizeType.Fit)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (width == 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height == 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            var resizeOp = new ResizeOperation(resizeType, width, height);

            return await Resize(result, resizeOp);
        }


    }
}
