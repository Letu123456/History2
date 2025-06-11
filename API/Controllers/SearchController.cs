using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Mvc;
using Azure;
using OpenAI.Threads;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SearchController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchData([FromBody] SearchRequest request)
        {
            try
            {
                string query = request.SearchText;
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "SearchText không được để trống." });

                // Tạo HttpClient và cấp API key
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _config.GetValue<string>("OpenAI:ApiKey2"));

                // Build URL nội bộ tới GPTController
                var gptUrl = $"{Request.Scheme}://{Request.Host}/api/GPT/SearchData";

                // Gửi request lên GPTController/SearchData
                var payload = new { SearchText = query };
                var response = await client.PostAsJsonAsync(gptUrl, payload);

                if (!response.IsSuccessStatusCode)
                {
                    // Log và trả về lỗi chi tiết nếu response không thành công
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = "Lỗi khi gọi GPT SearchData.", details = errorDetails });
                }

                // Đọc và trả về List<Artifact> mà GPTController đã query từ DB
                var artifacts = await response.Content.ReadFromJsonAsync<List<Artifact>>();
                if (artifacts == null || artifacts.Count == 0)
                {
                    return Ok(new { message = "Không có hiện vật phù hợp với yêu cầu của bạn." });
                }

                return Ok(artifacts);
            }
            catch (HttpRequestException httpEx)
            {
                // Log lỗi liên quan đến HTTP request
                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                return StatusCode(500, new { message = "Lỗi khi gửi request đến server.", details = httpEx.Message });
            }
            catch (Exception ex)
            {
                // Log lỗi chung
                Console.WriteLine($"General error: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình xử lý.", details = ex.Message });
            }
        }
    }

    public class SearchRequest
    {
        public string SearchText { get; set; }
    }
}