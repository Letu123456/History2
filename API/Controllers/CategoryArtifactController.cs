using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryArtifactController : ControllerBase
    {
        private readonly ICategoryArtifactRepo categoryArtifactRepo;



        public CategoryArtifactController(ICategoryArtifactRepo _categoryArtifactRepo)
        {
            categoryArtifactRepo = _categoryArtifactRepo;


        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {



            try
            {
                return Ok(await categoryArtifactRepo.GetAll());
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
                return Ok(await categoryArtifactRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCateArtifact(string categoryName, int categoryHistoricalId)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            var categoryHistorical = new CategoryArtifact
            {
                Name = categoryName,
                CategoryHistoricalId= categoryHistoricalId
            };

            await categoryArtifactRepo.Add(categoryHistorical);


            return Ok(categoryHistorical);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id, string categoryName, int categoryHistoricalId)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var cateArtifact = await categoryArtifactRepo.GetById(id);
            if (cateArtifact == null)
            {
                return NotFound();
            }
            cateArtifact.Name = categoryName;
            cateArtifact.CategoryHistoricalId = categoryHistoricalId;



            await categoryArtifactRepo.Update(cateArtifact);

            return Ok(cateArtifact);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var artifact = await categoryArtifactRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await categoryArtifactRepo.Delete(id);
            return Ok();


        }
    }
}

    

