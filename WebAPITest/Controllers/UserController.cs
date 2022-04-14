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
        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }
    }
}
