using Business.DTO;

using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricalController : ControllerBase
    {
        private readonly IHistoricalRepo _historicalRepo;
        private readonly FilesService filesService;
        public HistoricalController(IHistoricalRepo historicalRepo,FilesService filesService)
        {
            _historicalRepo = historicalRepo;
           this.filesService = filesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHistorical()
        {

            try
            {
                return Ok(await _historicalRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHistoricalById(int id)
        {

            try
            {
                return Ok(await _historicalRepo.GetById(id));
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
                var artifact = await _historicalRepo.GetAllByCategory(categoryName);


                return Ok(artifact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateHistorical([FromForm] HistoricalDto historicalDto, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historicalDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(historicalDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historicalDto.Video != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(historicalDto.Video, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historicalDto.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await filesService.UploadFileAsync(historicalDto.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var history = new Historical()
            {
                Name = historicalDto.Name,
                Description = historicalDto.Description,
                Video = uploadedVideoUrl,
                Image = uploadedImageUrl,
                Podcast = uploadedPodcastUrl,
                StartDate = historicalDto.StartDate,
                EndDate = historicalDto.EndDate,
                CategoryHistorycalId = historicalDto.CategoryId,
                TimeLine= historicalDto.TimeLine,
                Location= historicalDto.Location,
                Gorvernance= historicalDto.Gorvernance,
                PolitialStructure=historicalDto.PolitialStructure,
                Figure=historicalDto.Figure
            };


            history.Images = new List<HistoricalImage>();
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

                    history.Images.Add(new HistoricalImage { ImageUrl = uploadedImagesUrl });
                }
            }

            foreach (var item in historicalDto.ArtifactIds) {

                history.ArtifactHistoricals.Add(new ArtifactHistorical()
                {
                    Historical = history,
                    ArtifacrId = item
                });
            
            }

            foreach (var item in historicalDto.FigureIds)
            {

                history.HistoricalFigures.Add(new HistoricalFigure()
                {
                    Historical = history,
                    FigureId = item
                });

            }



            await _historicalRepo.Add(history);

            return Ok(history);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateHistorical(int id, [FromForm] HistoricalDto historical, [FromForm] IEnumerable<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historical.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(historical.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedVideoUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historical.Video != null)
            {
                try
                {
                    uploadedVideoUrl = await filesService.UploadFileAsync(historical.Video, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            string? uploadedPodcastUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (historical.Podcast != null)
            {
                try
                {
                    uploadedPodcastUrl = await filesService.UploadFileAsync(historical.Podcast, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var history = await _historicalRepo.GetById(id);

            filesService.DeleteFileByUrlAsync(history.Image);
            filesService.DeleteFileByUrlAsync(history.Video);
            filesService.DeleteFileByUrlAsync(history.Podcast);


            history.Name = historical.Name;
            history.Description = historical.Description;
            history.StartDate = historical.StartDate;
            history.EndDate = historical.EndDate;
            history.Image = uploadedImageUrl;
            history.Video=uploadedVideoUrl;
            history.Podcast=uploadedPodcastUrl;
            history.TimeLine=historical.TimeLine;
            history.Location=historical.Location;
            history.Gorvernance=historical.Gorvernance;
            history.PolitialStructure=historical.PolitialStructure;
            history.Figure=historical.Figure;

            if (images != null)
            {

                history.Images.Clear();


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

                    history.Images.Add(new HistoricalImage { ImageUrl = uploadedImagesUrl });
                }
            }


            var existIds = history.ArtifactHistoricals.Select(x=>x.Id).ToList();
            var selectIds = historical.ArtifactIds.ToList();
            var toAdd = selectIds.Except(existIds).ToList();
            var toRemove = existIds.Except(selectIds).ToList();

            history.ArtifactHistoricals = history.ArtifactHistoricals.Where(x=>!toRemove.Contains((int)x.ArtifacrId)).ToList();
            foreach(var item in toAdd)
            {
                history.ArtifactHistoricals.Add(new ArtifactHistorical()
                {
                    ArtifacrId = item
                });

            }

            var existId = history.HistoricalFigures.Select(x => x.Id).ToList();
            var selectId = historical.FigureIds.ToList();
            var toAdds = selectId.Except(existId).ToList();
            var toRemoves = existId.Except(selectId).ToList();

            history.HistoricalFigures = history.HistoricalFigures.Where(x => !toRemoves.Contains((int)x.FigureId)).ToList();
            foreach (var item in toAdds)
            {
                history.HistoricalFigures.Add(new HistoricalFigure()
                {
                    FigureId = item
                });

            }

            await _historicalRepo.Update(history);

            return Ok(history);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteHistorical(int id)
        {


            var history = await _historicalRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

           await _historicalRepo.Delete(id);
            return Ok();


        }

    }
}
