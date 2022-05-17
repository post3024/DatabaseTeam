using System;

namespace WebAPITest.Models
{
    // this class handles the schema for a user's response for authentication
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
