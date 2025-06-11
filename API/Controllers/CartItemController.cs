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
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepo _cartItemRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public CartItemController(ICartItemRepo cartItemRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
           _cartItemRepo = cartItemRepo;
            _userManager = userManager;
            _signInManager = signInManager;
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _cartItemRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByCartId")]
        public async Task<IActionResult> GetAllCartItemByCartId(int cartId)
        {

            try
            {
                return Ok(await _cartItemRepo.GetAllByCartId(cartId));
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
                return Ok(await _cartItemRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng hay chưa
            var existingCartItem = await _cartItemRepo
                .FindByCartIdAndProductId(cartItemDto.cartId, cartItemDto.productId);

            if (existingCartItem != null)
            {
                // Nếu đã tồn tại, tăng số lượng lên 1
                existingCartItem.Quatity += cartItemDto.quatity;
                await _cartItemRepo.Update(existingCartItem);
                return Ok(existingCartItem);
            }
            else
            {
                // Nếu chưa tồn tại, thêm mới sản phẩm vào giỏ
                var cartItem = new CartItem()
                {
                    CartId = cartItemDto.cartId,
                    ProductId = cartItemDto.productId,
                    Quatity = cartItemDto.quatity
                };

                await _cartItemRepo.Add(cartItem);
                return Ok(cartItem);
            }
        }


        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateComment(int id, [FromBody] CartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }



            var cartItem = await _cartItemRepo.GetById(id);

            cartItem.CartId = cartItemDto.cartId;
            cartItem.ProductId= cartItemDto.productId;
            cartItem.Quatity= cartItemDto.quatity;


            await _cartItemRepo.Update(cartItem);

            return Ok(cartItem);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _cartItemRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _cartItemRepo.Delete(id);
            return Ok();


        }
    }
}
