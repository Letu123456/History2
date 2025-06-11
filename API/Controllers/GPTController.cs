using DataAccess.IRepo;
using DataAccess.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OpenAI;
using OpenAI.Chat;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly ILogger<GPTController> logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IMuseumRepo _museumRepo;
        private readonly IArtifactRepo _artifactRepo;
        public GPTController(ILogger<GPTController> logger, IConfiguration config, IMuseumRepo museumRepo,IArtifactRepo artifactRepo)
        {
            this.logger = logger;
            _config = config;
            _httpClient = new HttpClient();
            _museumRepo = museumRepo;
            _artifactRepo = artifactRepo;
        }






        [HttpGet("Chat")]
        public async Task<IActionResult> GetAIInfor()
        {
            var apiKey = _config.GetValue<string>("OpenAI:ApiKey");
            var endpoint = "https://openrouter.ai/api/v1/chat/completions";

            var requestBody = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = "Say hello as I am testing" }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.PostAsync(endpoint, content);
            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }

        
        [HttpPost("ChatGPT")]
        public async Task<IActionResult> GetAIBaseResult(string searchText)
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest("Yêu cầu không được để trống");
            }

            var apiKey = _config.GetValue<string>("OpenAI:ApiKey"); // Update with Gemini API key
                                                                    // Endpoint chính xác cho Gemini API
            var endpoint = "https://openrouter.ai/api/v1/chat/completions"; // Update with the Gemini endpoint

            var requestBody = new
            {
                model = "openai/gpt-3.5-turbo", // Update with the correct model for Gemini
                messages = new[] {
            new { role = "system", content = "Bạn là một trợ lý AI thân thiện và hữu ích." },
            new { role = "user", content = searchText }
        },
                temperature = 0.7, // Thêm các tham số tùy chọn
                max_tokens = 2048
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                using var httpClient = new HttpClient(); // Tạo mới HttpClient để tránh các vấn đề về connection
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.Timeout = TimeSpan.FromSeconds(30); // Thiết lập timeout

                var response = await httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    logger.LogError("Gemini API trả về lỗi: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, $"Lỗi từ API: {errorContent}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    var root = doc.RootElement;

                    // Kiểm tra cấu trúc phản hồi
                    if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                    {
                        logger.LogError("Không tìm thấy 'choices' trong phản hồi: {Response}", responseBody);
                        return BadRequest("Không tìm thấy dữ liệu trả lời từ AI.");
                    }

                    var firstChoice = choices[0];
                    if (!firstChoice.TryGetProperty("message", out var message))
                    {
                        logger.LogError("Không tìm thấy 'message' trong choice: {Response}", responseBody);
                        return BadRequest("Cấu trúc phản hồi không hợp lệ.");
                    }

                    if (!message.TryGetProperty("content", out var contentValue))
                    {
                        logger.LogError("Không tìm thấy 'content' trong message: {Response}", responseBody);
                        return BadRequest("Không có nội dung phản hồi.");
                    }

                    var resultText = contentValue.GetString();
                    return Ok(resultText);
                }
                catch (JsonException jsonEx)
                {
                    logger.LogError(jsonEx, "Lỗi phân tích JSON từ phản hồi: {Response}", responseBody);
                    return StatusCode(500, "Lỗi xử lý phản hồi từ API.");
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogError("Request timeout khi gọi Gemini API");
                return StatusCode(408, "Request timeout");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gọi Gemini API");
                return StatusCode(500, "Lỗi nội bộ: " + ex.Message);
            }
        }



        [HttpPost("MuseumChat")]
        public async Task<IActionResult> GetMuseumChatResult([FromBody] MuseumChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Question))
            {
                return BadRequest("Câu hỏi không được để trống.");
            }

            // Lấy thông tin bảo tàng dựa trên ID
            var museum = await _museumRepo.GetById(request.MuseumId);
            if (museum == null)
            {
                return NotFound($"Không tìm thấy bảo tàng với ID {request.MuseumId}.");
            }

            var apiKey = _config.GetValue<string>("OpenAI:ApiKey");
            var endpoint = "https://openrouter.ai/api/v1/chat/completions";

            // Tạo ngữ cảnh cho chatbot dựa trên thông tin bảo tàng
            string museumContext = museum.GetContext();

            // Tạo prompt cho AI
            var requestBody = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = $"Bạn là chatbot của bảo tàng {museum.Name}. {museumContext} Hãy trả lời câu hỏi một cách tự nhiên, thông minh, luôn đề cập đến tên bảo tàng ({museum.Name}) trong câu trả lời. Nếu câu hỏi không liên quan đến bảo tàng, hãy từ chối trả lời và giải thích rằng bạn chỉ biết về bảo tàng này." },
                new { role = "user", content = request.Question }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize JSON để lấy nội dung trả lời
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            var messageContent = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(messageContent);
        }



        [HttpPost("ChatGPTGeneQuiz")]
        public async Task<IActionResult> GetAIBaseQuizResult(string searchText)
        {
            var apiKey = _config.GetValue<string>("OpenAI:ApiKey");
            var endpoint = "https://openrouter.ai/api/v1/chat/completions";

            var requestBody = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = new[]
                {
        new { role = "system", content = "Bạn là trợ lý chuyên tạo câu hỏi trắc nghiệm lịch sử Việt Nam vui nhộn, dành cho học sinh ở các cấp học khác nhau, phù hợp với cấp độ. " +
        "Tất cả câu hỏi *phải liên quan trực tiếp đến giai đoạn lịch sử* được người tạo nêu ra. chỉ tạo câu hỏi xoay quanh nhân vật, sự kiện, đặc điểm của giai đoạn này, không được vượt ra ngoài phạm vi thời kỳ này. " +
        "\r\nHãy tạo ra 10 câu hỏi trắc nghiệm về lịch sử, mỗi câu hỏi có 4 đáp án và chỉ ra đáp án đúng.  \r\nHãy trả về kết quả dưới dạng JSON, chỉ gồm JSON, không có lời giải thích nào khác." },
        new { role = "user", content = searchText }
    }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize JSON response
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            // Extract the assistant's reply
            var messageContent = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(messageContent);
        }
        [HttpPost("SearchData")]
        public async Task<IActionResult> SearchData([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.SearchText))
                return BadRequest(new { message = "SearchText không được để trống." });

            string query = request.SearchText;

            // 1. Gọi GPT để phân tích từ khoá thành các từ liên quan
            string expansionPrompt = $@"
Từ khoá: ""{query}""

PHÂN TÍCH VÀ TRẢ VỀ:
1. Trước tiên, bạn *PHẢI PHÂN TÍCH* từ khoá truy vấn đầu vào để xác định tất cả các cách gọi tương đương, bao gồm:
   - Nếu là *tên nhân vật*: phân tích các tên thật, biệt danh, tước hiệu, danh xưng dân gian *đã được xác thực*
   - Nếu từ khoá là *một khái niệm chung*, như 'vũ khí', 'trang phục', 'biểu tượng' — thì phải phân tích thành *các loại cụ thể*, ví dụ:
   - 'vũ khí' → mở rộng thành: kiếm, gươm, giáo, mâu, thương, cung tên, nỏ, súng, hoả khí, khiên,...
   - 'trang phục' → mở rộng thành: áo bào, long bào, áo giáp, y phục cổ đại,...
   - 'biểu tượng' → mở rộng thành: trống đồng, ấn ngọc, lạc hồng, cờ tổ quốc, chữ song hỷ,...

KẾT QUẢ TRẢ VỀ:
- Phải là *duy nhất một mảng JSON* theo định dạng:
[
  {{ ""Id"": 1, ""Name"": ""Từ khóa"" }},
  {{ ""Id"": 2, ""Name"": ""Từ đã được phân tích"" }}
]
Phải có bao gồm cả từ khóa trong mảng JSON trả về.


Ví dụ: Từ khóa: Trần Hưng Đạo
Kết Quả trả về:

[
  {{ ""Id"": 1, ""Name"": ""Trần Hưng Đạo"" }},
  {{ ""Id"": 2, ""Name"": ""Trần Quốc Tuấn"" }},
  {{ ""Id"": 3, ""Name"": ""Hưng Đạo Vương"" }},
  {{ ""Id"": 4, ""Name"": ""Quốc công Tiết chế"" }}
]

Ví dụ: Từ Khóa: Kiếm
Kết Quả trả về:
[
  {{ ""Id"": 1, ""Name"": ""kiếm"" }},
  {{ ""Id"": 2, ""Name"": ""gươm"" }},
  {{ ""Id"": 3, ""Name"": ""bảo kiếm"" }},
  {{ ""Id"": 4, ""Name"": ""vũ khí"" }}
]

Ví dụ: Từ Khóa: Áo Bào
Kết Quả trả về:
[
  {{ ""Id"": 1, ""Name"": ""Áo Bào"" }},
  {{ ""Id"": 2, ""Name"": ""Long Bào"" }},
  {{ ""Id"": 3, ""Name"": ""Áo Choàng Vua"" }}
]

*Tuyệt đối KHÔNG thêm bất kỳ văn bản, tiêu đề, ghi chú hay ký hiệu nào khác.*
Không viết lời giải thích.
Không bao bọc trong ```json hoặc markdown.

Nếu không có từ nào liên quan, trả về mảng rỗng: []";

            var gptExpandBody = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = @"Bạn là một chuyên gia AI về lịch sử Việt Nam. 
Bạn có kiến thức sâu rộng về các triều đại, nhân vật lịch sử, sự kiện lớn, biểu tượng văn hoá và hiện vật truyền thống.

KIẾN THỨC CHUYÊN MÔN:
Hiểu rõ các nhân vật lịch sử Việt Nam, bao gồm tên thật, biệt hiệu, tước vị, danh xưng dân gian.
Nhận biết các loại hiện vật: vũ khí, trang phục, biểu tượng lịch sử.
Nắm rõ từ đồng nghĩa, từ cổ, cách diễn đạt khác nhau (vd: kiếm = gươm, vua = hoàng đế, áo bào = long bào).


YÊU CẦU BẮT BUỘC:
Chỉ trả về mảng JSON chuẩn (không chú thích, không markdown).
Không viết thêm lời giải thích."

        },
                    new { role = "user", content = expansionPrompt }
                }
            };

            var apiKey = _config.GetValue<string>("OpenAI:ApiKey");
            var endpoint = "https://openrouter.ai/api/v1/chat/completions";
            var expandContent = new StringContent(JsonSerializer.Serialize(gptExpandBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            HttpResponseMessage expandResponse;
            try
            {
                expandResponse = await _httpClient.PostAsync(endpoint, expandContent);
            }
            catch (HttpRequestException httpEx) when (httpEx.InnerException is IOException ioEx &&
                                                       ioEx.InnerException is SocketException sockEx &&
                                                       sockEx.SocketErrorCode == SocketError.ConnectionReset)
            {
                return StatusCode(503, new { message = "Kết nối GPT tạm thời gián đoạn, vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi gửi request đến GPT.", error = ex.Message });
            }

            /*var expandResponse = await _httpClient.PostAsync(endpoint, expandContent);*/
            if (!expandResponse.IsSuccessStatusCode)
                return StatusCode((int)expandResponse.StatusCode, new { message = "Lỗi khi gọi GPT SearchData." });

            var expandRaw = await expandResponse.Content.ReadAsStringAsync();
            List<TermDto> relatedTerms;

            try
            {
                using var expandDoc = JsonDocument.Parse(expandRaw);
                var contentText = expandDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                var cleanJson = contentText
                    .Trim()
                    .Trim('`')
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                relatedTerms = JsonSerializer.Deserialize<List<TermDto>>(cleanJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "GPT không trả về mảng từ hợp lệ",
                    details = expandRaw,
                    error = ex.Message
                });
            }

            var names = relatedTerms.Select(x => x.Name).ToList();
            var allArtifacts = await _artifactRepo.GetAll();

            var matchedArtifacts = allArtifacts.Where(a =>
                names.Any(term =>
                    (a.ArtifactName?.IndexOf(term, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                    (a.Description?.IndexOf(term, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0
                )).ToList();

            return Ok(matchedArtifacts);
        }
    }

    // DTO để nhận dữ liệu từ request
    public class MuseumChatRequest
    {
        public int MuseumId { get; set; }
        public string Question { get; set; }
    }

    public class TermDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}



