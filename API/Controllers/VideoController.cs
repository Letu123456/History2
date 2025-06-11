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
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepo _videoRepo;
        private readonly FilesService filesService;
        private readonly UserManager<User> _userManager;
        
        
        
        public VideoController(IVideoRepo videoRepo, FilesService filesService, UserManager<User> userManager)
        {
            
            this.filesService = filesService;
            _userManager = userManager;
            _videoRepo = videoRepo;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvent()
        {

            try
            {
                return Ok(await _videoRepo.GetAll());
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
                return Ok(await _videoRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllByCategoryId")]
        public async Task<IActionResult> GetVideoByCategoryId(int CategoryId)
        {

            try
            {
                return Ok(await _videoRepo.GetAllByCategory(CategoryId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }






        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] VideoDto videoDto)
        {
            
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (videoDto.VideoClip != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(videoDto.VideoClip, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload video: {ex.Message}");
                }
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var video = new Video()
            {
                Name=videoDto.Name,
                Description=videoDto.Description,
                Source=videoDto.Source,
                Music=videoDto.Music,
                VideoClip=uploadedVideoUrl,
                CategoryVideoId=videoDto.CategoryId,
                UserId = user.Id,
                

            };
            
            

            await _videoRepo.Add(video);

            return Ok(video);
        }
        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateEvent(int id, [FromForm] VideoDto videoDto)
        {
            
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (videoDto.VideoClip != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(videoDto.VideoClip, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }



            // Upload ảnh lên S3 nếu có file




            var video = await _videoRepo.GetById(id);

            await filesService.DeleteFileByUrlAsync(video.VideoClip);




            video.Name= videoDto.Name;
            video.Description= videoDto.Description;
            video.Source= videoDto.Source;
            video.Music= videoDto.Music;
            video.VideoClip = uploadedVideoUrl;
            video.CategoryVideoId = videoDto.CategoryId;


           
            


            await _videoRepo.Update(video);

            return Ok(video);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _videoRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _videoRepo.Delete(id);
            return Ok();


        }
    }
}
