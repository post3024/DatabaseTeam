using System;

// this class handles the schema of a professor to be inserted into the database
namespace WebAPITest.Models
{
    public class ProfessorInsertDTO
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public int teach_load { get; set; }

        public string user_email { get; set; }
    }
}
