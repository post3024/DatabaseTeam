using System;
// this class handles a professor prefers to teach a class into the database
namespace WebAPITest.Models
{
    public class ClassesPreferToTeachDTO
    {
        public int class_id { get; set; }

        public bool prefer_to_teach { get; set; }
    }
}
