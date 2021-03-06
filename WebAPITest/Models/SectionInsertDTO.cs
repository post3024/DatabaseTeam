using System;

// this class holds the schema for a section to be inserted into the database
namespace WebAPITest.Models
{
    public class SectionInsertDTO
    {
        public int section_num { get; set; }

        public int class_id { get; set; }

        public int room_id { get; set; }

        public int professor_id { get; set; }

        public int plan_id { get; set; }

        public int section_time_slot_id {get; set;}
    }
}
