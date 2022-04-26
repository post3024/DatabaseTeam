using System;
namespace WebAPITest.Models
{
    public class TimeSlotDTO
    {
        public int time_slot_id { get; set; }

        public TimeSpan start_time { get; set; }

        public TimeSpan end_time { get; set; }

        public string day_of_week { get; set; }

        public TimeSlotDTO(int time_slot_id, TimeSpan start_time, TimeSpan end_time, string day_of_week)
        {
            this.time_slot_id = time_slot_id;
            this.start_time = start_time;
            this.end_time = end_time;
            this.day_of_week = day_of_week;
        }
    }
}
