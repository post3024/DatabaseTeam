using System.Text.Json.Serialization;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string user_email { get; set; }
        public string salt { get; set; }
        public string user_role { get; set; }

        [JsonIgnore]
        public string user_password { get; set; }

        public User(ProfUser user)
        {
            this.user_id = user.professor_id;
            this.user_email = user.user_email;
            this.salt = user.salt;
            this.user_password = user.user_password;
            this.user_role = user.user_role;
        }

        public User()
        {

        }
    }
}