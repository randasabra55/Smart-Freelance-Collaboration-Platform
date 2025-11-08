/*using MediatR;
using Microsoft.AspNetCore.Http;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Service.Implementations;

namespace Smart_Freelance_Core.Features.Payment.Commands
{
    //[Endpoint(EndpointMethod.Post, EndpointTag.Payment, "CreateDeposit")]
    public record CreateDepositCommand(long ProjectId) : IRequest<Result<string>>;

    public class CreateDepositCommandHandler : IRequestHandler<CreateDepositCommand, Result<string>>
    {
        private readonly PaymentService _paymentService;
        private readonly Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateDepositCommandHandler(PaymentService paymentService, Context context, IHttpContextAccessor accessor)
        {
            _paymentService = paymentService;
            _context = context;
            _httpContextAccessor = accessor;
        }

        public async Task<Result<string>> Handle(CreateDepositCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.projects.FindAsync(request.ProjectId);
            if (project == null)
                return Result.Failure<string>(new Error(ErrorCode.NotFound, "Project not found"));

            var clientId = long.Parse(_httpContextAccessor.HttpContext!.User.FindFirst("nameid")!.Value);

            var payUrl = await _paymentService.CreateDepositSessionAsync(project, clientId);
            return Result.Success(payUrl);
        }
    }
}
*/