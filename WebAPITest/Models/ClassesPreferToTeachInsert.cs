using System;
// this class handles the schema for a professors preferred classes to teach
namespace WebAPITest.Models
{
    public class ClassesPreferToTeachInsertDTO
    {
        public int class_id { get; set; }

        public bool prefer_to_teach { get; set; }
    }
}
