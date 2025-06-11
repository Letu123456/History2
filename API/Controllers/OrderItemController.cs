using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepo _orderItemRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public OrderItemController(IOrderItemRepo orderItemRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _orderItemRepo=orderItemRepo;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _orderItemRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByOrderId")]
        public async Task<IActionResult> GetAllOrderItemByCartId(int orderId)
        {

            try
            {
                return Ok(await _orderItemRepo.GetAllByOrderId(orderId));
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
                return Ok(await _orderItemRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] OrderItemDto orderItemDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }




            var cartItem = new OrderItem()
            {
                OrderId = orderItemDto.OrderId,
                ProdutId=orderItemDto.ProductId,
                Quatity=orderItemDto.Quatity,
                PriceAtPurchase=orderItemDto.PriceAtPurchase,
            };

            await _orderItemRepo.Add(cartItem);

            return Ok(cartItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, int quatity)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }



            var orderItem = await _orderItemRepo.GetById(id);

            orderItem.Quatity   =quatity;


            await _orderItemRepo.Update(orderItem);

            return Ok(orderItem);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _orderItemRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _orderItemRepo.Delete(id);
            return Ok();


        }
    }
}
