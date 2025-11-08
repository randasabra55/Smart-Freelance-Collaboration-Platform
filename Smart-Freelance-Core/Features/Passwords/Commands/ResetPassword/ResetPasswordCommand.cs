using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;


namespace Smart_Freelance_Core.Passwords.Commands.ResetPassword;

//[Authorize]
//[Endpoint(EndpointMethod.Patch, EndpointTag.Password, "ResetPassword")]
public record ResetPasswordCommand(
    string Email,
    string OTP,
    string NewPassword
) : IRequest<Result>;

public class ResetPasswordCommandHandler(IPasswordService passwordService) : IRequestHandler<ResetPasswordCommand, Result>
{
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
     => passwordService.ResetPasswordAsync(request.Email, request.OTP, request.NewPassword, cancellationToken);

}
