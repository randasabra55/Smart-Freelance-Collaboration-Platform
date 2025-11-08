using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Data.Helpers;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Services;
using System.Text;
using X.Paymob.CashIn;
using X.Paymob.CashIn.Models.Orders;
using X.Paymob.CashIn.Models.Payment;

namespace Smart_Freelance_Service.Consumer
{
    public class PaymentConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _rabbitSettings;
        //private readonly Context _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<PaymentConsumer> _logger;
        // private readonly AuditLoggerService _auditLogger;
        private readonly IPaymobCashInBroker _paymobBroker;
        private readonly IConfiguration _configuration;

        public PaymentConsumer(
            RabbitMQSettings rabbitSettings,
            //Context context,
            IServiceScopeFactory scopeFactory,
            IHubContext<NotificationHub> hubContext,
            ILogger<PaymentConsumer> logger,
            // AuditLoggerService auditLogger,
            IPaymobCashInBroker paymobBroker,
            IConfiguration configuration)
        {
            _rabbitSettings = rabbitSettings;
            _scopeFactory = scopeFactory;
            // _context = context;
            _hubContext = hubContext;
            _logger = logger;
            // _auditLogger = auditLogger;
            _paymobBroker = paymobBroker;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitSettings.Host,
                UserName = _rabbitSettings.UserName,
                Password = _rabbitSettings.Password,
                VirtualHost = "/",
                Port = 5672

            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "InitialPaymentQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                var auditLogger = scope.ServiceProvider.GetRequiredService<AuditLoggerService>();
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonConvert.DeserializeObject<InitialPaymentMessage>(json);
                    if (message == null) return;

                    // add tractation in database with (pennding status)
                    var transaction = new Transaction
                    {
                        UserId = message.ClientId,
                        ProjectId = message.ProjectId,
                        Type = TransactionType.Deposit,
                        Currency = message.Currency,
                        Amount = message.Amount,
                        Status = TransactionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.Transactions.AddAsync(transaction);
                    await context.SaveChangesAsync();

                    await auditLogger.LogAsync("Initial payment initiated", "Transaction", transaction.Id);
                    _logger.LogInformation($"[PaymentConsumer] Transaction {transaction.Id} created for Project {message.ProjectId}");

                    //go to Paymob 
                    await InitiatePaymobPayment(context, transaction, message);

                    // send link of payment to client to pay
                    var paymentUrl = $"/payment/iframe?transId={transaction.Id}";
                    var notificationData = new
                    {
                        Message = $"Please complete the initial payment of {message.Amount} {message.Currency} to start collaboration.",
                        TransactionId = transaction.Id,
                        PaymentUrl = paymentUrl
                    };
                    await _hubContext.Clients.User(message.ClientId.ToString())
                        .SendAsync("InitialPaymentRequired", notificationData);

                    _logger.LogInformation($"[PaymentConsumer] Payment initiated for Project {message.ProjectId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[PaymentConsumer] Error: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: "InitialPaymentQueue", autoAck: true, consumer: consumer);
        }

        private async Task InitiatePaymobPayment(Context context, Transaction transaction, InitialPaymentMessage message)
        {
            try
            {
                // جيب بيانات العميل
                var client = await context.Users.FindAsync(message.ClientId);
                if (client == null)
                {
                    _logger.LogError("Client not found");
                    return;
                }

                // 1. أنشئ Order في Paymob (المبلغ بالـ cents)
                var amountCents = (int)(transaction.Amount * 100);
                var orderRequest = CashInCreateOrderRequest.CreateOrder(amountCents);
                var orderResponse = await _paymobBroker.CreateOrderAsync(orderRequest);

                // 2. أنشئ Billing Data
                var billingData = new CashInBillingData(
                    firstName: client.FullName ?? "",
                    lastName: client.FullName ?? "",
                    phoneNumber: client.PhoneNumber ?? "",
                    email: client.Email ?? "");

                // 3. Request Payment Key
                var integrationId = int.Parse(_configuration["Paymob:IntegrationId"] ?? "0");
                var paymentKeyRequest = new CashInPaymentKeyRequest(
                    integrationId: integrationId,
                    orderId: orderResponse.Id,
                    billingData: billingData,
                    amountCents: amountCents);
                var paymentKeyResponse = await _paymobBroker.RequestPaymentKeyAsync(paymentKeyRequest);

                // 4. بني URL للـ Iframe
                var iframeId = int.Parse(_configuration["Paymob:IframeId"] ?? "0");
                var iframeUrl = _paymobBroker.CreateIframeSrc(iframeId.ToString(), paymentKeyResponse.PaymentKey);

                // 5. خزن في Transaction
                transaction.PaymentGatewayTransactionId = orderResponse.Id.ToString();
                transaction.PaymentKey = iframeUrl; // أو PaymentKey = paymentKeyResponse.PaymentKey 
                await context.SaveChangesAsync();

                _logger.LogInformation($"[Paymob] Iframe URL generated: {iframeUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Paymob] Error: {ex.Message}");
                transaction.Status = TransactionStatus.Failed;
                await context.SaveChangesAsync();
            }
        }

        private class InitialPaymentMessage
        {
            public long ProjectId { get; set; }
            public long ClientId { get; set; }
            public long FreelancerId { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "EGP";
        }
    }

}









/*public class PaymentConsumer : BackgroundService
   {
       private readonly RabbitMQSettings _rabbitSettings;
       private readonly Context _context;
       private readonly IHubContext<NotificationHub> _hubContext;
       private readonly ILogger<PaymentConsumer> _logger;
       private readonly AuditLoggerService _auditLogger;
       private readonly IPaymobCashInBroker _paymobBroker; // من الـ SDK

       public PaymentConsumer(
           RabbitMQSettings rabbitSettings,
           Context context,
           IHubContext<NotificationHub> hubContext,
           ILogger<PaymentConsumer> logger,
           AuditLoggerService auditLogger,
           IPaymobCashInBroker paymobBroker) // حقن الـ broker
       {
           _rabbitSettings = rabbitSettings;
           _context = context;
           _hubContext = hubContext;
           _logger = logger;
           _auditLogger = auditLogger;
           _paymobBroker = paymobBroker;
       }

       protected override async Task ExecuteAsync(CancellationToken stoppingToken)
       {
           var factory = new ConnectionFactory()
           {
               HostName = _rabbitSettings.Host,
               UserName = _rabbitSettings.UserName,
               Password = _rabbitSettings.Password
           };
           var connection = await factory.CreateConnectionAsync();
           var channel = await connection.CreateChannelAsync();

           // Queue للدفع الأولي
           await channel.QueueDeclareAsync("InitialPaymentQueue", true, false, false, null);
           var consumer = new AsyncEventingBasicConsumer(channel);
           consumer.ReceivedAsync += async (sender, ea) =>
           {
               try
               {
                   var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                   var message = JsonConvert.DeserializeObject<InitialPaymentMessage>(json);
                   if (message == null) return;

                   // 1. أنشئ Transaction في DB (Pending)
                   var transaction = new Transaction
                   {
                       UserId = message.ClientId, // العميل اللي بيدفع
                       ProjectId = message.ProjectId,
                       Type = TransactionType.Deposit, // دفع أولي
                       Currency = message.Currency,
                       Amount = message.Amount,
                       Status = TransactionStatus.Pending,
                       CreatedAt = DateTime.UtcNow
                   };
                   await _context.Transactions.AddAsync(transaction);
                   await _context.SaveChangesAsync();
                   await _auditLogger.LogAsync("Initial payment initiated", "Transaction", transaction.Id);

                   // 2. ابدأ تدفق Paymob
                   await InitiatePaymobPayment(transaction, message);

                   // 3. أرسل payment key للعميل عبر SignalR أو redirect للـ iframe
                   // (هتعمل endpoint في Controller عشان يعرض الـ iframe)
                   var notificationData = new
                   {
                       Message = "Please complete the initial payment to start collaboration.",
                       TransactionId = transaction.Id,
                       PaymentUrl = $"/payment/iframe/{transaction.PaymentKey}" // هتعمله بعدين
                   };
                   await _hubContext.Clients.User(message.ClientId.ToString())
                       .SendAsync("InitialPaymentRequired", notificationData);

                   _logger.LogInformation($"Initial payment started for Project {message.ProjectId}");
               }
               catch (Exception ex)
               {
                   _logger.LogError($"Error in payment: {ex.Message}");
               }
           };
           await channel.BasicConsumeAsync("InitialPaymentQueue", true, consumer);
       }

       private async Task InitiatePaymobPayment(Transaction transaction, InitialPaymentMessage message)
       {
           // 3. أنشئ Order في Paymob
           var amountCents = (int)(transaction.Amount * 100); // Paymob بيستخدم cents
           var orderRequest = CashInCreateOrderRequest.CreateOrder(amountCents);
           var orderResponse = await _paymobBroker.CreateOrderAsync(orderRequest);

           // 4. أنشئ Billing Data (من user info)
           var client = await _context.Users.FindAsync(message.ClientId); // افترض ApplicationUser
           var billingData = new CashInBillingData(
               firstName: client.FullName?? "",
               lastName: client.FullName ?? "",
               phoneNumber: client.PhoneNumber ?? "",
               email: client.Email!);

           // 5. Request Payment Key
           var integrationId = 12345; // من config
           var paymentKeyRequest = new CashInPaymentKeyRequest(
               integrationId: integrationId,
               orderId: orderResponse.Id,
               billingData: billingData,
               amountCents: amountCents);
           var paymentKeyResponse = await _paymobBroker.RequestPaymentKeyAsync(paymentKeyRequest);

           // 6. خزن الـ Payment Key في Transaction
           transaction.PaymentKey = paymentKeyResponse.PaymentKey;
           transaction.PaymentGatewayTransactionId = orderResponse.Id.ToString();
           await _context.SaveChangesAsync();

           // 7. أنشئ URL للـ Iframe (هترجعه للعميل)
           var iframeUrl = _paymobBroker.CreateIframeSrc(
               iframeId: "67890", // من config
               token: paymentKeyResponse.PaymentKey);
           transaction.PaymentKey = iframeUrl; // أو خزنها في field جديد
           await _context.SaveChangesAsync();
       }

       // class InitialPaymentMessage زي اللي فوق
   }*/
