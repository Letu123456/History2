using AutoMapper;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtifactController : ControllerBase
    {
        private readonly IArtifactRepo _artifactRepo;
        private readonly FilesService _filesService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public ArtifactController(IArtifactRepo artifactRepo,FilesService filesService,
            IMapper mapper, UserManager<User> userManager)
        {
            _artifactRepo = artifactRepo;
            _filesService = filesService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArtifact()
        {

            try
            {
                var artifact = await _artifactRepo.GetAll();


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtifactById(int id)
        {

            try
            {
                var artifact = await _artifactRepo.GetById(id);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetArtifactByCategory")]
        public async Task<IActionResult> GetArtifactByCategory(string categoryName)
        {

            try
            {
                var artifact = await _artifactRepo.GetAllByCategory(categoryName);


                return Ok(artifact);
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

          
            if (user.MuseumId == null) {
                return NotFound();
            }

            try
            {
                var artifact = await _artifactRepo.GetAllByMuseumId((int)user.MuseumId);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateArtifact([FromForm] ArtifactDto artifactRq, [FromForm] IEnumerable<IFormFile> images)
        {
            if (images == null || !images.Any())
                return BadRequest("At least one image is required.");

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (artifactRq.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(artifactRq.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (artifactRq.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await _filesService.UploadFileAsync(artifactRq.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload podcast: {ex.Message}");
                }
            }

            var artifact = new Artifact()
            {
                ArtifactName = artifactRq.Name,
                Description = artifactRq.Description,
                Image=uploadedImageUrl,
                Podcast=uploadedPodcastUrl,
                CategoryArtifactId = artifactRq.CategoryArtifactId,
                MuseumId=artifactRq.MuseumId,
                DateDiscovered= artifactRq.DateDiscovered,
                Dimenson=artifactRq.Dimenson,
                Weight=artifactRq.Weight,
                Material=artifactRq.Material,
                Function=artifactRq.Function,
                Condition=artifactRq.Condition,
                Origin=artifactRq.Origin,
                Status=true,
            };

            artifact.Images = new List<ArtifactImage>();
            if (images != null)
            {
                string uploadedImagesUrl = null;
                foreach (var file in images)
                {
                 

                    try
                    {
                         uploadedImagesUrl = await _filesService.UploadFileAsync(file, "");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to upload image: {ex.Message}");
                    }

                    artifact.Images.Add(new ArtifactImage { ImageUrl = uploadedImagesUrl });
                }
            }

            await _artifactRepo.Add(artifact);

            return Ok(artifactRq);
        }

        [HttpPut("{id}")]

        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateArtifact(int id, [FromForm] ArtifactUpdateDto artifactRq, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (artifactRq.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(artifactRq.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (artifactRq.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await _filesService.UploadFileAsync(artifactRq.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload podcast: {ex.Message}");
                }
            }

            var artifact = await _artifactRepo.GetById(id);

           await _filesService.DeleteFileByUrlAsync(artifact.Image);
           await _filesService.DeleteFileByUrlAsync(artifact.Podcast);



            artifact.ArtifactName= artifactRq.Name;
            artifact.Description = artifactRq.Description;
            artifact.Image = uploadedImageUrl;
            artifact.Podcast = uploadedPodcastUrl;

            artifact.CategoryArtifactId = artifactRq.CategoryArtifactId;
            artifact.MuseumId= artifactRq.MuseumId;
            artifact.DateDiscovered= artifactRq.DateDiscovered;
            artifact.Dimenson= artifactRq.Dimenson;
            artifact.Function= artifactRq.Function;
            artifact.Weight= artifactRq.Weight;
            artifact.Material= artifactRq.Material;
            artifact.Condition= artifactRq.Condition;
            artifact.Origin= artifactRq.Origin;
            artifact.Status = true;
            if (images != null)
            {

                artifact.Images.Clear();


                string uploadedImagesUrl = null;
                foreach (var file in images)
                {


                    try
                    {
                        uploadedImagesUrl = await _filesService.UploadFileAsync(file, "");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Failed to upload image: {ex.Message}");
                    }

                    artifact.Images.Add(new ArtifactImage { ImageUrl = uploadedImagesUrl });
                }
            }





            await _artifactRepo.Update(artifact);

            return Ok(artifact);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteArtifact(int id)
        {


            var artifact = await _artifactRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _artifactRepo.Delete(id);
            return Ok();


        }

        [HttpPut("statusFalse")]

        public async Task<IActionResult> UpdateStatusFalse(int id)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            var appli = await _artifactRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }


            
            appli.Status = false;


            await _artifactRepo.Update(appli);

            return Ok(appli);
        }

        [HttpPut("statusTrue")]

        public async Task<IActionResult> UpdateStatusTrue(int id)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            var appli = await _artifactRepo.GetById(id);
            if (appli == null)
            {
                return NotFound();
            }



            appli.Status = true;


            await _artifactRepo.Update(appli);

            return Ok(appli);
        }
    }
}
