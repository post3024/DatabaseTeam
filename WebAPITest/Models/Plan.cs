using System;
namespace WebAPITest.Models
{
    public class PlanDTO
    {
        public int plan_id { get; set; }

        public string plan_name { get; set; }

        public string plan_description { get; set; }

        public int semester_year { get; set; }

        public int semester_num { get; set; }
    }
}
