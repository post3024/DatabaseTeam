using System;
namespace WebAPITest.Models
{
    public class ProfessorScheduleInfoDTO
    {
        public int professor_id { get; set; }

        public int class_num { get; set; }

        public string dept_name { get; set; }

        public string class_name { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        public bool on_monday { get; set; }

        public bool on_tuesday { get; set; }

        public bool on_wednesday { get; set; }

        public bool on_thursday { get; set; }

        public bool on_friday { get; set; }
    }
}
