using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;
using System.ComponentModel.DataAnnotations;


namespace Smart_Freelance_Core.Features.Auth.Commands.SendEmailConfirmationFeature.Handler;

//[Endpoint(EndpointMethod.Get, EndpointTag.Email, "SendConfirmationEmail")]
public record SendEmailConfirmationCommand : IRequest<Result>
{
    [EmailAddress]
    public required string Email { get; set; }
}
public class SendEmailConfirmationCommandHandler(
    IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<SendEmailConfirmationCommand, Result>
{
    public async Task<Result> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        return await emailConfirmationService.SendEmailConfirmation(request.Email);
    }
}

