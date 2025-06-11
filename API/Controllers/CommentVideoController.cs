using Business.DTO;
using Business.Migrations;
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
    public class CommentVideoController : ControllerBase
    {
        private readonly ICommentVideo _commentVideo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GPTService _gptService;
        public CommentVideoController(ICommentVideo commentVideo, UserManager<User> userManager, SignInManager<User> signInManager, GPTService gptService)
        {
            _commentVideo = commentVideo;
            _userManager = userManager;
            _signInManager = signInManager;
            _gptService = gptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _commentVideo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByVideoId")]
        public async Task<IActionResult> GetAllCommentByEventId(int videoId)
        {

            try
            {
                return Ok(await _commentVideo.GetAllByVideoId(videoId));
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
                return Ok(await _commentVideo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(string content,int videoId)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var isContentAppropriate = await _gptService.IsBlogContentAppropriateAsync(content);
            if (!isContentAppropriate)
            {
                return BadRequest("Nội dung bình luận không phù hợp. Vui lòng kiểm tra lại.");
            }

            //  string userId = _userManager.GetUserId(HttpContext.User);
            //var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var comment = new CommentVideo()
            {
                Content = content,
                
                CommentDate = DateTime.Now,
                UserId = user.Id,
                VideoId = videoId
            };

            await _commentVideo.Add(comment);

            return Ok(comment);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateComment(int id, string content,int videoId)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }




            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var comment = await _commentVideo.GetById(id);

            comment.Content = content;
            
            comment.CommentDate = DateTime.Now;
            comment.VideoId = videoId;







            await _commentVideo.Update(comment);

            return Ok(comment);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _commentVideo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _commentVideo.Delete(id);
            return Ok();


        }
    }
}
