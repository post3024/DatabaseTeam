using System;
namespace WebAPITest.Models
{
    public class PreferenceDTO
    {
        public int preference_id { get; set; }

        public int professor_id { get; set; }

        public int time_slot_id { get; set; }

        public int preference { get; set; }

        public PreferenceDTO(int preference_id, int professor_id, int time_slot_id, int preference)
        {
            this.preference_id = preference_id;
            this.professor_id = professor_id;
            this.time_slot_id = time_slot_id;
            this.preference = preference;
        }

        public PreferenceDTO()
        {

        }
    }
}
