using MediatR;
using Microsoft.Extensions.Logging;
using Smart_Freelance_Core.Features.Payment.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Payment.Queries
{
    // [Endpoint(EndpointMethod.Get, EndpointTag.Payment, "GetPaymentIframe")]
    public record GetPaymentIframeQuery(long TransId) : IRequest<Result<GetPaymentIframeResponseDto>>;


    public class GetPaymentIframeQueryHandler(
        Context context,
        ILoggerFactory loggerFactory)
        : IRequestHandler<GetPaymentIframeQuery, Result<GetPaymentIframeResponseDto>>
    {
        private readonly Context _context = context;
        private readonly ILogger _logger = loggerFactory.CreateLogger("PaymentIframe");

        public async Task<Result<GetPaymentIframeResponseDto>> Handle(GetPaymentIframeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(request.TransId, cancellationToken);
                if (transaction == null || transaction.Status != TransactionStatus.Pending)
                {
                    _logger.LogWarning($"Invalid transaction {request.TransId}");
                    return Result.Failure<GetPaymentIframeResponseDto>(new Error(ErrorCode.InvalidInput, "Transaction not found or already processed"));
                }

                var iframeUrl = transaction.PaymentKey; // الـ URL اللي اتولد في الـ Consumer

                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>إكمال الدفع</title>
                    <style> body {{ font-family: Arial; text-align: center; }} iframe {{ width: 100%; height: 600px; border: none; }} </style>
                </head>
                <body>
                    <h2>يرجى إكمال الدفع الأولي</h2>
                    <p>المبلغ: {transaction.Amount} {transaction.Currency}</p>
                    <iframe src=""{iframeUrl}""></iframe>
                    <p>بعد الدفع، ستُحوّل تلقائياً.</p>
                </body>
                </html>";

                _logger.LogInformation($"Iframe loaded for Transaction {request.TransId}");

                var response = new GetPaymentIframeResponseDto
                {
                    Html = html
                };

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Iframe error: {ex.Message}");
                return Result.Failure<GetPaymentIframeResponseDto>(new Error(ErrorCode.BadRequest, $"Error: {ex.Message}"));
            }
        }
    }
}
