using System;
namespace WebAPITest.Models
{
    public class ClassesPreferToTeachDTO
    {
        public int class_num { get; set; }

        public int dept_id { get; set; }

        public bool is_lab { get; set; }

        public string class_name { get; set; }

        public bool prefer_to_teach { get; set; }
    }
}
