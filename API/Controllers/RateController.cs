using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IRateRepo _rateRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GPTService _gptService;
        public RateController(IRateRepo rateRepo, UserManager<User> userManager, SignInManager<User> signInManager, GPTService gptService)
        {
            _rateRepo = rateRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _gptService = gptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _rateRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByBlogId")]
        public async Task<IActionResult> GetAllCommentByEventId(int eventId)
        {

            try
            {
                return Ok(await _rateRepo.GetAllByBlogId(eventId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {

            try
            {
                return Ok(await _rateRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] RateDto rateDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var isContentAppropriate = await _gptService.IsBlogContentAppropriateAsync(rateDto.Content);
            if (!isContentAppropriate)
            {
                return BadRequest("Nội dung comment không phù hợp. Vui lòng kiểm tra lại.");
            }
            //  string userId = _userManager.GetUserId(HttpContext.User);
            //var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var rate = new Rate()
            {
                Content = rateDto.Content,
                Rating = rateDto.Rating,
                RateDate = DateTime.Now,
                UserId = user.Id,
                BlogId = rateDto.BlogId
            };

            await _rateRepo.Add(rate);

            return Ok(rate);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateComment(int id, [FromBody] RateDto rateDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }




            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var rate = await _rateRepo.GetById(id);

            rate.Content = rateDto.Content;
            rate.Rating = rateDto.Rating;
            //comment.UserId = user.Id;
            rate.RateDate = DateTime.Now;
            rate.BlogId = rateDto.BlogId;







            await _rateRepo.Update(rate);

            return Ok(rate);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var rate = await _rateRepo.GetById(id);
            if (rate == null)
            {

                return NotFound();
            }

            await _rateRepo.Delete(id);
            return Ok();


        }
    }
}
