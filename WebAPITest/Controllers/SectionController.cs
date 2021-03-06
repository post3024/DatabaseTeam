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
    [ApiController]
    [Route("sections-management")]
    public class SectionController : ControllerBase
    {
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
        [Authorize("admin")]
        public async Task<ActionResult<List<SectionDTO>>> GetAllSections()
        {
            var sections = new List<SectionDTO>();
            try
            {
                var query = @"SELECT *
                              FROM section";
                              
                // connect and execute query string
                using (var connection = new MySqlConnection(connString))
                {
                    var result = await connection.QueryAsync<SectionDTO>(query, CommandType.Text);
                    sections = result.ToList();
                }
                
                // if any sections exist, return them
                if (sections.Count > 0)
                {
                    return Ok(sections);
                }
                // else, return 404 error
                else
                {
                    return NotFound();
                }
            }
            // catch the exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get sections by section id</summary>
        /// <remarks>GET request that retrieves the sections with specified section id.</remarks>
        [HttpGet("sections/{section_id}")]
        [Authorize("admin")]
        public async Task<ActionResult<List<SectionDTO>>> GetSectionById(string section_id)
        {
            var sections = new List<SectionDTO>();
            try
            {
                // Create the query string
                var query = @"SELECT * 
                              FROM section 
                              WHERE section_id = @section_id";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(query, new { section_id = section_id});
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
        [HttpGet("sections/plan/{plan_id}")]
        [Authorize("admin")]
        public async Task<ActionResult<List<SectionDTO>>> GetSectionsByPlanId(string plan_id)
        {
            var sections = new List<SectionDTO>();
            try
            {
                // Create the query string
                var query = @"SELECT * 
                              FROM section 
                              WHERE plan_id = @plan_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(query, new { plan_id = plan_id});
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

        /// <summary>Get sections by professor id</summary>
        /// <remarks>GET request that retrieves the sections with specified professor id.</remarks>
        [HttpGet("sections/professor/{professor_id}")]
        [Authorize("admin","user")]
        public async Task<ActionResult<List<ProfessorScheduleInfoDTO>>> GetSectionsByProfessorId(int professor_id)
        {
            var sections = new List<ProfessorScheduleInfoDTO>();
            try
            {
                // user can only access their own records
                var currentUser = (User)HttpContext.Items["User"];
                if (professor_id != currentUser.user_id && currentUser.user_role != "admin") { 
                    return Unauthorized(new { message = "Unauthorized" });
                }

                // Create the query string
                var query = @"SELECT section.professor_id,
                                class.class_num,
                                department.dept_name,
                                class.class_name,
                                time_slot.start_time,
                                time_slot.end_time,
                                section_time_slot.on_monday,
                                section_time_slot.on_tuesday,
                                section_time_slot.on_wednesday,
                                section_time_slot.on_thursday,
                                section_time_slot.on_friday
                             FROM (((section INNER JOIN class
                                        ON section.class_id = class.class_id)
                                    INNER JOIN department
                                        ON department.dept_id = class.dept_id)
                                    INNER JOIN section_time_slot
                                        ON section.section_time_slot_id = section_time_slot.section_time_slot_id)
                                    INNER JOIN time_slot
                                        ON section_time_slot.time_slot_id = time_slot.time_slot_id
                             WHERE section.professor_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorScheduleInfoDTO>(query, new { professor_id = professor_id});
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
        [Authorize("admin")]
        public async Task<ActionResult> DeleteSectionById(string section_id)
        {
            try
            {
                // Create the query string
                var deleteQuery = @"DELETE FROM section " +
                                   "WHERE section_id = @section_id";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(deleteQuery, new { section_id = section_id});
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
        [HttpDelete("sections/delete/plan/{plan_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteSectionByPlanId(string plan_id)
        {
            try
            {
                // Create the query string
                var deleteQuery = @"DELETE FROM section " +
                                   "WHERE plan_id = @plan_id";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<SectionDTO>(deleteQuery, new { plan_id = plan_id });
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
        [Authorize("admin")]
        public async Task<ActionResult<SectionDTO>> InsertSection(SectionInsertDTO model)
        {
            try
            {
                // create query string
                var query = @"INSERT INTO section (
                                section_num, 
                                room_id, 
                                professor_id, 
                                plan_id, 
                                section_time_slot_id, 
                                class_id) " +
                             "VALUES (@section_num, @room_id, @professor_id, @plan_id, @section_time_slot_id, @class_id);" +
                             "SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute the query string
                    var id = await connection.QueryAsync<int>(query, new { section_num = model.section_num, 
                        room_id = model.room_id, 
                        professor_id = model.professor_id, 
                        plan_id = model.plan_id, 
                        section_time_slot_id = model.section_time_slot_id, 
                        class_id =model.class_id });
                    
                    // create new sectionDTO object from the returned id and model
                    int section_id = id.ToList()[0];
                    SectionDTO newSection = new(
                        section_id, 
                        model.section_num, 
                        model.class_id, 
                        model.room_id, 
                        model.professor_id, 
                        model.plan_id, 
                        model.section_time_slot_id
                    );
                    return Ok(newSection);
                }
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create new sections</summary>
        /// <remarks>POST request that creates multiple sections with inputted list of information.</remarks>
        [HttpPost("sections/create/multiple")]
        [Authorize("admin")]
        public async Task<ActionResult<SectionDTO>> InsertSections(List<SectionInsertDTO> model)
        {
            try
            {
                List<SectionDTO> newSections = new List<SectionDTO>();
                foreach (var item in model)
                {
                    // create query to create the each section and then get the id
                    string query = @"INSERT INTO section (
                                        section_num,
                                        room_id, 
                                        professor_id, 
                                        plan_id, 
                                        section_time_slot_id, 
                                        class_id) " +
                                    "VALUES (@section_num, @room_id, @professor_id, @plan_id, @section_time_slot_id, @class_id);" +
                                    "SELECT LAST_INSERT_ID();";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // execute the query string
                        var result = await connection.QueryAsync<int>(query, new
                        {
                            section_num = item.section_num,
                            room_id = item.room_id,
                            professor_id = item.professor_id,
                            plan_id = item.plan_id,
                            section_time_slot_id = item.section_time_slot_id,
                            class_id = item.class_id
                        });
                        int section_id = result.ToList()[0];

                        // Create new section object and add it to the list
                        SectionDTO section = new(
                            section_id, 
                            item.section_num, 
                            item.class_id, 
                            item.room_id, 
                            item.professor_id, 
                            item.plan_id, 
                            item.section_time_slot_id
                        );
                        newSections.Add(section);
                    }
                }
                return Ok(newSections);
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete previous sections related to inputted plan_id and insert a new list attached to plan id </summary>
        /// <remarks>POST request that deletes sections based on plan id and creates multiple sections with inputted list of information.</remarks>
        [HttpPost("sections/delete/create/multiple/{plan_id}")]
        [Authorize("admin")]
        public async Task<ActionResult<SectionDTO>> DeletePlanSectionsInsertList(List<SectionInsertDTO> models, string plan_id)
        {
            try
            {
                String deleteQuery = @"DELETE FROM section " +
                                      "WHERE plan_id = @plan_id";
                using (var connection = new MySqlConnection(connString))
                {
                    var result = await connection.QueryAsync<int>(deleteQuery, new { plan_id = plan_id });
                }


                List<SectionDTO> newSections = new List<SectionDTO>();
                foreach (var item in models)
                {
                    // create query string
                    var query = @"INSERT INTO section (
                                    section_num, 
                                    room_id, 
                                    professor_id,
                                    plan_id, 
                                    section_time_slot_id, 
                                    class_id) " +
                                 "VALUES (@section_num, @room_id, @professor_id, @plan_id, @section_time_slot_id, @class_id);" +
                                 "SELECT LAST_INSERT_ID();";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // execute the query string
                        var result = await connection.QueryAsync<int>(query, new
                        {
                            section_num = item.section_num,
                            room_id = item.room_id,
                            professor_id = item.professor_id,
                            plan_id = item.plan_id,
                            section_time_slot_id = item.section_time_slot_id,
                            class_id = item.class_id
                        });
                        int section_id = result.ToList()[0];

                        // Create new SectionDTO object and add it to the list
                        SectionDTO section = new(
                            section_id, 
                            item.section_num, 
                            item.class_id, 
                            item.room_id, 
                            item.professor_id, 
                            Int32.Parse(plan_id), 
                            item.section_time_slot_id
                        );
                        newSections.Add(section);
                    }
                }
                return Ok(newSections);
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update sections by section id</summary>
        /// <remarks>PUT request that updates the section with specified section id to be set to the new inputted values.</remarks>
        [HttpPut("sections/update/{section_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> UpdateSection(SectionInsertDTO model, int section_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE section
                                 SET section_num = @section_num,
                                     class_id = @class_id,
                                     room_id = @room_id,
                                     professor_id = @professor_id,
                                     plan_id = @plan_id,
                                     section_time_slot_id = @section_time_slot_id " +
                                "WHERE section_id = @section_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<SectionDTO>(query, new
                    {
                        section_num = model.section_num,
                        room_id = model.room_id,
                        professor_id = model.professor_id,
                        plan_id = model.plan_id,
                        section_time_slot_id = model.section_time_slot_id,
                        class_id = model.class_id,
                        section_id = section_id
                    });
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
