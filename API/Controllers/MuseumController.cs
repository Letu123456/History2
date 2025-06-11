using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuseumController : ControllerBase
    {
        private readonly IMuseumRepo _museumRepo;
        private readonly FilesService filesService;
        public MuseumController(IMuseumRepo museumRepo, FilesService filesService)
        {
            _museumRepo = museumRepo;
            this.filesService = filesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMuseum()
        {

            try
            {
                return Ok(await _museumRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMuseumById(int id)
        {

            try
            {
                return Ok(await _museumRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMuseum([FromForm] MuseumDto museumDto, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (museumDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(museumDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (museumDto.Video != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(museumDto.Video, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            

            var museum = new Museum()
            {
                Name = museumDto.Name,
                Description = museumDto.Description,
                Video = uploadedVideoUrl,
                Image = uploadedImageUrl,
                Location=museumDto.Location,
                EstablishYear=museumDto.EstablishYear,
                Contact=museumDto.Contact

            };
            museum.Images = new List<MuseumImage>();
            if (images != null)
            {
                string uploadedImagesUrl = null;
                foreach (var file in images)
                {


                    try
                    {
                        uploadedImagesUrl = await filesService.UploadFileAsync(file, "");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to upload image: {ex.Message}");
                    }

                    museum.Images.Add(new MuseumImage { ImageUrl = uploadedImagesUrl });
                }
            }


            await _museumRepo.Add(museum);

            return Ok(museum);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateMuseum(int id, [FromForm] MuseumDto museumDto, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (museumDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(museumDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (museumDto.Video != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(museumDto.Video, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            
            

            var museum = await _museumRepo.GetById(id);

            filesService.DeleteFileByUrlAsync(museum.Image);
            filesService.DeleteFileByUrlAsync(museum.Video);



            museum.Name = museumDto.Name;
            museum.Description = museumDto.Description;

            museum.Image = uploadedImageUrl;
            museum.Video = uploadedVideoUrl;
            museum.Location = museumDto.Location;
            museum.EstablishYear= museumDto.EstablishYear;
            museum.Contact=museumDto.Contact;


            if (images != null)
            {

                museum.Images.Clear();


                string uploadedImagesUrl = null;
                foreach (var file in images)
                {


                    try
                    {
                        uploadedImagesUrl = await filesService.UploadFileAsync(file, "");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to upload image: {ex.Message}");
                    }

                    museum.Images.Add(new MuseumImage { ImageUrl = uploadedImagesUrl });
                }
            }

            await _museumRepo.Update(museum);

            return Ok(museum);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _museumRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _museumRepo.Delete(id);
            return Ok();


        }
    }
}
