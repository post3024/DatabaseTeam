using System.Text.Json.Serialization;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    // this class handles the schema of the user table in the database
    public class User
    {
        public int user_id { get; set; }
        
        public string user_email { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string salt { get; set; }

        public string user_role { get; set; }

        [JsonIgnore]
        public string user_password { get; set; }
        
        // constructor
        public User(ProfUser prof)
        {
            user_id = prof.professor_id;
            user_email = prof.user_email;
            first_name = prof.first_name;
            last_name = prof.last_name;
            salt = prof.salt;
            user_role = prof.user_role;
            user_password = prof.user_password;
        }

        public User()
        {

        }
    }
}
