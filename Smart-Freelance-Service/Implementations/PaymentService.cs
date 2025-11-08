/*using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Data;
using System.Text;

namespace Smart_Freelance_Service.Implementations
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly Context _context;

        public PaymentService(HttpClient httpClient, IConfiguration config, Context context)
        {
            _httpClient = httpClient;
            _config = config;
            _context = context;
        }

        public async Task<string> CreateDepositSessionAsync(Project project, long clientId)
        {
            var apiKey = _config["Paymob:ApiKey"];
            var integrationId = _config["Paymob:IntegrationId"];
            var iframeId = _config["Paymob:IframeId"];
            var baseUrl = _config["Paymob:BaseUrl"];

            // 1️⃣ Authentication
            var authResponse = await _httpClient.PostAsync($"{baseUrl}/auth/tokens",
                new StringContent(JsonConvert.SerializeObject(new { api_key = apiKey }),
                Encoding.UTF8, "application/json"));
            var authData = JsonConvert.DeserializeObject<dynamic>(
                await authResponse.Content.ReadAsStringAsync());
            string token = authData.token;

            // 2️⃣ Create Order
            decimal depositAmount = project.Budget * 0.2m * 100; // convert to piasters
            var orderPayload = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = depositAmount,
                currency = "EGP",
                items = new object[] { },
            };

            var orderResponse = await _httpClient.PostAsync($"{baseUrl}/ecommerce/orders",
                new StringContent(JsonConvert.SerializeObject(orderPayload),
                Encoding.UTF8, "application/json"));
            var orderData = JsonConvert.DeserializeObject<dynamic>(
                await orderResponse.Content.ReadAsStringAsync());
            string orderId = orderData.id;

            // 3️⃣ Generate Payment Key
            var paymentKeyPayload = new
            {
                auth_token = token,
                amount_cents = depositAmount,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "N/A",
                    email = "client@mail.com",
                    floor = "N/A",
                    first_name = "Client",
                    street = "N/A",
                    building = "N/A",
                    phone_number = "0123456789",
                    shipping_method = "N/A",
                    postal_code = "00000",
                    city = "Assiut",
                    country = "EG",
                    last_name = "Client"
                },
                currency = "EGP",
                integration_id = integrationId
            };

            var paymentKeyResponse = await _httpClient.PostAsync($"{baseUrl}/acceptance/payment_keys",
                new StringContent(JsonConvert.SerializeObject(paymentKeyPayload),
                Encoding.UTF8, "application/json"));
            var paymentKeyData = JsonConvert.DeserializeObject<dynamic>(
                await paymentKeyResponse.Content.ReadAsStringAsync());
            string paymentKey = paymentKeyData.token;

            // 4️⃣ Save Transaction locally
            var transaction = new Transaction
            {
                ProjectId = project.Id,
                UserId = clientId,
                Amount = project.Budget * 0.2m,
                PaymentGatewayTransactionId = orderId,
                PaymentKey = paymentKey,
                Type = TransactionType.Deposit
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // 5️⃣ Return Pay URL
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";
        }
    }
}
*/