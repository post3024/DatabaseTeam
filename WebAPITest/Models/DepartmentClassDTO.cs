// This class handles the schema for an inner join between the class and department table﻿

namespace WebAPITest.Models
{
    public class DepartmentClassDTO
    {
        public int class_id { get; set; }

        public int class_num { get; set; }

        public int dept_id { get; set; }

        public string class_name { get; set; }

        public int capacity { get; set; }

        public int credits { get; set; }

        public bool is_lab { get; set; }

        public int num_sections { get; set; }

        public string dept_name { get; set; }
    }
}
