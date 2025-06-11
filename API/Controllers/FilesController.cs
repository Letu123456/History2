using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Business.DTO;
using DataAccess.Service;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _config;
        
        public FilesController(IConfiguration config)
        {
            _config = config;
            
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string? prefix)
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
            return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFilesAsync( string? prefix)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new S3ObjectDto()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
                };
            });

            return Ok(s3Objects);
        }

        [HttpGet("get-by-key")]
        public async Task<IActionResult> GetFileByKeyAsync( string key)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";
            
            var s3Object = await _s3Client.GetObjectAsync(bucketName, key);
            return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFileAsync( string key)
        {

            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.APSoutheast2;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);

            string bucketName = "vnhistory2";
            await _s3Client.DeleteObjectAsync(bucketName, key);
            return NoContent();
        }
    }
}
