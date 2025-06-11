using Business;
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using DataAccess.Service;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly FilesService filesService;
        private readonly AppDbContext _context;

        public ProductController(IProductRepo productRepo, FilesService filesService, AppDbContext context)
        {
            _productRepo = productRepo;
            this.filesService = filesService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepo.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            if (productDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(productDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Point = productDto.Point,
                Stock = productDto.Stock,
                Image = uploadedImageUrl,
                CategoryProductId = productDto.CategoryProductId
            };

            await _productRepo.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

            if (productDto.Image != null)
            {
                try
                {
                    uploadedImageUrl = await filesService.UploadFileAsync(productDto.Image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            var product = await _productRepo.GetByIdAsync(id);
            filesService.DeleteFileByUrlAsync(product.Image);

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Point = productDto.Point;
            product.Stock = productDto.Stock;
            product.Image = uploadedImageUrl;
            product.CategoryProductId = productDto.CategoryProductId;

            await _productRepo.UpdateAsync(product);
            return Ok(product);
        }
        [HttpPut("updateStock")]
        public async Task<IActionResult> UpdateProduct(int id, int stock  )
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            string? uploadedImageUrl = null;

           

            var product = await _productRepo.GetByIdAsync(id);
            
            product.Stock = stock;
            

            await _productRepo.UpdateAsync(product);
            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (!string.IsNullOrEmpty(product.Image))
            {
                try
                {
                    await filesService.DeleteFileByUrlAsync(product.Image);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to delete image: {ex.Message}");
                }
            }

            await _productRepo.DeleteAsync(id);
            return Ok();
        }

        [HttpPost("redeem/{userId}/{productId}")]
        public async Task<IActionResult> RedeemGift(string userId, int productId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found!" });
            }

            var product = _context.products.Find(productId);
            if (product == null)
            {
                return NotFound(new { message = "Gift not found!" });
            }

            if (user.Point >= product.Point)
            {
                user.Point -= product.Point;

                var redeem = new RedemptionHistory
                {
                    UserId = userId,
                    ProductId = productId,
                    RedeemedAt = DateTime.UtcNow
                };
               _context.redemptionHistories.Add(redeem);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Gift redeemed successfully!", userPoints = user.Point });
            }
            else
            {
                return BadRequest(new { message = "Not enough points to redeem this gift!" });
            }
        }
    }

}


