// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class AuthenticateResponse
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            username = user.user_email;
            user_id = user.user_id;
            this.token = token;
        }
    }
}