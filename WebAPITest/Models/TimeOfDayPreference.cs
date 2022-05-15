using System;
namespace WebAPITest.Models
{
    public class TimeOfDayPreferenceDTO
    {
        public bool prefer_morning { get; set; }
        
        public bool prefer_afternoon { get; set; }

        public bool prefer_evening { get; set; }
    }
}
