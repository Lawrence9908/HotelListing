using AutoMapper;
using HotelListing.API.Entity;
using HotelListing.API.Models.Account;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Repository.Implimentation
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AuthManager(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> Login(LoginRequest loginRequest)
        {

            bool isValidUser =false;

            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user is null)
                {
                    return default;
                }
                isValidUser = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                if (!isValidUser)
                {
                    return default;
                }
                
            }
            catch (Exception)
            {
                
            }
            return isValidUser;
        
        }

        public async Task<IEnumerable<IdentityError>> Register(RegisterRequest registerRequest)
        {
            var user = _mapper.Map<ApplicationUser>(registerRequest);
            user.UserName = registerRequest.Email;

            var result  = await _userManager.CreateAsync(user, registerRequest.Password);


            // IF EVERYTHING PASSED ADD THE ROLES
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            return result.Errors;
 
        }
    }
}
