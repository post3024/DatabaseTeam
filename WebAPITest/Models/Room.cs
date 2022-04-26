using System;
namespace WebAPITest.Models
{
    public class RoomDTO
    {
        public int room_id { get; set; }

        public int room_num { get; set; }

        public int capacity { get; set; }

        public string building_name { get; set; }

        public RoomDTO(int room_id, int room_num, int capacity, string building_name)
        {
            this.room_id = room_id;
            this.room_num = room_num;
            this.capacity = capacity;
            this.building_name = building_name;
        }

        public RoomDTO()
        {

        }
    }
}
