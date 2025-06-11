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
    public class CategoryFigureController : ControllerBase
    {
        private readonly ICategoryFigureRepo _categoryFigureRepo;



        public CategoryFigureController(ICategoryFigureRepo categoryFigureRepo)
        {
          _categoryFigureRepo = categoryFigureRepo;


        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateArtifact()
        {



            try
            {
                return Ok(await _categoryFigureRepo.GetAll());
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
                return Ok(await _categoryFigureRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCateArtifact([FromBody] CategoryHistoryDto categoryHistoryDto)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }


            var categoryHistorical = new CategoryFigure
            {
                Name = categoryHistoryDto.Name
            };

            await _categoryFigureRepo.Add(categoryHistorical);


            return Ok(categoryHistorical);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateCateArtifact(int id, [FromBody] CategoryHistoryDto categoryHistoryDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            var cateArtifact = await _categoryFigureRepo.GetById(id);
            if (cateArtifact == null)
            {
                return NotFound();
            }
            cateArtifact.Name = categoryHistoryDto.Name;



            await _categoryFigureRepo.Update(cateArtifact);

            return Ok(cateArtifact);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteCateArtifact(int id)
        {


            var artifact = await _categoryFigureRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _categoryFigureRepo.Delete(id);
            return Ok();


        }
    }
}
