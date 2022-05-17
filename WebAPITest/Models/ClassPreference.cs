using System;
// This model handles a professors class preference to be inserted
namespace WebAPITest.Models
{
    public class ClassPreferenceDTO
    {
        public int class_id { get; set; }

        public bool can_teach { get; set; }

        public bool prefer_to_teach { get; set; }
    }
}
