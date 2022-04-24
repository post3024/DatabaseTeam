using System;
namespace WebAPITest.Models
{
    public class CreateUserResponse
    {
        public int user_id { get; set; }
        public string username { get; set; }

        public CreateUserResponse(int user_id, string user_email)
        {
            username = user_email;
            this.user_id = user_id;
        }
    }
}
