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

        public PlanDTO(int plan_id, string plan_name, string plan_description, int semester_year, int semester_num)
        {
            this.plan_id = plan_id;
            this.plan_name = plan_name;
            this.plan_description = plan_description;
            this.semester_year = semester_year;
            this.semester_num = semester_num;
        }

        public PlanDTO()
        {

        }
    }
}
