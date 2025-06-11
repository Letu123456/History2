using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public OrderController(IOrderRepo orderRepo, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _orderRepo = orderRepo;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _orderRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetAllOrderByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            try
            {
                return Ok(await _orderRepo.GetAllByUserId(user.Id));
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
                return Ok(await _orderRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] OrderDto orderDto)
        {




            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);



            var order = new Order()
            {

                OrderCode = orderDto.OrderCode,
                UserId = user.Id,
                TotalAmount= orderDto.TotalAmount,
                Status="Đang chờ duyệt",
                CreatedAt= DateTime.UtcNow.AddHours(7),


            };

            await _orderRepo.Add(order);

            return Ok(order);
        }

        [HttpPut("{id}")]


        public async Task<IActionResult> UpdateComment(int id,string status)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            

            var order = await _orderRepo.GetById(id);

            order.Status= status;
            

            await _orderRepo.Update(order);

            return Ok(order);
        }


        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteComment(int id)
        {


            var artifact = await _orderRepo.GetById(id);
            if (artifact == null)
            {

                return NotFound();
            }

            await _orderRepo.Delete(id);
            return Ok();


        }
    }
}
