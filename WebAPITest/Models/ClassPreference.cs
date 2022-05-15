using System;
namespace WebAPITest.Models
{
    public class ClassPreferenceDTO
    {
        public int class_id { get; set; }

        public bool can_teach { get; set; }

        public bool prefer_to_teach { get; set; }
    }
}
