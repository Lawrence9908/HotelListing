using HotelListing.API.Models.Account;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AccountController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var errors = await _authManager.Register(registerRequest);
            if(errors.Any()) { 
                foreach(var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                };

                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var authReponse= await _authManager.Login(loginRequest);
            if (authReponse == null)
            {
                return Unauthorized();
            }

            return Ok(authReponse);
        }



        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authReponse = await _authManager.VerifyRefreshToken(request);
            if (authReponse == null)
            {
                return Unauthorized();
            }

            return Ok(authReponse);
        }
    }
}
