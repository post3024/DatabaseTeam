using System;
namespace WebAPITest.Models
{
    public class ClassesCanTeachInsertDTO
    {
        public int class_num { get; set; }

        public int dept_id { get; set; }

        public bool is_lab { get; set; }

        public bool can_teach { get; set; }
    }
}
