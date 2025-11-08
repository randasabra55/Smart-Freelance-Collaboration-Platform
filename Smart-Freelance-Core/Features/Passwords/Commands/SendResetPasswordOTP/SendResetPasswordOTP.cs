
using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Passwords.Commands.ResetPassword;

//[Authorize]
//[Endpoint(EndpointMethod.Post, EndpointTag.Password, "SendResetPasswordOtp")]
public record SendResetPasswordOTP(string Email) : IRequest<Result>;

public class SendResetPasswordOTPHandler(IPasswordService passwordService) : IRequestHandler<SendResetPasswordOTP, Result>
{
    public Task<Result> Handle(SendResetPasswordOTP request, CancellationToken cancellationToken)
     => passwordService.CreatePasswordResetOtpAsync(email: request.Email);
}
