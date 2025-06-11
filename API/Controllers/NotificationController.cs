using API.Hubs;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepo _notificationRepo;
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepo _roleRepo;
        private readonly IBlogRepo _blogRepo;
        private readonly IHubContext<ChatHub> _chatHub;

        public NotificationController(INotificationRepo notificationRepo, UserManager<User> userManager, IRoleRepo roleRepo, IBlogRepo blogRepo, IHubContext<ChatHub> chatHub)
        {
            _notificationRepo = notificationRepo;
            _userManager = userManager;
            _roleRepo = roleRepo;
            _blogRepo = blogRepo;
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                return Ok(await _notificationRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("UserId")]
        public async Task<IActionResult> GetAllNotifyByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            try
            {
                return Ok(await _notificationRepo.GetAllByUserId(user.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppliById(int id)
        {

            try
            {
                return Ok(await _notificationRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LikeBlog")]
        public async Task<IActionResult> LikeBlog(int blogId)
        {
            var blog = await _blogRepo.GetById(blogId);
            if (blog == null)
            {
                return NotFound("Blog không tồn tại");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var blogTitle = blog.Title;
            var senderName = user.UserName; // Đảm bảo không null

            var message = $"{senderName} đã thích blog {blogTitle} của bạn";

            var appli = new Notification()
            {
                Message = message,
                ReceiverNotiId=blog.UserId,

                IsRead = false,
                BlogId=blog.Id,
                CreatedAt = DateTime.Now,
                SenderNotiId=user.Id
            };
            await _notificationRepo.Add(appli);
            await _chatHub.Clients.User(blog.UserId)
               .SendAsync("Like Blog", blogId, message);

            return Ok(appli);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppli(string message)
        {

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var appli = new Notification()
            {
                Message= message,


                IsRead=false,

               CreatedAt = DateTime.Now,
                //UserId = user.Id

            };
            await _notificationRepo.Add(appli);


            return Ok(appli);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAppli(int id, string message)
        {
            
            var appli = await _notificationRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }
            appli.Message= message;
            

            

            await _notificationRepo.Update(appli);

            return Ok(appli);
        }


        

        

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteAppli(int id)
        {


            var artifact = await _notificationRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _notificationRepo.Delete(id);
            return Ok();


        }
    }
}
