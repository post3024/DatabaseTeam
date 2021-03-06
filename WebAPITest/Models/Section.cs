using System;

// this class handles the schema for a section data object from the database
namespace WebAPITest.Models
{
    public class SectionDTO
    {
        public int section_id { get; set; }

        public int section_num { get; set; }

        public int class_id { get; set; }

        public int room_id { get; set; }

        public int professor_id { get; set; }

        public int plan_id { get; set; }

        public int section_time_slot_id { get; set; }

        // constructor
        public SectionDTO(int section_id, int section_num, int class_id, int room_id, int professor_id, int plan_id, int section_time_slot_id)
        {
            this.section_id = section_id;
            this.section_num = section_num;
            this.class_id = class_id;
            this.room_id = room_id;
            this.professor_id = professor_id;
            this.plan_id = plan_id;
            this.section_time_slot_id = section_time_slot_id;
        }

        public SectionDTO()
        {

        }
    }
}
