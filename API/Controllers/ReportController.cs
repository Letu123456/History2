using API.Hubs;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepo _reportRepo;
        private readonly UserManager<User> _userManager;
        private readonly INotificationRepo _notificationRepo;
        private readonly IHubContext<ChatHub> _chatHub;


        public ReportController(IReportRepo reportRepo, UserManager<User> userManager, INotificationRepo notificationRepo, IHubContext<ChatHub> chatHub)
        {
            _reportRepo = reportRepo;
            _userManager = userManager;
            _notificationRepo = notificationRepo;
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {

            try
            {
                return Ok(await _reportRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCateArtifactById(int id)
        {

            try
            {
                return Ok(await _reportRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCateArtifact([FromBody] ReportDto reportDto)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var report = new Report()
            {
                Title = reportDto.Title,
                Content = reportDto.Content,
                Status = false,
                Created = DateTime.Now,
                UserId = user.Id

            };
            await _reportRepo.Add(report);


            return Ok(report);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id, [FromBody] ReportDto reportDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var report = await _reportRepo.GetById(id);
            if (report == null)
            {
                return NotFound();
            }
            report.Title = reportDto.Title;
            report.Content = reportDto.Content;
            report.Status = false;
            report.Created = DateTime.Now;

            await _reportRepo.Update(report);

            return Ok(report);
        }


        [HttpPut("statusTrue")]

        public async Task<IActionResult> UpdateStatusTrue(int id, string message)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            
            var appli = await _reportRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            appli.Status = true;
            

            await _reportRepo.Update(appli);

            var appliTitle = appli.Title;
            var senderName = user.UserName;

            var fullMessage = $"{senderName} đã nhận đơn báo cáo {appliTitle} của bạn vì lý do: {message}";


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
               .SendAsync("Update status", id, fullMessage);

            return Ok(appli);
        }


        

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var artifact = await _reportRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _reportRepo.Delete(id);
            return Ok();


        }
    }
}
