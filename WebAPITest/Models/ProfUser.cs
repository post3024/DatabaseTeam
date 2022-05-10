using System.Text.Json.Serialization;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class ProfUser
    {
        public int professor_id { get; set; }
        public string user_email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string salt { get; set; }
        public string user_role { get; set; }

        [JsonIgnore]
        public string user_password { get; set; }
    }
}
