using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryProductController : ControllerBase
    {
        private readonly ICategoryProductRepo _categoryRepo;

        public CategoryProductController(ICategoryProductRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            var categoryDtos = categories.Select(c => new CategoryProductDto { Name = c.Name }).ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var categoryDto = new CategoryProductDto { Name = category.Name };
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryProductDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new CategoryProduct { Name = categoryDto.Name };
            var createdCategory = await _categoryRepo.CreateCategoryAsync(category);

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, categoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryProductDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryUpdate = new CategoryProduct { Name = categoryDto.Name };
            var updatedCategory = await _categoryRepo.UpdateCategoryAsync(id, categoryUpdate);

            if (updatedCategory == null) return NotFound();
            return Ok(categoryDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryRepo.DeleteCategoryAsync(id);
            if (!deleted) return BadRequest("Cannot delete category with existing products.");
            return NoContent();
        }
    }
}
