using Smart_Freelance_Core.Features.Auth.Dto;
using Smart_Freelance_Data.Entities.Identity;

namespace Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Extensions;
public static class UserExtensions
{
    public static ApplicationUser ToEntity(this CreateUserRequest userRequest) => new ApplicationUser
    {
        FullName = userRequest.FullName,
        Email = userRequest.Email,
        UserName = userRequest.Email,
        Role = userRequest.Role,
        Specialization = userRequest.Specialization,
        PhoneNumber = userRequest.PhoneNumber,
        CreatedAt = DateTime.UtcNow,
    };
}
