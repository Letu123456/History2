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
    public class ShippingInforController : ControllerBase
    {
        private readonly IShippingInforRepo _shippingInforRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ShippingInforController(IShippingInforRepo shippingInforRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _shippingInforRepo=shippingInforRepo;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _shippingInforRepo.GetAll());
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
                return Ok(await _shippingInforRepo.GetAllByOrderId(orderId));
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
                return Ok(await _shippingInforRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] ShippingInforDto shippingInforDto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }




            var shipping = new ShippingInfor()
            {
                OrderId = shippingInforDto.OrderId,
                Address=shippingInforDto.Address,
                PhoneNumber=shippingInforDto.PhoneNumber,
                DeliveryStatus=shippingInforDto.DeliveryStatus,
            };

            await _shippingInforRepo.Add(shipping);

            return Ok(shipping);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id,string status)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }



            var shipping = await _shippingInforRepo.GetById(id);

            shipping.DeliveryStatus = status;


            await _shippingInforRepo.Update(shipping);

            return Ok(shipping);
        }

        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _shippingInforRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _shippingInforRepo.Delete(id);
            return Ok();


        }
    }
}
