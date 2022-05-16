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

        /// <summary>Get all class preferences for all professors</summary>
        /// <remarks>GET request that retrieves all class preferences for all professors. Only accessible by an admin.</remarks>
        [HttpGet("class-preferences")]
        [Authorize("admin")]
        public async Task<ActionResult<List<ProfessorClassPreferencesDTO>>> GetAllClassPreferences()
        {
            var professor_preferences = new List<ProfessorClassPreferencesDTO>();
            var prof_ids = new List<int>();
            try
            {
                // Create query string
                string query = @"SELECT DISTINCT prof_id
                                 FROM class_preference";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<int>(query, CommandType.Text);
                    prof_ids = result.ToList();
                }
                if (prof_ids.Count > 0)
                {
                    foreach (int prof_id in prof_ids)
                    {
                        var class_preferences = await GetClassPreference(prof_id);
                        if (class_preferences != null)
                        {
                            professor_preferences.Add(new ProfessorClassPreferencesDTO(prof_id, class_preferences));
                        }
                    }
                    return professor_preferences;
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

        // Helper method
        private async Task<List<ClassPreferenceDTO>> GetClassPreference(int professor_id)
        {
            try
            {
                List<ClassPreferenceDTO> class_preferences;
                // Create query string
                string query = @"SELECT *
                                 FROM class_preference
                                 WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassPreferenceDTO>(query, new {professor_id = professor_id });
                    class_preferences = result.ToList();
                }
                return class_preferences;
            }
            // catch exception
            catch (Exception)
            {
                return null;
            }
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
                string query = @"SELECT *
                                 FROM class_preference
                                 WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassesCanTeachDTO>(query, new { professor_id = professor_id });
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

        /// <summary>Get what professors can teach a specified class</summary>
        /// <remarks>GET request that retrieves a specified class and a list of the professors that can teach it. Only accessible by an admin.</remarks>
        [HttpGet("class-preferences/can-teach/by-class/{class_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetProfessorsForClass(int class_id)
        {
            var professors = new List<ProfessorDTO>();
            try
            {
                // Create query string
                string query = @"SELECT professor.*
                                 FROM class_preference
                                 JOIN professor
                                 WHERE class_preference.prof_id = professor.professor_id AND " +
                                 "class_id = @class_id AND can_teach = true;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ProfessorDTO>(query, new { class_id = class_id });
                    professors = result.ToList();
                }
                // If a preference was returned from database, return it
                if (professors.Count > 0)
                {
                    return Ok(professors);
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
                string query = @"SELECT *
                                 FROM class_preference
                                 WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassesPreferToTeachDTO>(query, new { professor_id = professor_id });
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

        /// <summary>Get what professors prefer to teach a specified class</summary>
        /// <remarks>GET request that retrieves a specified class and a list of the professors that prefer to teach it. Only accessible by an admin.</remarks>
        [HttpGet("class-preferences/prefer-to-teach/by-class/{class_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetProfessorPreferenceForClass(int class_id)
        {
            var professors = new List<ProfessorDTO>();
            try
            {
                // Create query string
                string query = @"SELECT professor.*
                                 FROM class_preference
                                 JOIN professor
                                 WHERE class_preference.prof_id = professor.professor_id AND " +
                                 "class_id = @class_id AND prefer_to_teach = true;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ProfessorDTO>(query, new { class_id = class_id});
                    professors = result.ToList();
                }
                // If a preference was returned from database, return it
                if (professors.Count > 0)
                {
                    return Ok(professors);
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
                    string query = @"INSERT INTO class_preference (class_id, prof_id, can_teach) " +
                                    "VALUES (@class_id, @professor_id, @can_teach) " +
                                    "ON DUPLICATE KEY UPDATE class_id = @class_id, prof_id = @professor_id, can_teach = @can_teach;";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, new { class_id = item.class_id, professor_id = professor_id, can_teach = item.can_teach });
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
                    string query = @"INSERT INTO class_preference (class_id, prof_id, prefer_to_teach) " +
                                    "VALUES (@class_id, @professor_id, @prefer_to_teach)" +
                                    "ON DUPLICATE KEY UPDATE class_id = @class_id, prof_id = @professor_id, prefer_to_teach = @prefer_to_teach;";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, new { class_id = item.class_id, professor_id = professor_id, prefer_to_teach = item.prefer_to_teach });
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
                                "WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new {professor_id = professor_id});
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
                                 WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<DayOfWeekPreferenceDTO>(query, new { professor_id = professor_id });
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
                                    "VALUES (@professor_id, @prefer_monday, @prefer_tuesday, @prefer_wednesday, @prefer_thursday, @prefer_friday) " +
                                    "ON DUPLICATE KEY UPDATE prof_id = @professor_id, prefer_monday = @prefer_monday, prefer_tuesday = @prefer_tuesday, prefer_wednesday = @prefer_wednesday, prefer_thursday = @prefer_thursday, prefer_friday = @prefer_friday;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<DayOfWeekPreferenceDTO>(query, new { professor_id = professor_id, 
                        prefer_monday = model.prefer_monday,
                        prefer_tuesday = model.prefer_tuesday, 
                        prefer_wednesday = model.prefer_wednesday, 
                        prefer_thursday = model.prefer_thursday, 
                        prefer_friday = model.prefer_friday });
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
                                "WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new { professor_id = professor_id});
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
                                 WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeOfDayPreferenceDTO>(query, new { professor_id = professor_id });
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
                                    "VALUES (@professor_id, @prefer_morning, @prefer_afternoon, @prefer_evening)" +
                                    "ON DUPLICATE KEY UPDATE prof_id = @professor_id, prefer_morning = @prefer_morning, " +
                                    "prefer_afternoon = @prefer_afternoon, prefer_evening = @prefer_evening;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeOfDayPreferenceDTO>(query, new { professor_id = professor_id, 
                        prefer_morning = model.prefer_morning, 
                        prefer_afternoon = model.prefer_afternoon, 
                        prefer_evening = model.prefer_evening });
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
                                "WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new { professor_id = professor_id });
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
                                 WHERE p.prof_id = @professor_id AND p.time_slot_id = t.time_slot_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<TimeSlotPreference>(query, new { professor_id = professor_id });
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
                                    "VALUES (@time_slot_id, @professor_id, @can_teach)" +
                                    " ON DUPLICATE KEY UPDATE time_slot_id = @time_slot_id, prof_id = @professor_id, can_teach = @can_teach;";

                    using (var connection = new MySqlConnection(connString))
                    {
                        // Execute the query
                        var result = await connection.QueryAsync(query, new { time_slot_id = item.time_slot_id, professor_id = professor_id, can_teach = item.can_teach });
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
                                "WHERE prof_id = @professor_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new { professor_id = professor_id });
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
