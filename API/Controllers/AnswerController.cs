using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerRepo _answerRepo;

        public AnswerController(IAnswerRepo answerRepo)
        {
            _answerRepo = answerRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var answers = await _answerRepo.GetAllAsync();
            return Ok(answers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnswerDto answerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var answer = new Answer
            {
                QuestionId = answerDto.QuestionId,
                Text = answerDto.Text,
                IsCorrect = answerDto.IsCorrect
            };

            await _answerRepo.AddAsync(answer);
            return CreatedAtAction(nameof(GetById), new { id = answer.Id }, answer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnswerDto answerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var answer = await _answerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound();

            answer.QuestionId = answerDto.QuestionId;
            answer.Text = answerDto.Text;
            answer.IsCorrect = answerDto.IsCorrect;

            await _answerRepo.UpdateAsync(answer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _answerRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
