using System;
// this model handles a qualification insert into the database
namespace WebAPITest.Models
{
    public class ClassesCanTeachDTO
    {
        public int class_id { get; set; }

        public bool can_teach { get; set; }
    }
}
