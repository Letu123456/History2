using Business.Model;
using DataAccess.IRepo;
using DataAccess.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IPaymentTransactionRepo _paymentTransactionRepo;
        private readonly UserManager<User> _userManager;
        public PaymentTransactionController(IPaymentTransactionRepo paymentTransactionRepo,UserManager<User> userManager)
        {
            _paymentTransactionRepo = paymentTransactionRepo;   
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {

            try
            {
                return Ok(await _paymentTransactionRepo.GetAll());
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
                return Ok(await _paymentTransactionRepo.GetAllByUserId(user.Id));
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
                return Ok(await _paymentTransactionRepo.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]


        public async Task<IActionResult> DeleteEvent(int id)
        {


            var history = await _paymentTransactionRepo.GetById(id);
            if (history == null)
            {

                return NotFound();
            }

            await _paymentTransactionRepo.Delete(id);
            return Ok();


        }
    }
}
