using System;
namespace WebAPITest.Models
{
    public class PreferenceInsertDTO
    {
        public int professor_id { get; set; }

        public int time_slot_id { get; set; }

        public int preference { get; set; }
    }
}
