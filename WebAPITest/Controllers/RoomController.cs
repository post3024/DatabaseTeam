using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using WebAPITest.Models;

namespace WebAPITest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("room-management")]
    public class RoomController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public RoomController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }


        /// <summary>Get all rooms</summary>
        /// <remarks>GET request that retrieves all rooms.</remarks>
        [HttpGet("rooms")]
        public async Task<ActionResult<List<RoomDTO>>> GetAllRooms()
        {
            var rooms = new List<RoomDTO>();
            try
            {
                //Create the query string
                string query = @"SELECT * FROM room";
                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query
                    var result = await connection.QueryAsync<RoomDTO>(query, CommandType.Text);
                    rooms = result.ToList();
                }

                //If there are room records, return them
                if (rooms.Count > 0)
                {
                    return Ok(rooms);
                }
                else
                {
                    return NotFound();
                }
            }
            //Catch an excpetion and return an error status code
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get room by room id</summary>
        /// <remarks>GET request that retrieves the room with specified room id.</remarks>
        [HttpGet("rooms/{room_id}")]
        public async Task<ActionResult<List<RoomDTO>>> GetProfessorById(string room_id)
        {
            var rooms = new List<RoomDTO>();
            try
            {
                //Create the query string
                string query = @"SELECT * 
                                 FROM room 
                                 WHERE room_id = '" + room_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the the query
                    var result = await connection.QueryAsync<RoomDTO>(query, CommandType.Text);
                    rooms = result.ToList();
                }

                //Return room records if they were found
                if (rooms.Count > 0)
                {
                    return Ok(rooms);
                }
                else
                {
                    //Return 404 not found otherwise
                    return NotFound();
                }
            }
            //Catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete room by room id</summary>
        /// <remarks>DELETE request that deletes the room with specified room id.</remarks>
        [HttpDelete("rooms/delete/{room_id}")]
        public async Task<ActionResult> DeleteRoomById(string room_id)
        {
            try
            {
                //Create query string
                string deleteQuery = @"DELETE FROM room " +
                                      "WHERE room_id = '" + room_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted room " + room_id);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new room</summary>
        /// <remarks>POST request that creates a new room with inputted values. Returns the auto-generated room id for the newly added room.</remarks>
        [HttpPost("rooms/create")]
        public async Task<ActionResult> InsertRoom(RoomInsertDTO model)
        {
            try
            {
                //Create the query string
                string query = @"INSERT INTO room (capacity, room_num) " +
                                "VALUES (" + model.capacity + "," + model.room_num + ");";
                string queryId = @"SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query string
                    var result = await connection.QueryAsync<RoomDTO>(query, CommandType.Text);
                    var id = await connection.QueryAsync<String>(queryId, CommandType.Text);
                    String room_id = id.ToList()[0];
                    return StatusCode(200, "Successfully created room " + model.room_num + " with room_id = " + room_id);
                }
            }
            //Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update room by room id</summary>
        /// <remarks>PUT request that updates the room with specified room id to be set to the new inputted values.</remarks>
        [HttpPut("rooms/update/{room_id}")]
        public async Task<ActionResult> UpdateRoom(RoomDTO model, int room_id)
        {
            try
            {
                //create the query string
                string query = @"UPDATE room
                                 SET room_id = " + model.room_id + ", capacity = " + model.capacity + ", room_num = " + model.room_num +
                                 " WHERE room_id = " + room_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated room");
            }
            //catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
