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
    [Route("section_time_slot-management")]
    public class SectionTimeSlotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public SectionTimeSlotController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }


        /// <summary>Get all section time slots</summary>
        /// <remarks>GET request that retrieves all section time slots.</remarks>
        [HttpGet("section_time_slots")]
        //[Authorize("admin", "user")]
        public async Task<ActionResult<List<SectionTimeSlotDTO>>> GetAllSectionTimeSlots()
        {
            var slots = new List<SectionTimeSlotDTO>();
            try
            {
                // create query string
                string query = @"SELECT * " +
                                "FROM section_time_slot";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<SectionTimeSlotDTO>(query, CommandType.Text);
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

        /// <summary>Get all section time slots formatted to id, time, and partOfDay</summary>
        /// <remarks>GET request that retrieves all section time slots and formats them to id, time, and partOfDay.</remarks>
        [HttpGet("section_time_slots/formatted")]
        //[Authorize("admin", "user")]
        public async Task<ActionResult<List<FormattedSectionTimeSlot>>> GetAllFormattedSectionTimeSlots()
        {
            var slots = new List<TimeSlotsJoinDTO>();
            try
            {
                // create query string
                string query = @"SELECT * " +
                                "FROM section_time_slot INNER JOIN time_slot " +
                                    "ON section_time_slot.time_slot_id = time_slot.time_slot_id";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<TimeSlotsJoinDTO>(query, CommandType.Text);
                    slots = result.ToList();
                }
                // if there are any time slots, return the records
                if (slots.Count > 0)
                {
                    var resultingSlots = new List<FormattedSectionTimeSlot>();
                    FormattedSectionTimeSlot newSlot;
                    string time = "";
                    string partOfDay = "";
                    foreach(var item in slots) {
                        time = "";
                        partOfDay = "";
                        if (item.on_monday) {
                            time += "M";
                        }
                        if (item.on_tuesday) {
                            time += "T";
                        }
                        if (item.on_wednesday) {
                            time += "W";
                        }
                        if (item.on_thursday) {
                            time += "H";
                        }
                        if (item.on_wednesday) {
                            time += "F";
                        }

                        time += " " + item.start_time + "-" + item.end_time;

                        int startHour = Int32.Parse(item.start_time.Split(':')[0]);

                        if (startHour <= 12)
                        {
                            partOfDay = "morning";
                        }
                        else if (startHour > 12 && startHour <= 17)
                        {
                            partOfDay = "afternoon";
                        }
                        else {
                            partOfDay = "night";
                        }

                        newSlot = new FormattedSectionTimeSlot(
                            item.section_time_slot_id,
                            time,
                            partOfDay
                        );
                        resultingSlots.Add(newSlot);
                    }

                    return Ok(resultingSlots);
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

        /// <summary>Get section time slot by section_time_slot_id</summary>
        /// <remarks>GET request that retrieves the section time slot with specified section time slot id.</remarks>
        [HttpGet("sectiontime_slots/{section_time_slot_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<SectionTimeSlotDTO>>> GetSectionTimeSlotById(int section_time_slot_id)
        {
            var slot = new List<SectionTimeSlotDTO>();
            try
            {
                // Create query string
                string query = @"SELECT * 
                                 FROM section_time_slot 
                                 WHERE section_time_slot_id = " + section_time_slot_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<SectionTimeSlotDTO>(query, CommandType.Text);
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

        /// <summary>Delete section time slot by section time slot id</summary>
        /// <remarks>DELETE request that deletes the section time slot with specified section time slot id.</remarks>
        [HttpDelete("section_time_slots/delete/{section_time_slot_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteSectionTimeSlotById(int section_time_slot_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM section_time_slot " +
                                      "WHERE section_time_slot_id = " + section_time_slot_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted time_slot with id: " + section_time_slot_id);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new time slot</summary>
        /// <remarks>POST request that creates a new time slot with the inputted information.</remarks>
        [HttpPost("section_time_slots/create")]
        [Authorize("admin")]
        public async Task<ActionResult<SectionTimeSlotDTO>> InsertSectionTimeSlot(SectionTimeSlotInsertDTO model)
        {
            try
            {
                // create the query string
                string query = @"INSERT INTO section_time_slot (time_slot_id, on_monday, on_tuesday, on_wednesday, on_thursday, on_friday) " +
                                "VALUES (" + model.time_slot_id + "," + model.on_monday + "," + model.on_tuesday + "," + model.on_wednesday + "," + model.on_thursday + "," + model.on_friday + ");" +
                                "SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var id = await connection.QueryAsync<int>(query, CommandType.Text);
                    int section_time_slot_id = id.ToList()[0];
                    SectionTimeSlotDTO newTimeSlot = new(section_time_slot_id, model.time_slot_id, model.on_monday, model.on_tuesday, model.on_wednesday, model.on_thursday, model.on_friday);
                    return Ok(newTimeSlot);
                }
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}