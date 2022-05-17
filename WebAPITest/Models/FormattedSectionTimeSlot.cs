using System;

// this class handles the schema for a formatted version of the section time slots table
namespace WebAPITest.Models
{
    public class FormattedSectionTimeSlot
    {
        public int id { get; set; }

        public string time {get; set;}

        public string partOfDay { get; set; }

        public FormattedSectionTimeSlot(int id, string time, string partOfDay)
        {
            this.id = id;
            this.time = time;
            this.partOfDay = partOfDay;
        }

        public FormattedSectionTimeSlot()
        {

        }
    }
}
