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

        /// <summary>Create a new admin user</summary>
        /// <remarks>POST request that creates a new admin user with inputted information.</remarks>
        [HttpPost("admin/create")]
        public async Task<ActionResult> CreateAdmin(AuthenticateRequest model)
        {
            var response = await _userService.CreateAdminUserAsync(model);

            if (response == null)
                return StatusCode(500, "User could not be created");

            return Ok(response);
        }

        /// <summary>Authenticate an admin user</summary>
        /// <remarks>POST request that authenticates an admin user with inputted information. If valid, returns a token to be used to authenticate future requests.</remarks>
        [HttpPost("admin/authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> AuthenticateAdmin(AuthenticateRequest model)
        {
            var response = await _userService.AuthenticateAdminAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        /// <summary>Authenticate a user (professor)</summary>
        /// <remarks>POST request that authenticates a user with inputted information. If valid, returns a token to be used to authenticate future requests.</remarks>
        [HttpPost("users/authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> AuthenticateUser(AuthenticateRequest model)
        {
            var response = await _userService.AuthenticateUserAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }
    }
}
