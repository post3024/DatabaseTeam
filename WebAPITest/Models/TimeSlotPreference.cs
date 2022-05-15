using System;
namespace WebAPITest.Models
{
    public class TimeSlotPreference
    {
        public int time_slot_id { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        public bool can_teach { get; set; }
    }
}
