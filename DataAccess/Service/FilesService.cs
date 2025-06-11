using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Service
{
    public class FilesService
    {
        private readonly IConfiguration _config;
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;
        public FilesService(IConfiguration config) 
        { 
        _config = config;
            var accessKey = config.GetValue<string>("AWS:AccessKey");
            var secretKey = config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;

            _s3Client = new AmazonS3Client(accessKey, secretKey, region);
            _bucketName = "vnhistory2";
        }
        public async Task<string> UploadFileAsync(IFormFile file, string? prefix)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return $"https://{bucketName}.s3.{region.SystemName}.amazonaws.com/{request.Key}";

        }

        public async Task<string> GetFileByKeyAsync(string key)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";

            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return s3Object.Key;
           
        }

        public async Task<bool> DeleteFileByUrlAsync(string url)
        {
            try
            {
                // List all objects in the bucket
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                // Match the URL to the object's key
                var uri = new Uri(url);
                var matchingKey = listResponse.S3Objects
                    .FirstOrDefault(o => url.Contains(o.Key))?.Key;

                if (matchingKey == null)
                {
                    return false; // File not found
                }

                // Delete the matched object
                await _s3Client.DeleteObjectAsync(_bucketName, matchingKey);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"S3 error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"Internal error: {ex.Message}");
                throw;
            }
        }


        public async Task<Stream> GetImageByUrlAsync(string url)
        {
            try
            {
                // List all objects in the bucket
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                // Match the URL to the object's key
                var uri = new Uri(url);
                var matchingKey = listResponse.S3Objects
                    .FirstOrDefault(o => url.Contains(o.Key))?.Key;

                if (matchingKey == null)
                {
                    return null; // File not found
                }

                // Get the object from S3
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = matchingKey
                };

                var getObjectResponse = await _s3Client.GetObjectAsync(getObjectRequest);

                // Return the object's content as a stream
                return getObjectResponse.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"S3 error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"Internal error: {ex.Message}");
                throw;
            }
        }

        public async void DeleteFileAsync(string key)
        {

            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";
            await _s3Client.DeleteObjectAsync(bucketName, key);
           
        }
    }
}
