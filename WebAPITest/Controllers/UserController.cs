using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
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
        /// <remarks>POST request that creates a new admin user with inputted information. Only an existing admin can add another admin user.</remarks>
        [HttpPost("admin/create")]
        [Authorize("admin")]
        public async Task<ActionResult> CreateAdmin(CreateAdminDTO model)
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

        /// <summary>Change a user or admin's password</summary>
        /// <remarks>POST request that changes the user's password. User or admin must be logged in and send a valid token to be authorized for this endpoint.</remarks>
        [HttpPost("change-password")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> ChangePassword(string new_password)
        {
            var currentUser = (User)HttpContext.Items["User"];
            var response = await _userService.ChangeUserPasswordAsync(currentUser, new_password);
            if (response == null)
                return StatusCode(500, "Error: password could not be updated");

            return StatusCode(200, "Successfully updated password");
        }

        /// <summary>Admin or user forgot their password</summary>
        /// <remarks>POST request that changes the user's password to a new temporary string. User or admin sends in their email and if they exist as a current user, a new password will be generated and sent via email to the user.</remarks>
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string user_email)
        {
            User adminUser = await _userService.GetAdminUserByEmail(user_email);
            User profUser = await _userService.GetProfUserByEmail(user_email);
            if (adminUser == null && profUser == null)
            {
                return StatusCode(500, "Error: user does not exist");
            }
            else
            {
                User user;
                if (adminUser != null)
                {
                    user = adminUser;
                }
                else
                {
                    user = profUser;
                }
                // generate random 32-bit password
                var passwordStr = _userService.GeneratePassword(16, 1);
                var response = await _userService.ChangeUserPasswordAsync(user, passwordStr);
                if (response == null)
                {
                    return StatusCode(500, "Error: password could not be updated");
                }
                else
                {
                    // create email message
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("classyscheduleUST@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(user_email));
                    email.Subject = "ClassySchedule Forgotten Password";
                    string messageText = "Hello " + user.first_name + " " + user.last_name + ",\n\nForgot your password? " +
                        "Please use your St. Thomas email and this temporary password to login.\n\n" + "Password = " + passwordStr + "\n\nFeel free to change this password in your account settings.\n\nThank you,\nClassySchedule";
                    email.Body = new TextPart(TextFormat.Plain) { Text = messageText };

                    // send email
                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("classyscheduleUST@gmail.com", "@ryb5rgLQb7J");
                    smtp.Send(email);
                    smtp.Disconnect(true);

                    return StatusCode(200, "Successfully updated password");
                }
            }
        }
    }
}
