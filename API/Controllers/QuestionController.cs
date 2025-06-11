using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepo _questionRepo;
        private readonly FilesService _filesService;

        public QuestionController(IQuestionRepo questionRepo, FilesService filesService)
        {
            _questionRepo = questionRepo;
            _filesService = filesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _questionRepo.GetAllAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] QuestionDto questionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (questionDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(questionDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var question = new Question
            {
                QuizId = questionDto.QuizId,
                Text = questionDto.Text,
                Points = questionDto.Points,
                Image=uploadedImageUrl
            };

            await _questionRepo.AddAsync(question);
            return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] QuestionDto questionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (questionDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(questionDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }


            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound();

            question.QuizId = questionDto.QuizId;
            question.Text = questionDto.Text;
            question.Points = questionDto.Points;
            question.Image = uploadedImageUrl;

            await _questionRepo.UpdateAsync(question);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
