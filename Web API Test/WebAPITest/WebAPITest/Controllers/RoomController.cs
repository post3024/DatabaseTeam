using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using WebAPITest.Models;

namespace WebAPITest.Controllers
{
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


        //Get all of the room records in the table
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
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        //Get a room by room id
        [HttpPost("room/id")]
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
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post request will delete a room record based on the inputed room_id
         * 
         * @param room_id - id of a room record in the database
         */
        [HttpPost("room/delete")]
        public async Task<ActionResult<List<RoomDTO>>> DeleteRoomById(string room_id)
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
                return StatusCode(200, "Successfully deleted " + room_id);
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post request will create a new room record based on the inputted room_num and capacity
         */  
        [HttpPost("room/create")]
        public async Task<ActionResult<List<RoomDTO>>> InsertRoom(string room_num, string capacity)
        {
            try
            {
                //Create the query string
                string query = @"INSERT INTO room (capacity, room_num) " +
                                "VALUES (" + capacity + "," + room_num + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query string
                    var result = await connection.QueryAsync<RoomDTO>(query, CommandType.Text);
                }
                //Return successful status code
                return StatusCode(200, "Successfully created " + room_num + " with capacity " + capacity);
            }
            //Catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }

}
