using Business.DTO;
using Business.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly GhnService _ghnService;

        public GhnController(GhnService ghnService)
        {
            _ghnService = ghnService;
        }

        [HttpPost("calculate-fee")]
        public async Task<IActionResult> CalculateFee([FromBody] object payload)
        {
            var result = await _ghnService.CalculateFeeAsync(payload);
            return Ok(JsonConvert.DeserializeObject(result));
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] object payload)
        {
            var result = await _ghnService.CreateOrderAsync(payload);
            return Ok(JsonConvert.DeserializeObject(result));
        }
       

        [HttpPost("orders")]
        public async Task<IActionResult> GetAllOrders([FromBody] OrderListRequest request)
        {
            var result = await _ghnService.GetAllOrdersAsync(request);
            return Ok(JsonConvert.DeserializeObject(result));
        }

        [HttpPost("order-detail")]
        public async Task<IActionResult> GetOrderDetail([FromBody] string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                return BadRequest("order_code là bắt buộc.");

            var result = await _ghnService.GetOrderDetailAsync(orderCode);
            return Ok(JsonConvert.DeserializeObject(result));
        }
        [HttpPost("calculate-fee-byId")]
        public async Task<IActionResult> CalculateShippingFee([FromBody] ShippingFeeRequest request)
        {
            try
            {
                // Gọi phương thức CalculateFeeByDestinationAsync từ GhnService
                var response = await _ghnService.CalculateFeeByDestinationAutoAsync(request.ToDistrictId, request.ToWardCode);

                // Trả về kết quả từ API GHN
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tính phí vận chuyển", error = ex.Message });
            }
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts([FromQuery] int province_id)
        {
            var districts = await _ghnService.GetDistrictsByProvinceAsync(province_id);
            return Ok(districts);
        }

        [HttpGet("district-id")]
        public async Task<IActionResult> GetDistrictIdByName([FromQuery] string districtName)
        {
            var districtId = await _ghnService.GetDistrictIdByNameInDaNangAsync(districtName);

            if (districtId == null)
                return NotFound("Không tìm thấy quận trong TP. Đà Nẵng.");

            return Ok(new { districtId });
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _ghnService.GetProvincesAsync();

            if (provinces == null || !provinces.Any())
                return NotFound("Không tìm thấy danh sách tỉnh/thành.");

            return Ok(provinces);
        }

        [HttpGet("wards")]
        public async Task<IActionResult> GetWards([FromQuery] int districtId)
        {
            var wards = await _ghnService.GetWardsByDistrictIdAsync(districtId);

            if (wards == null || !wards.Any())
                return NotFound("Không tìm thấy phường nào.");

            return Ok(wards);
        }
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupDistrictAndWard([FromQuery] string districtName, [FromQuery] string wardName)
        {
            var (districtId, wardCode) = await _ghnService.GetDistrictAndWardAsync(districtName, wardName);

            if (districtId == null || string.IsNullOrEmpty(wardCode))
                return NotFound("Không tìm thấy quận hoặc phường.");

            var response = await _ghnService.CalculateFeeByDestinationAutoAsync((int)districtId, wardCode);

            // Trả về kết quả từ API GHN
            return Ok(response);

            //return Ok(new { districtId, wardCode });
        }

        [HttpGet("ReturnId")]
        public async Task<IActionResult> ReturnDistrictAndWard([FromQuery] string districtName, [FromQuery] string wardName)
        {
            var (districtId, wardCode) = await _ghnService.GetDistrictAndWardAsync(districtName, wardName);

            if (districtId == null || string.IsNullOrEmpty(wardCode))
                return NotFound("Không tìm thấy quận hoặc phường.");

            

            

            return Ok(new { districtId, wardCode });
        }

        [HttpPost("cancel-order")]
        public async Task<IActionResult> CancelOrder([FromBody] List<string> orderCodes)
        {
            if (orderCodes == null || orderCodes.Count == 0)
                return BadRequest("Danh sách order code không được rỗng.");

            try
            {
                var result = await _ghnService.CancelOrdersAsync(orderCodes);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
        [HttpPost("create-order-byRequest")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _ghnService.CreateOrderByDetailsAsync(
                    request.ToName,
                    request.ToPhone,
                    request.ToAddress,
                    request.ToWardCode,
                    request.ToDistrictId,
                    request.ProductNames
                );

                // Optionally deserialize the response to check for success or specific fields
                var responseObject = JsonConvert.DeserializeObject<dynamic>(response);
                if (responseObject.code == 200) // Assuming GHN API returns a 'code' field for success
                {
                    return Ok(responseObject);
                }
                else
                {
                    return BadRequest(new { message = "Failed to create order", details = responseObject.message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

    }

}

public class ShippingFeeRequest
{
    public int ToDistrictId { get; set; }
    public string ToWardCode { get; set; }
}
 public class CalculateFeeRequest
    {
        public string ToDistrictName { get; set; }
        public string ToWardName { get; set; }
    }

public class CreateOrderRequest
{
    public string ToName { get; set; }
    public string ToPhone { get; set; }
    public string ToAddress { get; set; }
    public string ToWardCode { get; set; }
    public int ToDistrictId { get; set; }
    public List<ProductGHN> ProductNames { get; set; }
}

