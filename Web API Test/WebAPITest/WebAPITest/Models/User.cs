using System.Text.Json.Serialization;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string role { get; set; }

        [JsonIgnore]
        public string password { get; set; }
    }
}