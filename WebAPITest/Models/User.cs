using System.Text.Json.Serialization;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string user_email { get; set; }
        public string salt { get; set; }

        [JsonIgnore]
        public string user_password { get; set; }
    }
}