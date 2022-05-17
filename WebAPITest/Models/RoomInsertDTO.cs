using System;

// this class handles the schema for a room object to be inserted
namespace WebAPITest.Models
{
    public class RoomInsertDTO
    {
        public int room_num { get; set; }

        public string building_name { get; set; }

        public int capacity { get; set; }
    }
}
