using HotelListing.API.Entity;
using HotelListing.API.Models.Account;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
         private readonly ILogger  _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            _logger.LogInformation("User Register in start");
            _logger.LogError($"Register request: {registerRequest.Email}");
            try
            {
                var errors = await _authManager.Register(registerRequest);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    };

                    return BadRequest(ModelState);
                }

                return Ok();
            }catch(Exception ex)
            {
                _logger.LogError(ex,$"Something Went wrong in the: {nameof(Register)} - User Registration attempt for {registerRequest.Email}");
                return Problem($"Something went wrong in the  {nameof(Register)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation($"Login Attempt for {loginRequest.Email}");
            try
            {
                var authReponse = await _authManager.Login(loginRequest);
                if (authReponse == null)
                {
                    return Unauthorized();
                }

                return Ok(authReponse);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,$"Something Went wrong in the  {nameof(Login)}");
                return Problem($"Something Went Wrong in the  {nameof(Login)}", statusCode: 500);
            }
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
