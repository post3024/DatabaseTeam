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
    [Route("schedule-management")]
    public class ScheduleController : ControllerBase
    {
        //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IConfiguration configuration, ILogger<ScheduleController> logger)
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

        [HttpGet("schedules")]
        public async Task<ActionResult<List<ScheduleDTO>>> GetAllRooms()
        {
            var schedules = new List<ScheduleDTO>();
            try
            {
                string query = @"SELECT * FROM schedule";
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<ScheduleDTO>(query, CommandType.Text);
                    schedules = result.ToList();
                }
                if (schedules.Count > 0)
                {
                    return Ok(schedules);
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
