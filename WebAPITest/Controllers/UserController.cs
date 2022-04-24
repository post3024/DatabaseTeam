using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPITest.Models;
using WebAPITest.Services;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Controllers
{
    [ApiController]
    [Route("user-management")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Authenticate a user</summary>
        /// <remarks>POST request that authenticates a user with inputted information. If valid, returns a token to be used to authenticate future requests.</remarks>
        [HttpPost("users/authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.AuthenticateAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        /// <summary>Create a new user</summary>
        /// <remarks>POST request that creates a new user with inputted information.</remarks>
        [HttpPost("users/create")]
        public async Task<ActionResult> CreateUser(AuthenticateRequest model)
        {
            var response = await _userService.CreateUserAsync(model);

            if (response == null)
                return StatusCode(500, "User could not be created");

            return Ok(response);
        }
    }
}
