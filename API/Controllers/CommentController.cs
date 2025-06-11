using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Service;

namespace API.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo _comment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GPTService _gptService;
        public CommentController(ICommentRepo comment,UserManager<User> userManager, SignInManager<User> signInManager, GPTService gptService)
        {
            _comment = comment;
            _userManager = userManager;
            _signInManager = signInManager;
            _gptService = gptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _comment.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByEventId")]
        public async Task<IActionResult> GetAllCommentByEventId(int eventId)
        {

            try
            {
                return Ok(await _comment.GetAllByEventId(eventId));
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
                return Ok(await _comment.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var isContentAppropriate = await _gptService.IsBlogContentAppropriateAsync(commentDto.Content);
            if (!isContentAppropriate)
            {
                return BadRequest("Nội dung bình luận không phù hợp. Vui lòng kiểm tra lại.");
            }

            //  string userId = _userManager.GetUserId(HttpContext.User);
            //var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            User user = await _userManager.FindByIdAsync(userId);
            


            var comment = new Comment()
            {
                Content = commentDto.Content,
                Rating=commentDto.Rating,
                CommentDate=DateTime.Now,
                UserId= user.Id,
                EventId=commentDto.EventId
            };

            await _comment.Add(comment);

            return Ok(comment);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var comment = await _comment.GetById(id);
            
            comment.Content = commentDto.Content;
            comment.Rating = commentDto.Rating;
            //comment.UserId = user.Id;
            comment.CommentDate = DateTime.Now;
            comment.EventId = commentDto.EventId;







            await _comment.Update(comment);

            return Ok(comment);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _comment.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _comment.Delete(id);
            return Ok();


        }
    }
}
