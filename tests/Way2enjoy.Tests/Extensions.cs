﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;

namespace Way2enjoy.Tests
{
    static class Extensions
    {
        public static FakeResponseHandler Compress(this FakeResponseHandler fakeResponse)
        {
            var content = new Way2enjoyApiResult()
            {
                Input = new Way2enjoyApiInput
                {
                    Size = 18031,
                    Type = "image/jpeg"
                },
                Output = new Way2enjoyApiOutput
                {
                    Width = 400,
                    Height = 400,
                    Size = 16646,
                    Type = "image/jpeg",
                    Ratio = 0.9232f,
                    Url = "https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"
                }
            };
            var compressResponseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Content = new StringContent(JsonConvert.SerializeObject(content)),
            };
            compressResponseMessage.Headers.Location = new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php");
            compressResponseMessage.Headers.Add("Compression-Count", "99");

            fakeResponse.AddFakePostResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), compressResponseMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler CompressAndFail(this FakeResponseHandler fakeResponse)
        {
            var errorApiObject = new Way2enjoyApiException(400, "reason", "title", "message");

            var compressResponseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Content = new StringContent(JsonConvert.SerializeObject(errorApiObject))
            };
            fakeResponse.AddFakePostResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), compressResponseMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler Download(this FakeResponseHandler fakeResponse)
        {
            var compressedCatStream = File.OpenRead(Way2enjoyTests.CompressedCat);
            var outputResponseMessage = new HttpResponseMessage
            {
                Content = new StreamContent(compressedCatStream),
                StatusCode = System.Net.HttpStatusCode.OK
            };

            fakeResponse.AddFakeGetResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), outputResponseMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler DownloadAndFail(this FakeResponseHandler fakeResponse)
        {
            var outputResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new Responses.ApiErrorResponse { Error = "Stuff's on fire yo!", Message = "This is the error message"  })),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };

            fakeResponse.AddFakeGetResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), outputResponseMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler Resize(this FakeResponseHandler fakeResponse)
        {
            var resizedCatStream = File.OpenRead(Way2enjoyTests.ResizedCat);
            var resizeMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StreamContent(resizedCatStream)
            };
            resizeMessage.Headers.Add("Image-Width", "150");
            resizeMessage.Headers.Add("Image-Height", "150");

            fakeResponse.AddFakePostResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), resizeMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler S3(this FakeResponseHandler fakeResponse)
        {
            var amazonMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            amazonMessage.Headers.Add("Location", "https://s3-ap-southeast-2.amazonaws.com/way2enjoy-test-bucket/path.jpg");

            fakeResponse.AddFakePostResponse(new Uri("https://api.tinify.com/output"), amazonMessage);
            return fakeResponse;
        }

        public static FakeResponseHandler S3AndFail(this FakeResponseHandler fakeResponse)
        {
            var amazonMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new Responses.ApiErrorResponse { Error = "Stuff's on fire yo!", Message = "This is the error message" })),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            //amazonMessage.Headers.Add("Location", "https://s3-ap-southeast-2.amazonaws.com/way2enjoy-test-bucket/path.jpg");

            fakeResponse.AddFakePostResponse(new Uri("https://way2enjoy.com/modules/compress-png/way2enjoy-cli2.php"), amazonMessage);
            return fakeResponse;
        }
    }
}
