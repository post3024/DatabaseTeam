using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPITest.Models;
using MySql.Data.MySqlClient;
using Dapper;

namespace WebAPITest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("sections-management")]
    public class SectionController : ControllerBase
    {
        //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public SectionController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all sections</summary>
        /// <remarks>GET request that retrieves all sections.</remarks>
        [HttpGet("sections")]
        public async Task<ActionResult<List<SectionDTO>>> GetAllSections()
        {
            var sections = new List<SectionDTO>();
            try
            {
                string query = @"SELECT * FROM section";
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                    sections = result.ToList();
                }
                if (sections.Count > 0)
                {
                    return Ok(sections);
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

        /// <summary>Get sections by section id</summary>
        /// <remarks>GET request that retrieves the sections with specified section id.</remarks>
        [HttpGet("sections/{section_id}")]
        public async Task<ActionResult<List<SectionDTO>>> GetSectionById(string section_id)
        {
            var sections = new List<SectionDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM section 
                                 WHERE section_id = '" + section_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                    sections = result.ToList();
                }
                // If the prof exists, return the record
                if (sections.Count > 0)
                {
                    return Ok(sections);
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

        /// <summary>Get sections by plan id</summary>
        /// <remarks>GET request that retrieves the sections with specified plan id.</remarks>
        [HttpGet("sections/{plan_id}")]
        public async Task<ActionResult<List<SectionDTO>>> GetSectionsByPlanId(string plan_id)
        {
            var sections = new List<SectionDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM section 
                                 WHERE plan_id = " + plan_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                    sections = result.ToList();
                }
                // If the prof exists, return the record
                if (sections.Count > 0)
                {
                    return Ok(sections);
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
        [HttpDelete("sections/delete/{section_id}")]
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
                    var result = await connection.QueryAsync<SectionDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted schedule with id: " + section_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete sections by plan id</summary>
        /// <remarks>DELETE request that deletes the section with specified plan id.</remarks>
        [HttpDelete("sections/delete/{plan_id}")]
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
                    var result = await connection.QueryAsync<SectionDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted schedule with plan id: " + plan_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new section</summary>
        /// <remarks>POST request that creates a new section with inputted information.</remarks>
        [HttpPost("sections/create")]
        public async Task<ActionResult> InsertSection(SectionInsertDTO model)
        {
            try
            {
                // create query string
                string query = @"INSERT INTO section (section_num, dept_id, room_id, professor_id, class_num, plan_id) " +
                                "VALUES (" + model.section_num + "," + model.dept_id + "," + model.room_id + "," + model.professor_id + "," + model.class_num + "," + model.plan_id + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                    return StatusCode(200, "Successfully created section number: " + model.section_num);
                }
            }
            //catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update sections by section id</summary>
        /// <remarks>PUT request that updates the section with specified section id to be set to the new inputted values.</remarks>
        [HttpPut("sections/update/{section_id}")]
        public async Task<ActionResult> UpdateSection(SectionInsertDTO model, int section_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE section
                                 SET section_num = " + model.section_num + ", dept_id = " + model.dept_id + ", room_id = " + model.room_id +
                                 ", professor_id = " + model.professor_id + ", class_num = " + model.class_num + ", plan_id = " + model.plan_id + 
                                 " WHERE section_id = " + section_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated section");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
