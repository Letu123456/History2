using Business;
using Business.Model;
using DataAccess.Service;
using Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("success")]
        public async Task<IActionResult> PaymentSuccess([FromBody] PaymentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid payment data.");
            }

            var transaction = new TransactionHistory
            {
                UserId = request.UserId,
                Amount = request.Amount,
                PremiumPlan = request.PremiumPlan,
                Status = "Success",
                CreatedAt = DateTime.UtcNow
            };

            _context.transactionHistories.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment recorded successfully." });
        }
    }

    public class PaymentRequest
    {
        public string UserId { get; set; }
        public string Amount { get; set; }
        public string PremiumPlan { get; set; }
    }
}
