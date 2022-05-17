using System;
// This class handles an admin object to be inserted into the database
namespace WebAPITest.Models
{
    public class CreateAdminDTO
    {
        public string username { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string password { get; set; }
    }
}
