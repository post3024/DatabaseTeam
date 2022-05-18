// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class AuthenticateResponse
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string token { get; set; }
        public string user_role { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            username = user.user_email;
            first_name = user.first_name;
            last_name = user.last_name;
            user_id = user.user_id;
            this.token = token;
            user_role = user.user_role;
        }

        public AuthenticateResponse(ProfUser user, string token)
        {
            username = user.user_email;
            first_name = user.first_name;
            last_name = user.last_name;
            user_id = user.professor_id;
            this.token = token;
            user_role = user.user_role;
        }
    }
}