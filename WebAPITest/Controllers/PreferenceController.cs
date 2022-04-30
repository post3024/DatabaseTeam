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
    [Route("preference-management")]
    public class PreferenceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public PreferenceController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all classes and whether a professor can or cannot teach them</summary>
        /// <remarks>GET request that retrieves all classes and whether the indicated professor can or cannot teach them. A user can only access their own preferences.</remarks>
        [HttpGet("class-preferences/can-teach/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ClassesCanTeachDTO>>> GetClassesCanTeach(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            var classes = new List<ClassesCanTeachDTO>();
            try
            {
                // Create query string
                string query = @"SELECT class.class_num, class.dept_id, class.is_lab, class.class_name, class_preference.can_teach
                                 FROM class_preference
                                 JOIN class
                                 WHERE prof_id = " + professor_id +
                                 " AND class.class_num = class_preference.class_num" +
                                 " AND class.dept_id = class_preference.dept_id" +
                                 " AND class.is_lab = class_preference.is_lab;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassesCanTeachDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                // If a preference was returned from database, return it
                if (classes.Count > 0)
                {
                    return Ok(classes);
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

        /// <summary>Get all classes and whether a professor prefers to teach them</summary>
        /// <remarks>GET request that retrieves all classes and whether the indicated professor prefers to teach them. A user can only access their own preferences.</remarks>
        [HttpGet("class-preferences/prefer-to-teach/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ClassesPreferToTeachDTO>>> GetClassesPreferToTeach(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            var classes = new List<ClassesPreferToTeachDTO>();
            try
            {
                // Create query string
                string query = @"SELECT class.class_num, class.dept_id, class.is_lab, class.class_name, class_preference.prefer_to_teach 
                                 FROM class_preference
                                 JOIN class
                                 WHERE prof_id = " + professor_id +
                                 " AND class.class_num = class_preference.class_num" +
                                 " AND class.dept_id = class_preference.dept_id" +
                                 " AND class.is_lab = class_preference.is_lab;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassesPreferToTeachDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                // If a preference was returned from database, return it
                if (classes.Count > 0)
                {
                    return Ok(classes);
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

        /// <summary>Save professor preferences for what classes they can and cannot teach</summary>
        /// <remarks>POST request that saves professor preferences for what classes they can and cannot teach. If the preference already exists,
        /// it is updated with the inputted information. If it does not exist, it is added to the table. Request takes in a list of class preference objects.
        /// A user can only access their own preferences.</remarks>
        [HttpPost("class-preferences/can-teach/save/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> SaveCanTeachPreference(List<ClassesCanTeachInsertDTO> model, int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            foreach (ClassesCanTeachInsertDTO item in model.ToList())
            {
                try
                {
                    // create the query string
                    string query = @"INSERT INTO class_preference (class_num, dept_id, is_lab, prof_id, can_teach) " +
                                    "VALUES (" + item.class_num + "," + item.dept_id + "," + item.is_lab + "," + professor_id + "," + item.can_teach +
                                    ") ON DUPLICATE KEY UPDATE class_num = " + item.class_num + ", dept_id = " + item.dept_id +
                                    ", is_lab = " + item.is_lab + ", prof_id = " + professor_id + ", can_teach = " + item.can_teach + ";";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, CommandType.Text);
                    }
                }
                // catch the exceptions
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            return StatusCode(200, "Successfully updated preferences");
        }

        /// <summary>Save professor preferences for what classes they prefer to teach</summary>
        /// <remarks>POST request that saves professor preferences for what classes they prefer to teach. If the preference already exists,
        /// it is updated with the inputted information. If it does not exist, it is added to the table. Request takes in a list of class preference objects.
        /// A user can only access their own preferences.</remarks>
        [HttpPost("class-preferences/prefer-to-teach/save/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> AddPreferToTeachPreference(List<ClassesPreferToTeachInsertDTO> model, int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            foreach (ClassesPreferToTeachInsertDTO item in model.ToList())
            {
                try
                {
                    // create the query string
                    string query = @"INSERT INTO class_preference (class_num, dept_id, is_lab, prof_id, prefer_to_teach) " +
                                    "VALUES (" + item.class_num + "," + item.dept_id + "," + item.is_lab + "," + professor_id + "," + item.prefer_to_teach +
                                    ")ON DUPLICATE KEY UPDATE class_num = " + item.class_num + ", dept_id = " + item.dept_id +
                                    ", is_lab = " + item.is_lab + ", prof_id = " + professor_id + ", prefer_to_teach = " + item.prefer_to_teach + ";";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, CommandType.Text);
                    }
                }
                // catch the exceptions
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            return StatusCode(200, "Successfully updated preferences");
        }

        /// <summary>Delete all class preferences for a professor</summary>
        /// <remarks>DELETE request that removes all class preferences for a professor with specified id. A user can only access their own preferences.</remarks>
        [HttpDelete("class-preferences/delete/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> DeleteClassPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            try
            {
                // create the query string
                string query = @"DELETE FROM class_preference " +
                                "WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preferences");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get the days of the week a professor prefers to teach</summary>
        /// <remarks>GET request that retrieves the days of the week and whether the indicated professor prefers to teach then.
        /// A user can only access their own preferences.</remarks>
        [HttpGet("day-of-week-preferences/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<DayOfWeekPreferenceDTO>> GetDayOfWeekPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            var results = new List<DayOfWeekPreferenceDTO>();
            try
            {
                // Create query string
                string query = @"SELECT *
                                 FROM day_of_week_preference
                                 WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<DayOfWeekPreferenceDTO>(query, CommandType.Text);
                    results = result.ToList();
                }
                // If a preference was returned from database, return it
                if (results.Count > 0)
                {
                    return Ok(results[0]);
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

        /// <summary>Saves the days of the week a professor prefers to teach</summary>
        /// <remarks>POST request that saves the days of the week the indicated professor prefers to teach. If the preference already exists,
        /// it is updated with the inputted information. If it does not exist, it is added to the table. A user can only access their own preferences.</remarks>
        [HttpPost("day-of-week-preferences/save/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> SaveDayOfWeekPreferences(DayOfWeekPreferenceDTO model, int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });
            
            try
            {
                // Create query string
                string query = @"INSERT INTO day_of_week_preference (prof_id, prefer_monday, prefer_tuesday, prefer_wednesday, prefer_thursday, prefer_friday) " +
                                    "VALUES (" + professor_id + "," + model.prefer_monday + "," + model.prefer_tuesday + "," +
                                    model.prefer_wednesday + "," + model.prefer_thursday + "," + model.prefer_friday +
                                    ")ON DUPLICATE KEY UPDATE prof_id = " + professor_id + ", prefer_monday = " + model.prefer_monday +
                                    ", prefer_tuesday = " + model.prefer_tuesday + ", prefer_wednesday = " + model.prefer_wednesday +
                                    ", prefer_thursday = " + model.prefer_thursday + ", prefer_friday = " + model.prefer_friday + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<DayOfWeekPreferenceDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated preferences");
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete all day of week preferences for a professor</summary>
        /// <remarks>DELETE request that removes all day of week preferences for a professor with specified id. A user can only access their own preferences.</remarks>
        [HttpDelete("day-of-week-preferences/delete/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> DeleteDayOfWeekPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            try
            {
                // create the query string
                string query = @"DELETE FROM day_of_week_preference " +
                                "WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preferences");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get the times of the day a professor prefers to teach</summary>
        /// <remarks>GET request that retrieves the times of the day and whether the indicated professor prefers to teach then. A user can only access their own preferences.</remarks>
        [HttpGet("time-of-day-preferences/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<TimeOfDayPreferenceDTO>> GetTimeOfDayPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            var results = new List<TimeOfDayPreferenceDTO>();
            try
            {
                // Create query string
                string query = @"SELECT *
                                 FROM time_of_day_preference
                                 WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeOfDayPreferenceDTO>(query, CommandType.Text);
                    results = result.ToList();
                }
                // If a preference was returned from database, return it
                if (results.Count > 0)
                {
                    return Ok(results[0]);
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

        /// <summary>Saves the times of the day a professor prefers to teach</summary>
        /// <remarks>POST request that saves the times of the day the indicated professor prefers to teach. If the preference already exists,
        /// it is updated with the inputted information. If it does not exist, it is added to the table. A user can only access their own preferences.</remarks>
        [HttpPost("time-of-day-preferences/save/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> SaveTimeOfDayPreferences(TimeOfDayPreferenceDTO model, int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            try
            {
                // Create query string
                string query = @"INSERT INTO time_of_day_preference (prof_id, prefer_morning, prefer_afternoon, prefer_evening) " +
                                    "VALUES (" + professor_id + "," + model.prefer_morning + "," + model.prefer_afternoon + "," + model.prefer_evening +
                                    ")ON DUPLICATE KEY UPDATE prof_id = " + professor_id + ", prefer_morning = " + model.prefer_morning +
                                    ", prefer_afternoon = " + model.prefer_afternoon + ", prefer_evening = " + model.prefer_evening + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeOfDayPreferenceDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated preferences");
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete all time of day preferences for a professor</summary>
        /// <remarks>DELETE request that removes all time of day preferences for a professor with specified id. A user can only access their own preferences.</remarks>
        [HttpDelete("time-of-day-preferences/delete/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> DeleteTimeOfDayPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            try
            {
                // create the query string
                string query = @"DELETE FROM time_of_day_preference " +
                                "WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preferences");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get all time slots and whether a professor can or cannot teach then</summary>
        /// <remarks>GET request that retrieves all the times slots and whether the indicated professor can or cannot teach then. A user can only access their own preferences.</remarks>
        [HttpGet("time-slot-preferences/can-teach/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<TimeSlotPreference>>> GetTimeSlotPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            var results = new List<TimeSlotPreference>();
            try
            {
                // Create query string
                string query = @"SELECT t.time_slot_id, t.start_time, t.end_time, p.can_teach
                                 FROM time_slot_preference p
                                 JOIN time_slot t
                                 WHERE p.prof_id = " + professor_id +
                                 " AND p.time_slot_id = t.time_slot_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeSlotPreference>(query, CommandType.Text);
                    results = result.ToList();
                }
                // If a preference was returned from database, return it
                if (results.Count > 0)
                {
                    return Ok(results);
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

        /// <summary>Save professor preferences for what time slots they can and cannot teach</summary>
        /// <remarks>POST request that saves professor preferences for what time slots they can and cannot teach. If the preference already exists,
        /// it is updated with the inputted information. If it does not exist, it is added to the table. Request takes in a list of time slot preference objects.
        /// A user can only access their own preferences.</remarks>
        [HttpPost("time-slot-preferences/can-teach/save/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> SaveTimeSlotPreference(List<TimeSlotPreferenceInsert> model, int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            foreach (TimeSlotPreferenceInsert item in model.ToList())
            {
                try
                {
                    // create the query string
                    string query = @"INSERT INTO time_slot_preference (time_slot_id, prof_id, can_teach) " +
                                    "VALUES (" + item.time_slot_id + "," + professor_id + "," + item.can_teach +
                                    ") ON DUPLICATE KEY UPDATE time_slot_id = " + item.time_slot_id + ", prof_id = " +
                                    professor_id + ", can_teach = " + item.can_teach + ";";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, CommandType.Text);
                    }
                }
                // catch the exceptions
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            return StatusCode(200, "Successfully updated preferences");
        }

        /// <summary>Delete all time slot preferences for a professor</summary>
        /// <remarks>DELETE request that removes all time slot preferences for a professor with specified id. A user can only access their own preferences.</remarks>
        [HttpDelete("time-slot-preferences/delete/{professor_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult> DeleteTimeSlotPreferences(int professor_id)
        {
            // user can only access their own records
            var currentUser = (User)HttpContext.Items["User"];
            if (professor_id != currentUser.user_id && currentUser.user_role != "admin")
                return Unauthorized(new { message = "Unauthorized" });

            try
            {
                // create the query string
                string query = @"DELETE FROM time_slot_preference " +
                                "WHERE prof_id = " + professor_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preferences");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
