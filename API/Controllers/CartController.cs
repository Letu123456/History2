using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _cartRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public CartController(ICartRepo cartRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _cartRepo = cartRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _cartRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetAllCartByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            try
            {
                return Ok(await _cartRepo.GetAllByUserId(user.Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {

            try
            {
                return Ok(await _cartRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment()
        {
            
            

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var cart = new Cart()
            {
                
                CreatedDate = DateTime.Now,
                UserId = user.Id,
               
            };

            await _cartRepo.Add(cart);

            return Ok(cart);
        }

        

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _cartRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _cartRepo.Delete(id);
            return Ok();


        }
    }
}
