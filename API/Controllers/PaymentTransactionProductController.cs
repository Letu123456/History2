using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionProductController : ControllerBase
    {
        private readonly IPaymentTransactionProductRepo _paymentTransactionProductRepo;
        private readonly UserManager<User> _userManager;
        public PaymentTransactionProductController(IPaymentTransactionProductRepo paymentTransactionProductRepo, UserManager<User> userManager)
        {
            _paymentTransactionProductRepo = paymentTransactionProductRepo;
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _paymentTransactionProductRepo.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetAllCommentByuserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            try
            {
                return Ok(await _paymentTransactionProductRepo.GetAllByUserId(user.Id));
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
                return Ok(await _paymentTransactionProductRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _paymentTransactionProductRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _paymentTransactionProductRepo.Delete(id);
            return Ok();


        }
    }
}
