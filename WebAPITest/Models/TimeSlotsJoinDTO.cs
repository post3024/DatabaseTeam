using System;
// this class handles the schema of a join of the section_time_slot table and the time_slot table
namespace WebAPITest.Models
{
    public class TimeSlotsJoinDTO
    {
        public int section_time_slot_id { get; set; }

        public int time_slot_id { get; set; }

        public bool on_monday { get; set; }

        public bool on_tuesday { get; set; }

        public bool on_wednesday { get; set; }

        public bool on_thursday { get; set; }

        public bool on_friday { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }
    }
}
