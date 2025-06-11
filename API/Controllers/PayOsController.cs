
using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly PayOsService _payOsService;
        private readonly IPaymentTransactionRepo _paymentTransactionRepo;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPaymentTransactionProductRepo _paymentProductRepo;

        public PayOsController(PayOsService payOsService,IPaymentTransactionRepo paymentTransactionRepo,UserManager<User> userManager,IConfiguration configuration,IPaymentTransactionProductRepo paymentTransactionProductRepo)
        {
            _payOsService = payOsService;
            _paymentTransactionRepo = paymentTransactionRepo;
            _userManager = userManager;
            _configuration = configuration;
            _paymentProductRepo = paymentTransactionProductRepo;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(string successUrl,string cancelUrl)
        {
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

            List<ItemData> items = new List<ItemData>();
            foreach (var cartItem in ShoppingCartDto.ListCart)
            {
                var item = new ItemData(
                    cartItem.Product.Name,
                    cartItem.Count,
                    (int)(cartItem.Product.Price * 1000));
                items.Add(item);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);

            var createPayment = await _payOsService.CreatePaymentLinkAsync(
                2000,
                items,
                "Thanh toan premium",
                "successUrl",
                "cancelUrl", //https://google.com
                orderCode);

            var transaction = new PaymentTransaction
            {
                OrderCode = orderCode.ToString(),
                CheckoutUrl = createPayment.checkoutUrl,
                Status = "PENDING", // Trạng thái khởi tạo
                TotalPrice=2000,
                Created = DateTime.UtcNow.AddHours(7),
                UserId = user.Id
            };

            await _paymentTransactionRepo.Add(transaction);

            return Ok(new
            {
                orderCode = orderCode,
                checkoutUrl = createPayment.checkoutUrl
            });
        }

        [HttpPost("create-payment-forProduct")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { message = "Items list cannot be empty." });
            }

            try
            {
                // Generate order code based on current timestamp (microseconds)
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

                // Convert request items to ItemData
                var items = request.Items.Select(item => new ItemData(
                    item.Name,
                    item.Quantity,
                    item.Price
                )).ToList();

                // Calculate total amount (sum of price * quantity for each item)
                //int totalAmount = items.Sum(item => item.price * item.quantity);

                // Get user ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                // Get user details
                User user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Create payment link
                var createPayment = await _payOsService.CreatePaymentLinkAsync(
                    request.TotalPrice,
                    items,
                    "Thanh toan don hang",
                    request.SuccessUrl,
                    request.CancelUrl,
                    orderCode
                );

                // Create and save payment transaction
                var transaction = new PaymentTransactionProduct
                {
                    OrderCode = orderCode.ToString(),
                    CheckoutUrl = createPayment.checkoutUrl,
                    TotalPrice=request.TotalPrice,
                    OrderId=request.OrderId,
                    Status = "PENDING",
                    Created = DateTime.UtcNow.AddHours(7),
                    UserId = user.Id
                };

                await _paymentProductRepo.Add(transaction);

                // Return response
                return Ok(new
                {
                    orderCode = orderCode,
                    checkoutUrl = createPayment.checkoutUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the payment", error = ex.Message });
            }
        }

        [HttpPost("StatusBill")]
        public async Task<IActionResult> CheckPaymentStatusAsync([FromQuery] string orderCode)
        {
            var resultJson = await _payOsService.GetPaymentStatusAsync(orderCode);
            return Content(resultJson, "application/json");
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] dynamic payload)
        {
            string orderCode = payload.data?.orderCode?.ToString();

            if (string.IsNullOrEmpty(orderCode))
                return BadRequest("Missing orderCode");

            // 🔁 Gọi lại để lấy thông tin đầy đủ từ PayOS
            var resultJson = await _payOsService.GetPaymentStatusAsync(orderCode);
            dynamic statusPayload = JsonConvert.DeserializeObject(resultJson);

            // Lấy giá trị cancel từ payload webhook (nếu có), ưu tiên dùng trong URL như bạn nói
            bool isCancelled = false;
            try
            {
                isCancelled = payload.data.cancel == true; // Đảm bảo an toàn nếu không có field
            }
            catch { }

            // Nếu cancel là true → CANCELLED, ngược lại → dùng status trả về từ PayOS
            string status = isCancelled ? "CANCELLED" : statusPayload?.data?.status?.ToString()?.ToUpper();

            if (string.IsNullOrEmpty(status))
                return BadRequest("Không xác định được trạng thái đơn hàng");

            // ✅ Cập nhật DB
            await _paymentTransactionRepo.UpdateTransactionStatusAsync(orderCode, status);

            // ✅ Nếu đã thanh toán thành công thì nâng cấp Premium
            if (status == "PAID")
            {
                var transaction = await _paymentTransactionRepo.GetTransactionByOrderCodeAsync(orderCode);
                if (transaction != null && !string.IsNullOrEmpty(transaction.UserId))
                {
                    var user = await _userManager.FindByIdAsync(transaction.UserId);
                    if (user != null)
                    {
                        user.IsPremium = true;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            return Ok(); // Trả về 200 cho PayOS
        }

        [HttpPost("webhookForProduct")]
        public async Task<IActionResult> PayOSWebhookForProduct([FromBody] dynamic payload)
        {
            string orderCode = payload.data?.orderCode?.ToString();

            if (string.IsNullOrEmpty(orderCode))
                return BadRequest("Missing orderCode");

            // 🔁 Gọi lại để lấy thông tin đầy đủ từ PayOS
            var resultJson = await _payOsService.GetPaymentStatusAsync(orderCode);
            dynamic statusPayload = JsonConvert.DeserializeObject(resultJson);

            // Lấy giá trị cancel từ payload webhook (nếu có), ưu tiên dùng trong URL như bạn nói
            bool isCancelled = false;
            try
            {
                isCancelled = payload.data.cancel == true; // Đảm bảo an toàn nếu không có field
            }
            catch { }

            // Nếu cancel là true → CANCELLED, ngược lại → dùng status trả về từ PayOS
            string status = isCancelled ? "CANCELLED" : statusPayload?.data?.status?.ToString()?.ToUpper();

            if (string.IsNullOrEmpty(status))
                return BadRequest("Không xác định được trạng thái đơn hàng");

            // ✅ Cập nhật DB
            await _paymentTransactionRepo.UpdateTransactionStatusAsync(orderCode, status);

            // ✅ Nếu đã thanh toán thành công thì nâng cấp Premium
            

            return Ok(); // Trả về 200 cho PayOS
        }



    }
}

public class CreatePaymentRequest
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public int OrderId {  get; set; }
    public int TotalPrice {  get; set; }
    public List<Item> Items { get; set; }

    public class Item
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}