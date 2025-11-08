using MediatR;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Passwords.Commands.VerifyOTP
{
    //[Endpoint(EndpointMethod.Post, EndpointTag.Password, "VerifyOTP")]
    public record verifyOTPCommand(string Email, string OTP) : IRequest<Result>;

    public class VerifyOtpCommandHandler(IPasswordService passwordService) : IRequestHandler<verifyOTPCommand, Result>
    {
        public async Task<Result> Handle(verifyOTPCommand request, CancellationToken cancellationToken)
        {
            return await passwordService.VerifyOtpAsync(request.Email, request.OTP, cancellationToken);
        }
    }
}
