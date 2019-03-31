﻿using System.IO;
using System.Threading.Tasks;
using Way2enjoy.Responses;

namespace Way2enjoy
{
    public static class ImageDataExtensions
    {
        /// <summary>
        /// Get the image data as a byte array
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <returns>Byte array of the image data</returns>
        public async static Task<byte[]> GetImageByteData<T>(this Task<T> result) where T: Way2enjoyImageResponse
        {
            var imageResponse = await result;
            return await GetImageByteData(imageResponse);
        }

        /// <summary>
        /// Get the image data as a byte array
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <returns>Byte array of the image data</returns>
        public async static Task<byte[]> GetImageByteData(this Way2enjoyImageResponse result)
        {
            return await result.HttpResponseMessage.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Gets the image data as a stream
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <returns>Stream of compressed image data</returns>
        public async static Task<Stream> GetImageStreamData<T>(this Task<T> result) where T : Way2enjoyImageResponse
        {
            var imageResponse = await result;
            return await GetImageStreamData(imageResponse);
        }

        /// <summary>
        /// Gets the image data as a stream
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <returns>Stream of compressed image data</returns>
        public async static Task<Stream> GetImageStreamData(Way2enjoyImageResponse result)
        {
            return await result.HttpResponseMessage.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Writes the image to disk
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <param name="filePath">The path to store the file</param>
        /// <returns></returns>
        public async static Task SaveImageToDisk<T>(this Task<T> result, string filePath) where T : Way2enjoyImageResponse
        {
            var response = await result;
            await SaveImageToDisk(response, filePath);
        }

        /// <summary>
        /// Writes the image to disk
        /// </summary>
        /// <param name="result">The result from compress</param>
        /// <param name="filePath">The path to store the file</param>
        /// <returns></returns>
        public async static Task SaveImageToDisk(this Way2enjoyImageResponse result, string filePath)
        {
            var byteData = await result.GetImageByteData();
            File.WriteAllBytes(filePath, byteData);
        }


    }
}
