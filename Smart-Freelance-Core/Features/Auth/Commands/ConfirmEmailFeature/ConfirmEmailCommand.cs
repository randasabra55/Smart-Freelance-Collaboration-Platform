using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Features.Auth.Commands.ConfirmEmailFeature;

//[Endpoint(EndpointMethod.Patch, EndpointTag.Email, "ConfirmEmail")]
public record ConfirmEmailCommand(string Email, string VerificationCode) : IRequest<Result>;
public class ConfirmEmailCommandHandler(
    IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await emailConfirmationService.ConfirmEmail(request.Email, request.VerificationCode);
    }
}
