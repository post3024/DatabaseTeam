using System;
namespace WebAPITest.Models
{
    public class TimeSlotDTO
    {
        public int time_slot_id { get; set; }

        public TimeSpan start_time { get; set; }

        public TimeSpan end_time { get; set; }

        public string day_of_week { get; set; }
    }
}
