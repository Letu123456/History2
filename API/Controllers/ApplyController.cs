using API.Hubs;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly IAppliRepo _appliRepo;
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepo _roleRepo;
        private readonly INotificationRepo _notificationRepo;
        private readonly FilesService _filesService;
        private readonly IUserRepo _userRepo;
        private readonly IHubContext<ChatHub> _chatHub;

        public ApplyController(IAppliRepo appliRepo,UserManager<User> userManager, IRoleRepo roleRepo,INotificationRepo notificationRepo,FilesService filesService,IUserRepo userRepo, IHubContext<ChatHub> chatHub)
        {
            _appliRepo = appliRepo;
            _userManager = userManager;
            _roleRepo = roleRepo;
            _notificationRepo = notificationRepo;
            _filesService = filesService;
            _userRepo = userRepo;
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                return Ok(await _appliRepo.GetAll());
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
                return Ok(await _appliRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppli([FromForm] AppliDto appliDto)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (appliDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(appliDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            User user = await _userManager.FindByIdAsync(userId);

            var appli = new Appli()
            {
                Title=appliDto.Title,
                Content=appliDto.Content,
               Image=uploadedImageUrl,
                Status=0,
                Created=DateTime.Now,
                UserId=user.Id

            };
            await _appliRepo.Add(appli);


            return Ok(appli);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAppli(int id, [FromForm] AppliDto appliDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (appliDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(appliDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var appli = await _appliRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }
            appli.Title=appliDto.Title;
            appli.Content=appliDto.Content;
            appli.Status = 0;
            appli.Image = uploadedImageUrl;
            appli.Created=DateTime.Now;

           
            await _appliRepo.Update(appli);

            return Ok(appli);
        }


        [HttpPut("statusTrue")]

        public async Task<IActionResult> UpdateStatusTrue(int id, string message,int museumId)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            string manager = "manager";
            var appli = await _appliRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);
            appli.Status = 1;
           await _roleRepo.UpdateUserRole(appli.UserId, manager);
            await _userRepo.UpdateUserMuseumAsync(appli.UserId, museumId);

            await _appliRepo.Update(appli);

            var appliTitle = appli.Title;
            var senderName = user.UserName;

            var fullMessage = $"{senderName} đã chấp nhận đơn {appliTitle} của bạn vì lý do: {message}";


            var notification = new Notification
            {
                ReceiverNotiId = appli.UserId,
                SenderNotiId = user.Id,
                Message = fullMessage,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

           await _notificationRepo.Add(notification);

            await _chatHub.Clients.User(appli.UserId)
               .SendAsync("Update Status", id, fullMessage);

            return Ok(appli);
        }


        [HttpPut("statusFalse")]

        public async Task<IActionResult> UpdateStatusFalse(int id, string message)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            
            var appli = await _appliRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);


            appli.Status = 2;


            await _appliRepo.Update(appli);

            var appliTitle = appli.Title;
            var senderName = user.UserName;

            var fullMessage = $"{senderName} đã từ chối đơn {appliTitle} của bạn vì lý do: {message}";


            var notification = new Notification
            {
                ReceiverNotiId = appli.UserId,
                SenderNotiId = user.Id,
                Message = fullMessage,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

           await _notificationRepo.Add(notification);

            await _chatHub.Clients.User(appli.UserId)
               .SendAsync("Update Status", id, fullMessage);

            return Ok(appli);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteAppli(int id)
        {


            var artifact = await _appliRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _appliRepo.Delete(id);
            return Ok();


        }
    }
}
