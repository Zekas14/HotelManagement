using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HotelManagement.Features.AuthManagement.Register
{
    public record RegisterCommand(string FullName, string Username, string Email, string PhoneNumber, string Password) : IRequest<RequestResult<bool>>;

    public class RegisterHandler(IGenericRepository<User> userRepository, IGenericRepository<Guest> guestRepository, IGenericRepository<Role> roleRepository, IGenericRepository<UserRole> userRoleRepository) : IRequestHandler<RegisterCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.Get(u => u.Email == request.Email || u.Username == request.Username)
                                                   .FirstOrDefaultAsync(cancellationToken);
            if (existingUser != null)
                return RequestResult<bool>.Failure(ErrorCode.BadRequest, "User with this email or username already exists.");

            var guest = new Guest
            {
                FullName = request.FullName,
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            guestRepository.Add(guest);
            await guestRepository.SaveChangesAsync();

            var guestRole = await roleRepository.Get(r => r.Name == "Guest").FirstOrDefaultAsync(cancellationToken);
            if (guestRole != null)
            {
                userRoleRepository.Add(new UserRole
                {
                    UserId = guest.Id,
                    RoleId = guestRole.Id,
                    CreatedAt = DateTime.UtcNow
                });
                await userRoleRepository.SaveChangesAsync();
            }

            return RequestResult<bool>.Success(true, "Registration successful.");
        }
    }
}
