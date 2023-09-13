using HotelListing.API.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Repository.IRepostitory
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(RegisterRequest registerRequest);
        Task<AuthResponseDto> Login(LoginRequest loginRequest);
        Task<string> CreateRefreshToken();
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}
