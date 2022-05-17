using System;

// this class handles the schema for a section_time_slot data object
namespace WebAPITest.Models
{
    public class SectionTimeSlotDTO
    {
        public int section_time_slot_id { get; set; }

        public int time_slot_id { get; set; }

        public bool on_monday { get; set; }

        public bool on_tuesday { get; set; }

        public bool on_wednesday { get; set; }

        public bool on_thursday { get; set; }

        public bool on_friday { get; set; }

        // constructor
        public SectionTimeSlotDTO(int section_time_slot_id, int time_slot_id, bool on_monday, bool on_tuesday, bool on_wednesday, bool on_thursday, bool on_friday)
        {
            this.section_time_slot_id = section_time_slot_id;
            this.time_slot_id = time_slot_id;
            this.on_monday = on_monday;
            this.on_tuesday = on_tuesday;
            this.on_wednesday = on_wednesday;
            this.on_thursday = on_thursday;
            this.on_friday = on_friday;
        }

        public SectionTimeSlotDTO()
        {

        }
    }
}
