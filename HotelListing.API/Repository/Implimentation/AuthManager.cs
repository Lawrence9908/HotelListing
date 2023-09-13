using AutoMapper;
using HotelListing.API.Entity;
using HotelListing.API.Models.Account;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Repository.Implimentation
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private ApplicationUser _user;
        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, "HotelListingApi", "RefreshToken");
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, "HotelLsitingApi", "RefreshToken");
            var result = await _userManager.SetAuthenticationTokenAsync(_user, "HotelLsitingApi", "RefreshToken", newRefreshToken);

            return newRefreshToken;
        }

        public async Task<AuthResponseDto> Login(LoginRequest loginRequest)
        {

            bool isValidUser =false;

            
           _user = await _userManager.FindByEmailAsync(loginRequest.Email);
           isValidUser = await _userManager.CheckPasswordAsync(_user, loginRequest.Password);
           if (_user is null || isValidUser ==false)
           {
               return default;
           }
            
           var token = await GenerateToken();

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        public async Task<IEnumerable<IdentityError>> Register(RegisterRequest registerRequest)
        {
            _user = _mapper.Map<ApplicationUser>(registerRequest);
            _user.UserName = registerRequest.Email;

            var result  = await _userManager.CreateAsync(_user, registerRequest.Password);


            // IF EVERYTHING PASSED ADD THE ROLES
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }
            return result.Errors;
 
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;

            _user  = await  _userManager.FindByNameAsync(username);
            if(_user == null || _user.Id != request.UserId) { return null;}
            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);
            if(isValidRefreshToken)
            {
                return null;
            }
            var token = await GenerateToken();
            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
            await _userManager.UpdateSecurityStampAsync(_user);
        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles  = await _userManager.GetRolesAsync(_user);
            var rolesCliams  = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims  = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
            }.Union(rolesCliams).Union(userClaims);


            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
