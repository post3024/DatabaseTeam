using System;
namespace WebAPITest.Models
{
    public class TimeSlotInsertDTO
    {
        public TimeSpan start_time { get; set; }

        public TimeSpan end_time { get; set; }

        public string day_of_week { get; set; }
    }
}
