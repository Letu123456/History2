using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using Business.DTO;
using Business.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

public class GhnService
{
    private readonly HttpClient _httpClient;
    private readonly GhnSettings _settings;

    public GhnService(HttpClient httpClient, IOptions<GhnSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri("https://online-gateway.ghn.vn/shiip/public-api/");
        SetDefaultHeaders();
    }

    private void SetDefaultHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Token", _settings.Token);
        _httpClient.DefaultRequestHeaders.Add("ShopId", _settings.ShopId);
    }

    private StringContent SerializePayload(object payload)
    {
        return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
    }

    public async Task<string> CalculateFeeAsync(object payload)
    {
        SetDefaultHeaders();
        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/fee", content);
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> CreateOrderAsync(object payload)
    {
        SetDefaultHeaders();
        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/create", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<GhnProvince>> GetProvincesAsync()
    {
        SetDefaultHeaders();
        var res = await _httpClient.GetAsync("master-data/province");
        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<dynamic>(json);
        return JsonConvert.DeserializeObject<List<GhnProvince>>(obj.data.ToString());
    }

    public async Task<List<GhnDistrict>> GetDistrictsByProvinceAsync(int provinceId)
    {
        SetDefaultHeaders();
        var res = await _httpClient.GetAsync($"master-data/district?province_id={provinceId}");
        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<dynamic>(json);
        return JsonConvert.DeserializeObject<List<GhnDistrict>>(obj.data.ToString());
    }

    public async Task<List<Business.DTO.GhnWard>> GetWardsByDistrictAsync(int districtId)
    {
        SetDefaultHeaders();
        var res = await _httpClient.GetAsync($"master-data/ward?district_id={districtId}");
        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<dynamic>(json);
        return JsonConvert.DeserializeObject<List<Business.DTO.GhnWard>>(obj.data.ToString());
    }

    public async Task<string> GetAllOrdersAsync(OrderListRequest request)
    {
        SetDefaultHeaders();
        var content = SerializePayload(request);
        var response = await _httpClient.PostAsync("v2/shipping-order/list", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetOrderDetailAsync(string orderCode)
    {
        SetDefaultHeaders();
        var payload = new { order_code = orderCode };
        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/detail", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetOrderDetailByClientCodeAsync(string clientOrderCode)
    {
        SetDefaultHeaders();
        var payload = new { client_order_code = clientOrderCode };
        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/detail-by-client-code", content);
        return await response.Content.ReadAsStringAsync();
    }
    public async Task<string> GetAvailableServicesAsync(int fromDistrictId, int toDistrictId)
    {
        SetDefaultHeaders();
        var payload = new
        {
            from_district = fromDistrictId,
            to_district = toDistrictId
        };
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("v2/shipping-order/available-services", content);
        return await response.Content.ReadAsStringAsync();
    }


   
    public async Task<string> CalculateFeeByDestinationAutoAsync(int toDistrictId, string toWardCode)
    {
        SetDefaultHeaders();

        const int fromDistrictId = 1529;         // Ví dụ: Ngũ Hành Sơn – Đà Nẵng
        const string fromWardCode = "40404";     // Phường Mỹ An – mã GHN
        int shopId = int.Parse(_settings.ShopId);

        // 1. Lấy danh sách dịch vụ khả dụng
        var services = await GetAvailableServiceAsync(fromDistrictId, toDistrictId);

        if (services == null || !services.Any())
            throw new Exception("Không tìm thấy dịch vụ giao hàng phù hợp giữa hai quận.");

        // 2. Ưu tiên GHN Express (service_type_id = 1), nếu không thì chọn cái đầu tiên
        var selectedService = services.FirstOrDefault(s => s.ServiceTypeId == 1) ?? services.First();

        var payload = new
        {
            shop_id = shopId,
            from_district_id = fromDistrictId,
            from_ward_code = fromWardCode,

            to_district_id = toDistrictId,
            to_ward_code = toWardCode,

            service_id = selectedService.ServiceId,
            service_type_id = selectedService.ServiceTypeId,

            height = 15,
            length = 20,
            weight = 1500,
            width = 15,

            insurance_value = 10000,
            cod_failed_amount = 2000,
            coupon = (string)null,

            items = new[]
            {
            new
            {
                name = "TEST1",
                quantity = 1,
                height = 15,
                weight = 1500,
                length = 20,
                width = 15
            }
        }
        };

        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/fee", content);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"GHN Fee API lỗi: {response.StatusCode} - {result}");

        return result;
    }


    public async Task<List<GhnDistrict>> GetDistrictsByProvinceIdAsync(int provinceId)
    {
        SetDefaultHeaders();

        var response = await _httpClient.GetAsync($"master-data/district?province_id={provinceId}");
        var json = await response.Content.ReadAsStringAsync();

        var obj = JsonConvert.DeserializeObject<dynamic>(json);
        var dataJson = obj?.data?.ToString();

        return JsonConvert.DeserializeObject<List<GhnDistrict>>(dataJson);
    }
    public async Task<int?> GetDistrictIdByNameInDaNangAsync(string districtName)
    {
        SetDefaultHeaders();

        const int daNangProvinceId = 201; // Thành phố Đà Nẵng

        var response = await _httpClient.GetAsync($"master-data/district?province_id={daNangProvinceId}");
        var json = await response.Content.ReadAsStringAsync();

        // Deserialize trực tiếp với model
        var apiResponse = JsonConvert.DeserializeObject<GhnApiResponse<List<GhnDistrict>>>(json);

        if (apiResponse?.Data == null || !apiResponse.Data.Any())
            return null;

        var matchedDistrict = apiResponse.Data.FirstOrDefault(d =>
            !string.IsNullOrEmpty(d.DistrictName) &&
            d.DistrictName.Contains(districtName, StringComparison.OrdinalIgnoreCase));

        return matchedDistrict?.DistrictID;
    }
    public async Task<List<Business.DTO.GhnWard>> GetWardsByDistrictIdAsync(int districtId)
    {
        SetDefaultHeaders();

        var payload = new { district_id = districtId };
        var content = SerializePayload(payload);

        var response = await _httpClient.PostAsync("master-data/ward", content);
        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<GhnApiResponse<List<Business.DTO.GhnWard>>>(json);
        return apiResponse?.Data ?? new List<Business.DTO.GhnWard>();
    }

    public async Task<(int? DistrictId, string WardCode)> GetDistrictAndWardAsync(string districtName, string wardName)
    {
        SetDefaultHeaders();

        // B1: Lấy tất cả tỉnh
        var provinces = await GetProvincesAsync();

        foreach (var province in provinces)
        {
            // B2: Lấy tất cả quận trong tỉnh
            var districts = await GetDistrictsByProvinceAsync(province.ProvinceID);
            var matchedDistrict = districts.FirstOrDefault(d =>
                !string.IsNullOrEmpty(d.DistrictName) &&
                d.DistrictName.Contains(districtName, StringComparison.OrdinalIgnoreCase));

            if (matchedDistrict != null)
            {
                int districtId = matchedDistrict.DistrictID;

                // B3: Lấy phường trong quận đã khớp
                var wards = await GetWardsByDistrictIdAsync(districtId);
                var matchedWard = wards.FirstOrDefault(w =>
                    !string.IsNullOrEmpty(w.WardName) &&
                    w.WardName.Contains(wardName, StringComparison.OrdinalIgnoreCase));

                if (matchedWard != null)
                    return (districtId, matchedWard.WardCode);
            }
        }

        // Không tìm thấy
        return (null, null);
    }
    public async Task<List<GhnServiceInfor>> GetAvailableServiceAsync(int fromDistrictId, int toDistrictId)
    {
        SetDefaultHeaders();

        var payload = new
        {
            shop_id = int.Parse(_settings.ShopId), // hoặc truyền trực tiếp nếu ShopId là số
            from_district = fromDistrictId,
            to_district = toDistrictId
        };

        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/available-services", content);
        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<GhnApiResponse<List<GhnServiceInfor>>>(json);
        return apiResponse?.Data ?? new List<GhnServiceInfor>();
    }
    
    public async Task<string> CreateOrderByDetailsAsync(string toName, string toPhone, string toAddress, string toWardCode, int toDistrictId, List<ProductGHN> productItems)
    {
        SetDefaultHeaders();

        var items = productItems.Select(item => new
        {
            name = item.Name,
            code = $"{item.Name}123", // Generate a simple code based on product name
            quantity = item.Quantity,
            price = item.Price,
            length = 20,
            width = 15,
            height = 15,
            weight = 1500,
            category = new
            {
                level1 = "Quà lưu niệm"
            }
        }).ToArray();

        // Calculate total cod_amount based on price * quantity for each item
        int codAmount = productItems.Sum(item => item.Price * item.Quantity);

        var payload = new
        {
            payment_type_id = 2,
            note = "Tintest 123",
            required_note = "KHONGCHOXEMHANG",
            from_name = "VNMU",
            from_phone = "0987654321",
            from_address = "44 An Thượng 14, Phường Mỹ An, Quận Ngũ Hành Sơn, Đà Nẵng, Vietnam",
            from_ward_name = "Phường Mỹ An",
            from_district_name = "Quận Ngũ Hành Sơn",
            from_province_name = "Đà Nẵng",
            return_phone = "0917407175",
            return_address = "90 Huỳnh Thúc Kháng, Thị Trấn Hà Lam, Huyện Thăng Bình, Quảng Nam, Vietnam",
            return_district_id = 2239,
            return_ward_code = "340801",
            client_order_code = "sfbdfb",
            to_name = toName,
            to_phone = toPhone,
            to_address = toAddress,
            to_ward_code = toWardCode,
            to_district_id = toDistrictId,
            cod_amount = codAmount,
            content = "Theo New York Times",
            weight = 1500,
            length = 20,
            width = 15,
            height = 15,
            pick_station_id = 1444,
            deliver_station_id = (int?)null,
            insurance_value = 100000,
            service_id = 0,
            service_type_id = 2,
            coupon = (string)null,
            pick_shift = new[] { 2 },
            items = items
        };

        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/shipping-order/create", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CancelOrdersAsync(List<string> orderCodes)
    {
        SetDefaultHeaders();

        var payload = new
        {
            order_codes = orderCodes
        };

        var content = SerializePayload(payload);
        var response = await _httpClient.PostAsync("v2/switch-status/cancel", content);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Lỗi khi hủy đơn hàng: {response.StatusCode} - {result}");

        return result;
    }

}
