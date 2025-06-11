using Business;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepo _quizRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IAnswerRepo _answerRepo;
        private readonly AppDbContext _context;
        private readonly FilesService _filesService;

        public QuizController(IQuizRepo quizRepo, IQuestionRepo questionRepo, IAnswerRepo answerRepo, AppDbContext context, FilesService filesService)
        {
            _quizRepo = quizRepo;
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _context = context;
            _filesService = filesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _quizRepo.GetAllAsync();
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizRepo.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDto quizDto)
        {
            if (quizDto == null)
            {
                return BadRequest(new { message = "Invalid quiz data!" });
            }

            var categoryExists = await _context.categoryHistoricals
                                               .AnyAsync(c => c.Id == quizDto.CategoryHistoricalId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "CategoryHistorical does not exist!" });
            }

            

            var quiz = new Quiz
            {
                Title = quizDto.Title,
                Description = quizDto.Description,
                TimeLimit = quizDto.TimeLimit,
                Level= quizDto.Level,
                IsActive = quizDto.IsActive,
                CreatedAt = DateTime.Now,
                CategoryHistoricalId = quizDto.CategoryHistoricalId
                
            };

            await _quizRepo.AddAsync(quiz);
            return Ok(new { message = "Quiz created successfully!", quiz });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] QuizDto quizDto)
        {
            if (quizDto == null)
            {
                return BadRequest(new { message = "Invalid quiz data!" });
            }

            var existingQuiz = await _quizRepo.GetByIdAsync(id);
            if (existingQuiz == null)
            {
                return NotFound(new { message = "Quiz not found!" });
            }

            var categoryExists = await _context.categoryHistoricals
                                               .AnyAsync(c => c.Id == quizDto.CategoryHistoricalId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "CategoryHistorical does not exist!" });
            }

            existingQuiz.Title = quizDto.Title;
            existingQuiz.Description = quizDto.Description;
            existingQuiz.TimeLimit = quizDto.TimeLimit;
            existingQuiz.Level= quizDto.Level;
            existingQuiz.IsActive = quizDto.IsActive;
            existingQuiz.CreatedAt = DateTime.Now;
            existingQuiz.CategoryHistoricalId = quizDto.CategoryHistoricalId;

            await _quizRepo.UpdateAsync(existingQuiz);
            return Ok(new { message = "Quiz updated successfully!", quiz = existingQuiz });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.quiz
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();


            foreach (var question in quiz.Questions)
            {
                _context.answers.RemoveRange(question.Answers);
            }

            _context.question.RemoveRange(quiz.Questions);

            _context.quiz.Remove(quiz);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{quizId}/submit")]
        public async Task<IActionResult> SubmitQuiz(int quizId, [FromBody] List<UserAnswerDto> userAnswers)
        {
            if (userAnswers == null || !userAnswers.Any())
            {
                return BadRequest("Danh sách câu trả lời không hợp lệ!");
            }

            var questions = await _context.question
        .Where(q => q.QuizId == quizId)
        .Include(q => q.Answers)
        .ToListAsync();
            if (questions == null || !questions.Any())
                return NotFound("Quiz không tồn tại!");

            int totalPoints = 0;
            int correctAnswers = 0;

            foreach (var userAnswer in userAnswers)
            {
                var question = questions.FirstOrDefault(q => q.Id == userAnswer.QuestionId);
                if (question != null)
                {
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

                    if (correctAnswer != null && correctAnswer.Id == userAnswer.SelectedAnswerId)
                    {
                        totalPoints += question.Points;
                        correctAnswers++;
                    }
                }
            }

            var result = new
            {
                QuizId = quizId,
                TotalQuestions = questions.Count(),
                CorrectAnswers = correctAnswers,
                TotalPoints = totalPoints
            };

            return Ok(result);
        }
        [HttpPost("geneQuiz")]
        public async Task<IActionResult> GeneQuiz([FromBody] QuizDto quizDto)
        {
            if (quizDto == null || string.IsNullOrEmpty(quizDto.Description))
            {
                return BadRequest(new { message = "Thông tin quiz không hợp lệ!" });
            }

            var category = await _context.Set<CategoryHistorical>()
                                         .FirstOrDefaultAsync(c => c.Id == quizDto.CategoryHistoricalId);

            Console.WriteLine(category.Name);
            if (category == null)
            {
                return BadRequest(new { message = "Danh mục lịch sử không tồn tại!" });
            }

            var newQuiz = new Quiz
            {
                Title = quizDto.Title,
                Description = quizDto.Description,
                Level = quizDto.Level,
                TimeLimit = quizDto.TimeLimit,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CategoryHistoricalId = quizDto.CategoryHistoricalId
            };

            await _quizRepo.AddAsync(newQuiz);

            string prompt = $@"
Bạn hãy đóng vai một người biên soạn câu hỏi trắc nghiệm *vui nhộn* và *hóm hỉnh*, chuyên về lịch sử, để tạo ra 10 câu hỏi *đố vui lịch sử* phù hợp với cấp độ {newQuiz.Level}: (1 = Tiểu học, 2 = Trung Học Cơ Sở (THCS), 3 = Trung học Phổ Thông (THPT))

*Yêu cầu bắt buộc*:
- *Liên quan trực tiếp đến giai đoạn lịch sử*: ""{category.Name}"" chỉ tạo câu hỏi xoay quanh nhân vật, sự kiện, đặc điểm của giai đoạn này, không được vượt ra ngoài phạm vi thời kỳ này.
- *Bám sát mô tả*: ""{newQuiz.Description}""
- Câu hỏi phải mang tính chất *vui vẻ, dí dỏm, gợi nhớ*, có thể chơi như một trò đố vui, nhưng vẫn đảm bảo đúng kiến thức lịch sử.
Mỗi câu có 4 đáp án (A, B, C, D).
Phải chỉ rõ đáp án đúng bằng chỉ số (0–3).
Câu hỏi nên dễ đọc, hấp dẫn học sinh đúng cấp độ.

- *Không bao gồm lời giải thích hay văn bản thừa* ngoài kết quả JSON.

Cấu trúc kết quả:
[
  {{
    ""question"": ""<Nội dung câu hỏi>"",
    ""options"": [""<Đáp án A>"", ""<Đáp án B>"", ""<Đáp án C>"", ""<Đáp án D>""],
    ""correctIndex"": <số nguyên từ 0 đến 3>
  }},
  ...
]

Ví dụ, kết quả đúng phải giống như:  
[
  {{
    ""question"": ""Ai là người có biệt danh 'Bà Triệu cưỡi voi đánh giặc' khiến kẻ thù 'hồn xiêu phách lạc'?"",
    ""options"": [""Bà Huyện Thanh Quan"", ""Bà Triệu"", ""Bà Nguyễn Thị Định"", ""Bà Tú""],
    ""correctIndex"": 1
  }},
  {{
    ""question"": ""Vị vua nào được mệnh danh là 'chạy nhiều hơn đánh' nhưng vẫn giành được ngai vàng?"",
    ""options"": [""Lê Lợi"", ""Gia Long"", ""Ngô Quyền"", ""Trần Nhân Tông""],
    ""correctIndex"": 1
  }}
]
Hãy trả về *chỉ toàn bộ JSON* bắt đầu từ dấu [ và kết thúc bằng dấu ].
]";
            var requestScheme = Request.Scheme;
            var requestHost = Request.Host.ToUriComponent();
            var gptUrl = $"{requestScheme}://{requestHost}/api/GPT/ChatGPTGeneQuiz?searchText={Uri.EscapeDataString(prompt)}";

            List<GPTQuestion> gptQuestions;
            using (var client = new HttpClient())
            {
                var httpResponse = await client.PostAsync(gptUrl, null);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)httpResponse.StatusCode, new { message = "Gặp lỗi khi gọi GPTController" });
                }
                var resultContent = await httpResponse.Content.ReadAsStringAsync();

                try
                {
                    gptQuestions = JsonSerializer.Deserialize<List<GPTQuestion>>(resultContent);

                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Không thể parse kết quả GPT: " + ex.Message });
                }
            }

            foreach (var gptQ in gptQuestions)
            {
                var newQuestion = new Question
                {
                    Text = gptQ.question,
                    QuizId = newQuiz.Id,
                    Points = 1,
                    Image = null
                };
                await _questionRepo.AddAsync(newQuestion);

                for (int i = 0; i < gptQ.options.Count; i++)
                {
                    var newAnswer = new Answer
                    {
                        Text = gptQ.options[i],
                        QuestionId = newQuestion.Id,
                        IsCorrect = (i == gptQ.correctIndex)
                    };
                    await _answerRepo.AddAsync(newAnswer);
                }
            }

            return Ok(new { message = "Tạo quiz cùng câu hỏi tự động thành công!", quizId = newQuiz.Id, totalQuestions = gptQuestions.Count });
        }
        [HttpGet("filter")]
        public async Task<IActionResult> GetQuizByLevelAndCategory([FromQuery] int level, [FromQuery] int categoryHistoricalId)
        {
            var quizzes = await _context.quiz
                .Where(q => q.Level == level && q.CategoryHistoricalId == categoryHistoricalId)
                .ToListAsync();

            if (quizzes == null || !quizzes.Any())
            {
                return NotFound(new { message = "Không tìm thấy quiz phù hợp với yêu cầu." });
            }

            return Ok(quizzes);
        }





    }

    public class GPTQuestion
    {
        public string question { get; set; }
        public List<string> options { get; set; }
        public int correctIndex { get; set; }
    }

}

