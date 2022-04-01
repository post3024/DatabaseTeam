// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Username = user.username;
            Token = token;
        }
    }
}