using AutoMapper;
using Business.DTO;

using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FigureController : ControllerBase
    {
        private readonly IFigureRepo _figureRepo;
        private readonly FilesService _filesService;
        private readonly IMapper _mapper;
        public FigureController(IFigureRepo figureRepo, FilesService filesService,
            IMapper mapper)
        {
           _figureRepo=figureRepo;
            _filesService = filesService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArtifact()
        {

            try
            {
                var artifact = await _figureRepo.GetAll();


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFigureById(int id)
        {

            try
            {
                var artifact = await _figureRepo.GetById(id);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("GetFigureByName")]
        public async Task<IActionResult> GetFigureByName(string figureName)
        {

            try
            {
                var artifact = await _figureRepo.GetByName(figureName);


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
                var artifact = await _figureRepo.GetAllByCategory(categoryName);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateArtifact([FromForm] FigureDto figureDto, [FromForm] IEnumerable<IFormFile> images)
        {
            if (images == null || !images.Any())
                return BadRequest("At least one image is required.");

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (figureDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(figureDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (figureDto.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await _filesService.UploadFileAsync(figureDto.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload podcast: {ex.Message}");
                }
            }

            var figure = new Figure()
            {
               Name=figureDto.Name,
               Description=figureDto.Description,
               BirthDate=figureDto.BirthDate,
               DeathDate=figureDto.DeathDate,
               Era=figureDto.Era,
               Occupation=figureDto.Occupation,
               Image=uploadedImageUrl,
               Podcast=uploadedPodcastUrl,
               CategoryFigureId=figureDto.CategoryFigureId,
            };

            figure.Images = new List<FigureImage>();
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

                    figure.Images.Add(new FigureImage { ImageUrl = uploadedImagesUrl });
                }
            }

            await _figureRepo.Add(figure);

            return Ok(figureDto);
        }

        [HttpPut("{id}")]

        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateArtifact(int id, [FromForm] FigureDto figureDto, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (figureDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(figureDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (figureDto.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await _filesService.UploadFileAsync(figureDto.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload podcast: {ex.Message}");
                }
            }

            var figure = await _figureRepo.GetById(id);

            _filesService.DeleteFileByUrlAsync(figure.Image);
            _filesService.DeleteFileByUrlAsync(figure.Podcast);

            figure.Name = figureDto.Name;
            figure.Description= figureDto.Description;
            figure.BirthDate= figureDto.BirthDate;
            figure.DeathDate= figureDto.DeathDate;
            figure.Era= figureDto.Era;
            figure.Occupation= figureDto.Occupation;
            figure.CategoryFigureId= figureDto.CategoryFigureId;
            figure.Image = uploadedImageUrl;
            figure.Podcast = uploadedPodcastUrl;

            if (images != null)
            {

                figure.Images.Clear();


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

                    figure.Images.Add(new FigureImage { ImageUrl = uploadedImagesUrl });
                }
            }





            await _figureRepo.Update(figure);

            return Ok(figure);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteArtifact(int id)
        {


            var artifact = await _figureRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _figureRepo.Delete(id);
            return Ok();


        }
    }
}
