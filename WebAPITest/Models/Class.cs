using System;
// This model handles a class object of all fields from the class table
namespace WebAPITest.Models
{
    public class ClassDTO
    {
        public int class_id { get; set; }

        public int class_num { get; set; }

        public int dept_id { get; set; }

        public string class_name { get; set; }

        public int capacity { get; set; }

        public int credits { get; set; }

        public bool is_lab { get; set; }

        public int num_sections { get; set; }

        // constructor
        public ClassDTO(int class_id, int class_num, int dept_id, string class_name, int capacity, int credits, bool is_lab, int num_sections)
        {
            this.class_id = class_id;
            this.class_num = class_num;
            this.dept_id = dept_id;
            this.class_name = class_name;
            this.capacity = capacity;
            this.is_lab = is_lab;
            this.num_sections = num_sections;
        }

        public ClassDTO()
        {

        }
    }
}

