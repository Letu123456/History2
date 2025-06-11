using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepo _eventRepo;
        private readonly FilesService filesService;
        private readonly UserManager<User> _userManager;
        public EventController(IEventRepo eventRepo, FilesService filesService, UserManager<User> userManager)
        {
            _eventRepo = eventRepo;
            this.filesService = filesService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvent()
        {

            try
            {
                return Ok(await _eventRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getByHashtag")]
        public async Task<IActionResult> GetAllEventByHashtag(string hastag)
        {

            if (string.IsNullOrWhiteSpace(hastag))
            {
                return BadRequest("Hashtag cannot be empty.");
            }

            try
            {
                return Ok(await _eventRepo.GetAllByHashtag(hastag));
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
                return Ok(await _eventRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllByMuseumId")]
        public async Task<IActionResult> GetAllByMuseumId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);


            if (user.MuseumId == null)
            {
                return NotFound();
            }

            try
            {
                var artifact = await _eventRepo.GetAllByMuseumId((int)user.MuseumId);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] EventDto eventDto, [FromForm] IEnumerable<string> hastagDto)
        {
            if (hastagDto == null || !hastagDto.Any())
                return BadRequest("At least one hastag is required.");
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (eventDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(eventDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            



            var events = new Event()
            {
                Name = eventDto.Name,
                Description = eventDto.Description,
                
                Image = uploadedImageUrl,
                Location = eventDto.Location,
                StartDate = eventDto.StartDate,
                EndDate= eventDto.EndDate,
                MuseumId= eventDto.MuseumId,
                

            };

            events.Hastag = new List<Hastag>();
            if (hastagDto != null)
            {
               
                foreach (var hastag in hastagDto)
                {


                    try
                    {
                        events.Hastag.Add(new Hastag { Hashtag = hastag });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to add hastag: {ex.Message}");
                    }

                    
                }
            }

            await _eventRepo.Add(events);

            return Ok(events);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]


        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventDto eventDto, [FromForm] IEnumerable<string> hastagDto)
        {
            if (hastagDto == null || !hastagDto.Any())
                return BadRequest("At least one hastag is required.");
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (eventDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(eventDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to up image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            



            var events = await _eventRepo.GetById(id);

            filesService.DeleteFileByUrlAsync(events.Image);




            events.Name = eventDto.Name;
            events.Description = eventDto.Description;

            events.Image = uploadedImageUrl;
            
            events.Location = eventDto.Location;
            events.StartDate = eventDto.StartDate;
            events.EndDate = eventDto.EndDate;
            events.MuseumId=eventDto.MuseumId;
            

           
            if (hastagDto != null)
            {
                events.Hastag.Clear();
                foreach (var hastag in hastagDto)
                {


                    try
                    {
                        events.Hastag.Add(new Hastag { Hashtag = hastag });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to update hastag: {ex.Message}");
                    }


                }
            }

            await _eventRepo.Update(events);

            return Ok(events);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _eventRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _eventRepo.Delete(id);
            return Ok();


        }
    }
}
