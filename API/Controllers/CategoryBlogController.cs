using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryBlogController : ControllerBase
    {
        private readonly ICategoryBlogRepo categoryBlogRepo;



        public CategoryBlogController(ICategoryBlogRepo _categoryBlogRepo)
        {
            categoryBlogRepo = _categoryBlogRepo;


        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {



            try
            {
                return Ok(await categoryBlogRepo.GetAll());
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
                return Ok(await categoryBlogRepo.GetById(id));
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


            var categoryBlog = new CategoryBlog
            {
                Name = categoryArtifactDto.Name
            };

            await categoryBlogRepo.Add(categoryBlog);


            return Ok(categoryBlog);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id, [FromBody] CategoryHistoryDto categoryArtifactDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var cateArtifact = await categoryBlogRepo.GetById(id);
            if (cateArtifact == null)
            {
                return NotFound();
            }
            cateArtifact.Name = categoryArtifactDto.Name;



            await categoryBlogRepo.Update(cateArtifact);

            return Ok(cateArtifact);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var artifact = await categoryBlogRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await categoryBlogRepo.Delete(id);
            return Ok();


        }
    }
}
