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
    [ApiController]
    [Route("time_slot-management")]
    public class TimeSlotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public TimeSlotController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }


        /// <summary>Get all time slots</summary>
        /// <remarks>GET request that retrieves all time slots.</remarks>
        [HttpGet("time_slots")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<TimeSlotDTO>>> GetAllTimeSlots()
        {
            var slots = new List<TimeSlotDTO>();
            try
            {
                // create query string
                var query = @"SELECT *
                              FROM time_slot";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<TimeSlotDTO>(query, CommandType.Text);
                    slots = result.ToList();
                }
                // if there are any time slots, return the records
                if (slots.Count > 0)
                {
                    return Ok(slots);
                }
                else
                {
                    return NotFound();
                }
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get time slot by time_slot_id</summary>
        /// <remarks>GET request that retrieves the time slot with specified time slot id.</remarks>
        [HttpGet("time_slots/{time_slot_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<TimeSlotDTO>>> GetTimeSlotById(int time_slot_id)
        {
            var slot = new List<TimeSlotDTO>();
            try
            {
                // Create query string
                var query = @"SELECT * 
                              FROM time_slot 
                              WHERE time_slot_id = @time_slot_id";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeSlotDTO>(query, new { time_slot_id = time_slot_id});
                    slot = result.ToList();
                }
                // If plan exists, return it
                if (slot.Count > 0)
                {
                    return Ok(slot);
                }
                else
                {
                    return NotFound();
                }
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
