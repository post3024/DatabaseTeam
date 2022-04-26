using System;
namespace WebAPITest.Models
{
    public class CreateProfessorDTO
    {
        public int professor_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public int teach_load { get; set; }

        public string user_email { get; set; }

        public string password { get; set; }

        public CreateProfessorDTO(int professor_id, string first_name, string last_name, int teach_load, string user_email, string password)
        {
            this.professor_id = professor_id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.teach_load = teach_load;
            this.user_email = user_email;
            this.password = password;
        }
    }
}
