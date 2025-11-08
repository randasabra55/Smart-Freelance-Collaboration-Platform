using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Features.Auth.Commands.LogoutFeature;

//[Endpoint(EndpointMethod.Get, EndpointTag.Auth, "Logout")]
public record LogoutCommand : IRequest<Result>
{
}
public class LogoutCommandHandler(
    IIdentityService identityService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await identityService.SignOut();
    }
}
