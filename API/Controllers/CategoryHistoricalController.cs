using Business;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryHistoricalController : ControllerBase
    {
        private readonly ICategoryHistoricalRepo categoryHistoricalRepo;

        

        public CategoryHistoricalController(ICategoryHistoricalRepo _categoryHistoricalRepo)
        {
            categoryHistoricalRepo = _categoryHistoricalRepo;
           

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCateHistory()
        {

            

            try
            {
                return Ok(await categoryHistoricalRepo.GetAll());
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
                    return Ok(await categoryHistoricalRepo.GetById(id));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            [HttpPost]
            public async Task<IActionResult> CreateHistorical([FromBody] CategoryHistoryDto categoryHistoricalDto)
            {

                var categoryHistorical = new CategoryHistorical
                {
                    Name = categoryHistoricalDto.Name
                };
            
                await categoryHistoricalRepo.Add(categoryHistorical);
                

                return Ok(categoryHistorical);
            }

            [HttpPut("{id}")]


            public async Task<IActionResult> UpdateHistorical(int id, [FromBody] CategoryHistorical categoryHistorical)
            {
                if (!ModelState.IsValid)
                {

                    return BadRequest(ModelState);
                }
                var cateHistory = await categoryHistoricalRepo.GetById(id);
                if (cateHistory == null)
                {
                    return NotFound();
                }
                cateHistory.Name = categoryHistorical.Name;



               await categoryHistoricalRepo.Update(categoryHistorical);

                return Ok(categoryHistorical);
            }

            [HttpDelete("{id}")]


            public async Task<IActionResult> DeleteCateHistorical(int id)
            {


                var history = await categoryHistoricalRepo.GetById(id);
                if (history == null)
                {

                    return NotFound();
                }

               await categoryHistoricalRepo.Delete(id);
                return Ok();


            }
        }
    }

