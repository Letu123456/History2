using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public BucketsController(IConfiguration config)
        {
           _config = config;
        }

        

        [HttpPost("create")]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.USEast1;
            var _s3Client=new AmazonS3Client(accessKey, secretKey,region);
            //var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");
            await _s3Client.PutBucketAsync(bucketName);
            return Ok($"Bucket {bucketName} created.");
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllBucketAsync()
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.USEast1;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);
            
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBucketAsync(string bucketName)
        {
            var accessKey = _config.GetValue<string>("AWS:AccessKey");
            var secretKey = _config.GetValue<string>("AWS:SecretKey");
            var region = Amazon.RegionEndpoint.USEast1;
            var _s3Client = new AmazonS3Client(accessKey, secretKey, region);
            await _s3Client.DeleteBucketAsync(bucketName);
            return NoContent();
        }
    }
}
