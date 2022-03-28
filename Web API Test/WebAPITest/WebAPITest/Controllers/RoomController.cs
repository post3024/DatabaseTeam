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
        //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IConfiguration configuration, ILogger<RoomController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        [HttpGet("rooms")]
        public async Task<ActionResult<List<RoomDTO>>> GetAllRooms()
        {
            var rooms = new List<RoomDTO>();
            try
            {
                string query = @"SELECT room_id, capacity, room_num FROM room";
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<RoomDTO>(query, CommandType.Text);
                    rooms = result.ToList();
                }
                if (rooms.Count > 0)
                {
                    return Ok(rooms);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}
