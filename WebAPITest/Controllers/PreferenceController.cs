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
    [Authorize]
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


        /// <summary>Get all preferences for all professors</summary>
        /// <remarks>GET request that retrieves all preferences.</remarks>
        [HttpGet("preferences")]
        public async Task<ActionResult<List<PreferenceDTO>>> GetAllPreferences()
        {
            var preferences = new List<PreferenceDTO>();
            try
            {
                // create query string
                string query = @"SELECT * FROM preference";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<PreferenceDTO>(query, CommandType.Text);
                    preferences = result.ToList();
                }
                // if there are any preferences, return the records
                if (preferences.Count > 0)
                {
                    return Ok(preferences);
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

        /// <summary>Get preferences by professor id</summary>
        /// <remarks>GET request that retrieves the preferences with specified professor id.</remarks>
        [HttpGet("preferences/{professor_id}")]
        public async Task<ActionResult<List<PreferenceDTO>>> GetPreferencesByProfessorId(int professor_id)
        {
            var preferences = new List<PreferenceDTO>();
            try
            {
                // Create query string
                string query = @"SELECT * 
                                 FROM preference 
                                 WHERE professor_id = " + professor_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<PreferenceDTO>(query, CommandType.Text);
                    preferences = result.ToList();
                }
                // If a preference was returned from database, return it
                if (preferences.Count > 0)
                {
                    return Ok(preferences);
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

        /// <summary>Delete preference by preference id</summary>
        /// <remarks>DELETE request that deletes the preference with specified preference id.</remarks>
        [HttpDelete("preferences/delete/id/{preference_id}")]
        public async Task<ActionResult> DeletePreferenceById(int preference_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM preference " +
                                      "WHERE preference_id = " + preference_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preference with id: " + preference_id);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete preferences by professor id</summary>
        /// <remarks>DELETE request that deletes the preferences with specified professor id.</remarks>
        [HttpDelete("preferences/delete/professor/{professor_id}")]
        public async Task<ActionResult> DeletePreferencesByProfessorId(int professor_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM preference " +
                                      "WHERE professor_id = " + professor_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted preferences with professor id: " + professor_id);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new preference</summary>
        /// <remarks>POST request that creates a new preference with the inputted information.</remarks>
        [HttpPost("preferences/create")]
        public async Task<ActionResult> InsertPreference(PreferenceInsertDTO model)
        {
            try
            {
                // create the query string
                string query = @"INSERT INTO preference (professor_id, time_slot_id, preference) " +
                                "VALUES (" + model.professor_id + "," + model.time_slot_id + "," + model.preference + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<PreferenceDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully created preference");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update preference by preference id</summary>
        /// <remarks>PUT request that updates the preference with specified preference id to be set to the new inputted values.</remarks>
        [HttpPut("preferences/update/{preference_id}")]
        public async Task<ActionResult> UpdatePreference(PreferenceDTO model, int preference_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE preference
                                 SET professor_id = " + model.professor_id + ", preference = " + model.preference +
                                 " WHERE preference_id = " + preference_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<PreferenceDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated preference");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
