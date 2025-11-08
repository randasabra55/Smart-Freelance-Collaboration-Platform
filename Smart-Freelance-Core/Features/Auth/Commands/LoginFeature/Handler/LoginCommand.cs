using MediatR;
using Smart_Freelance_Data.Results;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;


namespace Smart_Freelance_Core.Features.Auth.Commands.LoginFeature.Handler;

//[Endpoint(EndpointMethod.Post, EndpointTag.Auth, "Login")]
public record LoginCommand(string Email, string Password) : IRequest<Result<UserSessionDto>>;
public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<UserSessionDto>>
{

    public async Task<Result<UserSessionDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.SignIn(request.Email, request.Password);
        return result;
    }
}
