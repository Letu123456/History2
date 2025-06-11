


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Business.DTO;
using Business.Model;
using Net.payOS;
using Net.payOS.Types;
using Microsoft.AspNetCore.Mvc;

namespace DataAccess.Service
{
    public class PayOsService
    {
        private readonly HttpClient _httpClient;
        private readonly PayOS _payOS;
        private readonly PayOsSettings _settings;

        public PayOsService(PayOS payOS, IOptions<PayOsSettings> options, IHttpClientFactory factory)
        {
            _payOS = payOS;
            _settings = options.Value;
            _httpClient = factory.CreateClient();
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(int amount, List<ItemData> items, string description, string returnUrl, string cancelUrl, int orderCode)
        {
            var paymentData = new PaymentData(
                orderCode,
                amount,
                description,
                items,
                returnUrl,
                cancelUrl
            );

            var createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment;
        }

        public async Task<string> GetPaymentStatusAsync(string orderCode)
        {
            var requestUrl = $"https://api-merchant.payos.vn/v2/payment-requests/{orderCode}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);

            var response = await _httpClient.GetAsync(requestUrl);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Không thể kiểm tra trạng thái đơn hàng: {content}");

            return content;
        }
    }
}
