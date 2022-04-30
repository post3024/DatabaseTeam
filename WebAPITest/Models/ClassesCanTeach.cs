using System;
namespace WebAPITest.Models
{
    public class ClassesCanTeachDTO
    {
        public int class_num { get; set; }

        public int dept_id { get; set; }

        public bool is_lab { get; set; }

        public string class_name { get; set; }

        public bool can_teach { get; set; }
    }
}
