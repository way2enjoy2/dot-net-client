using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Way2enjoy.ResizeOperations;

namespace Way2enjoy.Tests
{
    public class Way2enjoyTests
    {
        const string apiKey = "lolwat";

        internal const string Cat = "Resources/cat.jpg";
        internal const string CompressedCat = "Resources/compressedcat.jpg";
        internal const string ResizedCat = "Resources/resizedcat.jpg";
        internal const string SavedCatPath = "Resources/savedcat.jpg";

        [Fact]
        public void Way2enjoyClientThrowsWhenNoApiKeySupplied()
        {
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient(null));
        }

        [Fact]
        public void Way2enjoyClientThrowsWhenNoValidS3ConfigurationSupplied()
        {
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient(null));
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient(null, null));
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient("apiKey", null));
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient(null, new AmazonS3Configuration("a", "b", "c", "d")));
        }

        [Fact]
        public async Task Compression()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

            var result = await pngx.Compress(Cat);

            Assert.Equal("image/jpeg", result.Input.Type);
            Assert.Equal(400, result.Output.Width);
            Assert.Equal(400, result.Output.Height);
        }

        [Fact]
        public async Task CanBeCalledMultipleTimesWihtoutExploding()
        {
            using (var pngx = new Way2enjoyClient(apiKey))
            {
                Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

                var result = await pngx.Compress(Cat);

                Assert.Equal("image/jpeg", result.Input.Type);
                Assert.Equal(400, result.Output.Width);
                Assert.Equal(400, result.Output.Height);
            }

            using (var pngx = new Way2enjoyClient(apiKey))
            {
                Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

                var result = await pngx.Compress(Cat);

                Assert.Equal("image/jpeg", result.Input.Type);
                Assert.Equal(400, result.Output.Width);
                Assert.Equal(400, result.Output.Height);
            }
        }

        [Fact]
        public async Task CompressionCount()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

            var result = await pngx.Compress(Cat);

            Assert.Equal(99, result.CompressionCount);
        }

        [Fact]
        public async Task CompressionWithBytes()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

            var bytes = File.ReadAllBytes(Cat);
            var result = await pngx.Compress(bytes);

            Assert.Equal("image/jpeg", result.Input.Type);
            Assert.Equal(400, result.Output.Width);
            Assert.Equal(400, result.Output.Height);
        }

        [Fact]
        public async Task CompressionWithStreams()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

            using (var fileStream = File.OpenRead(Cat))
            {
                var result = await pngx.Compress(fileStream);

                Assert.Equal("image/jpeg", result.Input.Type);
                Assert.Equal(400, result.Output.Width);
                Assert.Equal(400, result.Output.Height);
            }
        }

        [Fact]
        public async Task CompressionShouldThrowIfNoPathToFile()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().Compress());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.Compress(string.Empty));
        }

        [Fact]
        public async Task CompressionShouldThrowIfNoNonSuccessStatusCode()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler().CompressAndFail());

            await Assert.ThrowsAsync<Way2enjoyApiException>(async () => await pngx.Compress(Cat));
        }

        [Fact]
        public async Task CompressionAndDownload()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Download());

            var downloadResult = await pngx.Compress(Cat)
                .Download()
                .GetImageByteData();

            Assert.Equal(16646, downloadResult.Length);
        }

        [Fact]
        public async Task CompressionAndDownloadAndGetUnderlyingStream()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Download());

            var downloadResult = await pngx.Compress(Cat)
                .Download()
                .GetImageStreamData();

            Assert.Equal(16646, downloadResult.Length);
        }

        [Fact]
        public async Task CompressionAndDownloadAndWriteToDisk()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Download());
            try
            {
                await pngx.Compress(Cat)
                .Download()
                .SaveImageToDisk(SavedCatPath);
            }
            finally
            {
                //try cleanup any saved file
                File.Delete(SavedCatPath);
            }

        }

        [Fact]
        public async Task ResizingOperationThrows()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.Compress((string)null).Resize(150, 150));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.Compress((string)null).Resize(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.Compress(Cat).Resize(null));

            Task<Responses.Way2enjoyCompressResponse> nullCompressResponse = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await nullCompressResponse.Resize(150, 150));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await pngx.Compress(Cat).Resize(0, 150));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await pngx.Compress(Cat).Resize(150, 0));
        }

        [Fact]
        public async Task DownloadingOperationThrows()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Download());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.Compress((string)null).Download());

            Task<Responses.Way2enjoyCompressResponse> nullCompressResponse = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await nullCompressResponse.Download());
        }

        [Fact]
        public async Task DownloadingOperationThrowsOnNonSuccessStatusCode()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .DownloadAndFail());

            await Assert.ThrowsAsync<Way2enjoyApiException>(async () => await pngx.Compress(Cat).Download());

        }

        [Fact]
        public async Task ResizingOperation()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            var resizedImageByteData = await pngx.Compress(Cat).Resize(150, 150).GetImageByteData();

            Assert.Equal(5970, resizedImageByteData.Length);
        }

        [Fact]
        public async Task ResizingScaleHeightOperation()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            var resizedImageByteData = await pngx.Compress(Cat).Resize(new ScaleHeightResizeOperation(150)).GetImageByteData();

            Assert.Equal(5970, resizedImageByteData.Length);
        }

        [Fact]
        public async Task ResizingScaleWidthOperation()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            var resizedImageByteData = await pngx.Compress(Cat).Resize(new ScaleWidthResizeOperation(150)).GetImageByteData();

            Assert.Equal(5970, resizedImageByteData.Length);
        }

        [Fact]
        public async Task ResizingFitResizeOperation()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            var result = await pngx.Compress(Cat);

            var resizedImageByteData = await pngx.Compress(Cat).Resize(new FitResizeOperation(150, 150)).GetImageByteData();

            Assert.Equal(5970, resizedImageByteData.Length);
        }

        [Fact]
        public async Task ResizingCoverResizeOperation()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            var resizedImageByteData = await pngx.Compress(Cat).Resize(new CoverResizeOperation(150, 150)).GetImageByteData();

            Assert.Equal(5970, resizedImageByteData.Length);
        }

        [Fact]
        public async Task ResizingCoverResizeOperationThrowsWithInvalidParams()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .Resize());

            await Assert.ThrowsAsync<ArgumentException>(async () => await pngx.Compress(Cat).Resize(new CoverResizeOperation(0, 150)));
            await Assert.ThrowsAsync<ArgumentException>(async () => await pngx.Compress(Cat).Resize(new CoverResizeOperation(150, 0)));
        }

        [Fact]
        public void CompressAndStoreToS3ShouldThrowIfNoApiKeyProvided()
        {
            Assert.Throws<ArgumentNullException>(() => new Way2enjoyClient(string.Empty, new AmazonS3Configuration("a", "b", "c", "d")));
        }

        [Fact]
        public async Task CompressAndStoreToS3ShouldThrowIfS3HasNotBeenConfigured()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .S3());

            var result = await pngx.Compress(Cat);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.SaveCompressedImageToAmazonS3(null, "bucket/path.jpg"));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await pngx.SaveCompressedImageToAmazonS3(result, string.Empty));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await pngx.SaveCompressedImageToAmazonS3(result, "bucket/path.jpg"));
        }

        private const string ApiKey = "lolwat";
        private const string ApiAccessKey = "lolwat";

        [Fact]
        public async Task CompressAndStoreToS3()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .S3());

            var result = await pngx.Compress(Cat);

            var sendToAmazon = (await pngx.SaveCompressedImageToAmazonS3(result,
                new AmazonS3Configuration(ApiKey, ApiAccessKey, "way2enjoy-test-bucket", "ap-southeast-2"),
                "path.jpg")).ToString();

            Assert.Equal("https://s3-ap-southeast-2.amazonaws.com/way2enjoy-test-bucket/path.jpg", sendToAmazon);
        }

        [Fact]
        public async Task CompressAndStoreToS3FooBar()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .S3AndFail());

            var result = await pngx.Compress(Cat);

            await Assert.ThrowsAsync<Way2enjoyApiException>(async () =>
                await pngx.SaveCompressedImageToAmazonS3(result,
                new AmazonS3Configuration(ApiKey, ApiAccessKey, "way2enjoy-test-bucket", "ap-southeast-2"), "path"));
        }

        [Fact]
        public async Task CompressAndStoreToS3Throws()
        {
            var pngx = new Way2enjoyClient(apiKey);
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .S3());

            var result = await pngx.Compress(Cat);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await pngx.SaveCompressedImageToAmazonS3(result, null, string.Empty));

            //S3 configuration has not been set
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await pngx.SaveCompressedImageToAmazonS3(result, path: string.Empty));
        }

        [Fact]
        public async Task CompressAndStoreToS3WithOptionsPassedIntoConstructor()
        {
            var pngx = new Way2enjoyClient(apiKey, new AmazonS3Configuration(ApiKey, ApiAccessKey, "way2enjoy-test-bucket", "ap-southeast-2"));
            Way2enjoyClient.HttpClient = new HttpClient(new FakeResponseHandler()
                .Compress()
                .S3());

            var result = await pngx.Compress(Cat);
            var sendToAmazon = (await pngx.SaveCompressedImageToAmazonS3(result, "path.jpg")).ToString();

            Assert.Equal("https://s3-ap-southeast-2.amazonaws.com/way2enjoy-test-bucket/path.jpg", sendToAmazon);
        }

        [Fact]
        public void Way2enjoyExceptionPopulatesCorrectData()
        {
            var StatusCode = 200;
            var StatusReasonPhrase = "status";
            var ErrorTitle = "title";
            var ErrorMessage = "message";
            var e = new Way2enjoyApiException(StatusCode, StatusReasonPhrase, ErrorTitle, "message");

            var msg = $"Api Service returned a non-success status code when attempting an operation on an image: {StatusCode} - {StatusReasonPhrase}. {ErrorTitle}, {ErrorMessage}";

            Assert.Equal(StatusCode, e.StatusCode);
            Assert.Equal(StatusReasonPhrase, e.StatusReasonPhrase);
            Assert.Equal(ErrorTitle, e.ErrorTitle);
            Assert.Equal(msg, e.Message);
        }

        [Fact]
        public void CallingDisposeDoesNotBlowUpTheWorld()
        {
            var pngx = new Way2enjoyClient(apiKey);

            pngx.Dispose();
        }
    }
}
