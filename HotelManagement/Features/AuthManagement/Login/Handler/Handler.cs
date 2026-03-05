using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HotelManagement.Features.AuthManagement.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<RequestResult<string>>;

    public class LoginHandler(IGenericRepository<User> userRepository, IGenericRepository<UserRole> userRolesRepository, IJwtService jwtService) : IRequestHandler<LoginCommand, RequestResult<string>>
    {
        public async Task<RequestResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.Get(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return RequestResult<string>.Failure(ErrorCode.UnAuthorized, "Invalid credentials.");

            var userRole = await userRolesRepository.Get(ur => ur.UserId == user.Id)
                                                    .Include(ur => ur.Role)
                                                    .FirstOrDefaultAsync(cancellationToken);

            string roleName = userRole?.Role?.Name ?? "Guest";
            string token = jwtService.GenerateToken(user, roleName);

            return RequestResult<string>.Success(token, "Login successful.");
        }
    }
}
