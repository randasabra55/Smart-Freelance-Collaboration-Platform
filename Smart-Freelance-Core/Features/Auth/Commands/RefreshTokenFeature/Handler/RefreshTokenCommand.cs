

using MediatR;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Features.Auth.Commands.RefreshTokenFeature.Handler;


//[Endpoint(EndpointMethod.Patch, EndpointTag.Auth, "RefreshToken")]
public record RefreshTokenCommand(string accessToken) : IRequest<Result<UserSessionDto>>;
public class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, Result<UserSessionDto>>
{

    public async Task<Result<UserSessionDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RefreshTokenAsync(request.accessToken);
    }
}
