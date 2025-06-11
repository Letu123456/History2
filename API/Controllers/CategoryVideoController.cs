using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryVideoController : ControllerBase
    {
        private readonly ICategoryVideoRepo categoryVideoRepo;



        public CategoryVideoController(ICategoryVideoRepo _categoryVideoRepo)
        {
            categoryVideoRepo = _categoryVideoRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {



            try
            {
                return Ok(await categoryVideoRepo.GetAll());
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
                return Ok(await categoryVideoRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCateArtifact([FromBody] CategoryHistoryDto categoryArtifactDto)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            var categoryBlog = new CategoryVideo
            {
                Name = categoryArtifactDto.Name
            };

            await categoryVideoRepo.Add(categoryBlog);


            return Ok(categoryBlog);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id, [FromBody] CategoryHistoryDto categoryArtifactDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var cateArtifact = await categoryVideoRepo.GetById(id);
            if (cateArtifact == null)
            {
                return NotFound();
            }
            cateArtifact.Name = categoryArtifactDto.Name;



            await categoryVideoRepo.Update(cateArtifact);

            return Ok(cateArtifact);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var artifact = await categoryVideoRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await categoryVideoRepo.Delete(id);
            return Ok();


        }
    }
}
