using System;

// this class handles the schema of a time slot to be inserted
namespace WebAPITest.Models
{
    public class TimeSlotInsertDTO
    {
        public string start_time { get; set; }

        public string end_time { get; set; }
    }
}
