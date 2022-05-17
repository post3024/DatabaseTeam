using System;
// This class handles a professor model to be inserted into the database
namespace WebAPITest.Models
{
    public class CreateProfessorDTO
    {
        public int professor_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public int teach_load { get; set; }

        public string user_email { get; set; }
    }
}
