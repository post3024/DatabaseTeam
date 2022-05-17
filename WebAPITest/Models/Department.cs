using System;

// this class handles the schema for the department table
namespace WebAPITest.Models
{
    public class DepartmentDTO
    {
        public int dept_id { get; set; }
        public string dept_name { get; set; }

        public DepartmentDTO(int dept_id, string dept_name)
        {
            this.dept_id = dept_id;
            this.dept_name = dept_name;
        }

        public DepartmentDTO()
        {

        }
    }
}
