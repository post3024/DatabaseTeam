using System;

// this class handles the schema for the professor table
namespace WebAPITest.Models
{
    public class ProfessorDTO
    {
        public int professor_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public int teach_load { get; set; }

        public string user_email { get; set; }

        public string user_role { get; set; }
    }
}
