using System;
// this class handles the schema of a time slot preference insert
namespace WebAPITest.Models
{
    public class TimeSlotPreferenceInsert
    {
        public int time_slot_id { get; set; }

        public bool can_teach { get; set; }
    }
}
