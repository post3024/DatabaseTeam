using System;
// this class handles the schema for a time slot
namespace WebAPITest.Models
{
    public class TimeSlotDTO
    {
        public int time_slot_id { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        // constructor
        public TimeSlotDTO(int time_slot_id, string start_time, string end_time)
        {
            this.time_slot_id = time_slot_id;
            this.start_time = start_time;
            this.end_time = end_time;
        }

        public TimeSlotDTO()
        {

        }
    }
}
