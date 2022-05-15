using System;
namespace WebAPITest.Models
{
    public class ClassInsertDTO
    {
        public int class_num { get; set; }

        public int dept_id { get; set; }

        public string class_name { get; set; }

        public int capacity { get; set; }

        public int credits { get; set; }

        public bool is_lab { get; set; }

        public int num_sections { get; set; }
    }
}
