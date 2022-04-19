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
    [Authorize]
    [ApiController]
    [Route("schedule-management")]
    public class ScheduleController : ControllerBase
    {
        //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ScheduleController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all schedules</summary>
        /// <remarks>GET request that retrieves all schedules.</remarks>
        [HttpGet("schedules")]
        public async Task<ActionResult<List<ScheduleDTO>>> GetAllSchedules()
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get schedules by section id</summary>
        /// <remarks>GET request that retrieves the schedules with specified section id.</remarks>
        [HttpGet("schedules/{section_id}")]
        public async Task<ActionResult<List<ScheduleDTO>>> GetScheduleBySectionId(string section_id)
        {
            var sects = new List<ScheduleDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM section 
                                 WHERE section_id = '" + section_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ScheduleDTO>(query, CommandType.Text);
                    sects = result.ToList();
                }
                // If the prof exists, return the record
                if (sects.Count > 0)
                {
                    return Ok(sects);
                }
                // else, send error
                else
                {
                    return NotFound();
                }
            }
            // Catch any exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get schedules by plan id</summary>
        /// <remarks>GET request that retrieves the schedules with specified plan id.</remarks>
        [HttpGet("schedules/{plan_id}")]
        public async Task<ActionResult<List<ScheduleDTO>>> GetSchedulesByPlanId(string plan_id)
        {
            var sects = new List<ScheduleDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM sections 
                                 WHERE plan_id = '" + plan_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ScheduleDTO>(query, CommandType.Text);
                    sects = result.ToList();
                }
                // If the prof exists, return the record
                if (sects.Count > 0)
                {
                    return Ok(sects);
                }
                // else, send error
                else
                {
                    return NotFound();
                }
            }
            // Catch any exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete sections by section id</summary>
        /// <remarks>DELETE request that deletes the section with specified section id.</remarks>
        [HttpDelete("schedules/delete/{section_id}")]
        public async Task<ActionResult> DeleteSectionById(string section_id)
        {
            try
            {
                // Create the query string
                string deleteQuery = @"DELETE FROM section " +
                                      "WHERE section_id = '" + section_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted schedule with id: " + section_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete sections by section id</summary>
        /// <remarks>DELETE request that deletes the section with specified section id.</remarks>
        [HttpDelete("schedules/delete/{plan_id}")]
        public async Task<ActionResult> DeleteSectionByPlanId(string plan_id)
        {
            try
            {
                // Create the query string
                string deleteQuery = @"DELETE FROM section " +
                                      "WHERE plan_id = '" + plan_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ScheduleDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted schedule with plan id: " + plan_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new professor</summary>
        /// <remarks>POST request that creates a new professor with inputted information. Returns the auto-generated professor id for the newly added professor.</remarks>
        [HttpPost("schedules/create")]
        public async Task<ActionResult> InsertSection(ScheduleDTO model)
        {
            try
            {
                // create query string
                string query = @"INSERT INTO section (section_num, dept_id, room_id, professor_id, class_num) " +
                                "VALUES (" + model.section_num + "," + model.dept_id + "," + model.room_id + "," + model.professor_id + "," + model.class_num + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute the query string
                    var result = await connection.QueryAsync<ScheduleDTO>(query, CommandType.Text);
                    return StatusCode(200, "Successfully created section number: " + model.section_num);
                }
            }
            //catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
