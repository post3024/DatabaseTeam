using System;
// this model handles a professor can teach a class insert
namespace WebAPITest.Models
{
    public class ClassesCanTeachInsertDTO
    {
        public int class_id { get; set; }

        public bool can_teach { get; set; }
    }
}
