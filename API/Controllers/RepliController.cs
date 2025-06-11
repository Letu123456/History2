using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepliController : ControllerBase
    {
        private readonly IRepliCommentRepo _repliComment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public RepliController(IRepliCommentRepo repliComment, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _repliComment = repliComment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRepliComment()
        {

            try
            {
                return Ok(await _repliComment.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepliCommentById(int id)
        {

            try
            {
                return Ok(await _repliComment.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRepliComment([FromBody] RepliCommentDto repliCommentDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            //  string userId = _userManager.GetUserId(HttpContext.User);
            //var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var repliComment = new RepliComment()
            {
                Content = repliCommentDto.Content,
                Rating = repliCommentDto.Rating,
                CommentDate = DateTime.Now,
                UserId = user.Id,
                EventId=repliCommentDto.EventId,
                CommentId = repliCommentDto.CommentId
            };

            await _repliComment.Add(repliComment);

            return Ok(repliComment);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateRepliComment(int id, [FromBody] RepliCommentDto repliCommentDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }




            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var repliComment = await _repliComment.GetById(id);

            repliComment.Content = repliCommentDto.Content;
            repliComment.Rating = repliCommentDto.Rating;
            //repliComment.UserId = user.Id;
            repliComment.CommentDate = DateTime.Now;
            repliComment.EventId = repliCommentDto.EventId;
            repliComment.CommentId= repliCommentDto.CommentId;







            await _repliComment.Update(repliComment);

            return Ok(repliComment);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _repliComment.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _repliComment.Delete(id);
            return Ok();


        }
    }
}
