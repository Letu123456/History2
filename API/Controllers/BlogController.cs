using API.Hubs;
using Business.DTO;
using Business.Model;
using Business.Options;
using CloudinaryDotNet;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepo _blogRepo;
        private readonly FilesService filesService;
        private readonly UserManager<User> _userManager;
        private readonly INotificationRepo _notificationRepo;
        private readonly GPTService _gptService;
        private readonly IHubContext<ChatHub> _chatHub;
        public BlogController(IBlogRepo blogRepo, FilesService filesService,UserManager<User> userManager, INotificationRepo notificationRepo,GPTService gptService, IHubContext<ChatHub> chatHub)
        {
            _blogRepo = blogRepo;
            this.filesService = filesService;
            _userManager = userManager;
            _notificationRepo = notificationRepo;
            _gptService = gptService;
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvent()
        {

            try
            {
                return Ok(await _blogRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {

            try
            {
                return Ok(await _blogRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getByHashtag")]
        public async Task<IActionResult> GetAllBlogByHashtag(string hastag)
        {

            if (string.IsNullOrWhiteSpace(hastag))
            {
                return BadRequest("Hashtag cannot be empty.");
            }

            try
            {
                return Ok(await _blogRepo.GetAllByHashtag(hastag));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getAllByUserLogged")]
        public async Task<IActionResult> GetAllBlogByUserLogged()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);


            try
            {
                return Ok(await _blogRepo.GetAllByUserId(user.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getAllByUserId")]
        public async Task<IActionResult> GetAllBlogByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("Hashtag cannot be empty.");
            }


            try
            {
                return Ok(await _blogRepo.GetAllByUserId(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] BlogDto blogDto, [FromForm] IEnumerable<string> hastagDto)
        {
            if (hastagDto == null || !hastagDto.Any())
                return BadRequest("At least one hastag is required.");
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var isContentAppropriate = await _gptService.IsBlogContentAppropriateAsync(blogDto.Content);
            if (!isContentAppropriate)
            {
                return BadRequest("Nội dung blog không phù hợp. Vui lòng kiểm tra lại.");
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (blogDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(blogDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var blog = new Blog()
            {
                Title = blogDto.Title,
                Content=blogDto.Content,

                Image = uploadedImageUrl,
                CreatedDate= DateTime.Now,
                UserId=user.Id,
                IsAccept=0,
                CategoryBlogId=blogDto.CategoryBlogId,

            };
            blog.HastagOfBlog = new List<HastagOfBlog>();
            if (hastagDto != null)
            {

                foreach (var hastag in hastagDto)
                {


                    try
                    {
                        blog.HastagOfBlog.Add(new HastagOfBlog { Hashtag = hastag });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to add hastag: {ex.Message}");
                    }


                }
            }

            user.Point += 10;

            // ✅ Cập nhật lại thông tin user trong database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest("Failed to update user score.");
            }

            await _blogRepo.Add(blog);

            return Ok(blog);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateEvent(int id, [FromForm] BlogDto blogDto, [FromForm] IEnumerable<string> hastagDto)
        {
            if (hastagDto == null || !hastagDto.Any())
                return BadRequest("At least one hastag is required.");
            

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (blogDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(blogDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }



            // Upload ảnh lên S3 nếu có file


            

            var blog = await _blogRepo.GetById(id);

           await filesService.DeleteFileByUrlAsync(blog.Image);




            blog.Title=blogDto.Title;
            blog.Content=blogDto.Content;

            blog.Image = uploadedImageUrl;

            blog.CreatedDate= DateTime.Now;
            blog.CategoryBlogId=blogDto.CategoryBlogId;
            // blog.UserId = user.Id;


            if (hastagDto != null)
            {
                blog.HastagOfBlog.Clear();
                foreach (var hastag in hastagDto)
                {


                    try
                    {
                        blog.HastagOfBlog.Add(new HastagOfBlog { Hashtag = hastag });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to update hastag: {ex.Message}");
                    }


                }
            }


            await _blogRepo.Update(blog);

            return Ok(blog);
        }

        

        [HttpPut("Accept")]
        public async Task<IActionResult> UpdateIsAccept(int id,string message)
        {
            var blog = await _blogRepo.GetById(id);
            if (blog == null)
                return NotFound(new { message = "Blog not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            blog.IsAccept = 1;
            await _blogRepo.Update(blog);

            var blogTitle = blog.Title;
            var senderName = user.UserName;

            var fullMessage = $"{senderName} đã chấp nhận blog {blogTitle} của bạn vì lý do: {message}";


            var notification = new Notification
            {
               ReceiverNotiId=blog.UserId,
               SenderNotiId=user.Id,
                Message = fullMessage,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

           await _notificationRepo.Add(notification);

            await _chatHub.Clients.User(blog.UserId)
                .SendAsync("Update Status", id, fullMessage);

            return Ok(blog);
        }



        [HttpPut("UnAccept")]
        public async Task<IActionResult> UpdateUnAccept(int id,string message)
        {
            var blog = await _blogRepo.GetById(id);
            if (blog == null)
                return NotFound(new { message = "Blog not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            blog.IsAccept = 2;
            await _blogRepo.Update(blog);

            var blogTitle = blog.Title;
            var senderName = user.UserName;

            var fullMessage = $"{senderName} đã từ chối blog {blogTitle} của bạn vì lý do: {message}";
            var notification = new Notification
            {
                ReceiverNotiId = blog.UserId,
                SenderNotiId = user.Id,
                Message = fullMessage,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

           await _notificationRepo.Add(notification);

            await _chatHub.Clients.User(blog.UserId)
               .SendAsync("Update Status", id, fullMessage);
            return Ok(blog);
        }


        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _blogRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _blogRepo.Delete(id);
            return Ok();


        }
    }
}
