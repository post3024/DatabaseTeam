using System;
// this class handles the preferences for a professor in the database
namespace WebAPITest.Models
{
    public class DayOfWeekPreferenceDTO
    {
        public bool prefer_monday { get; set; }
        public bool prefer_tuesday { get; set; }
        public bool prefer_wednesday { get; set; }
        public bool prefer_thursday { get; set; }
        public bool prefer_friday { get; set; }
    }
}
