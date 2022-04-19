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
    [Route("professor-management")]
    public class ProfessorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ProfessorController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all professsors</summary>
        /// <remarks>GET request that retrieves all professors.</remarks>
        [HttpGet("professors")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetAllProfessors()
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                // create the query string
                string query = @"SELECT professor_id, first_name, last_name, teach_load FROM professor";
                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                // If there are professors, return them
                if (profs.Count > 0)
                {
                    return Ok(profs);
                }
                // Else, respond with 404 error
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

        /// <summary>Get professor by professor id</summary>
        /// <remarks>GET request that retrieves the professor with specified professor id.</remarks>
        [HttpGet("professors/{prof_id}")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetProfessorById(string prof_id)
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM professor 
                                 WHERE professor_id = '" + prof_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                // If the prof exists, return the record
                if (profs.Count > 0)
                {
                    return Ok(profs);
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

        /// <summary>Delete professor by professor id</summary>
        /// <remarks>DELETE request that deletes the professor with specified professor id.</remarks>
        [HttpDelete("professors/delete/{prof_id}")]
        public async Task<ActionResult> DeleteProfessorById(string prof_id)
        {
            try
            {
                // Create the query string
                string deleteQuery = @"DELETE FROM professor " +
                                      "WHERE professor_id = '" + prof_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted " + prof_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new professor</summary>
        /// <remarks>POST request that creates a new professor with inputted information. Returns the auto-generated professor id for the newly added professor.</remarks>
        [HttpPost("professors/create")]
        public async Task<ActionResult> InsertProfessor(ProfessorInsertDTO model)
        {
            try
            {
                // create query string
                string query = @"INSERT INTO professor (first_name, last_name, teach_load) " +
                                "VALUES ('" + model.first_name + "','" + model.last_name + "'," + model.teach_load + ");";
                string queryId = @"SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    var id = await connection.QueryAsync<String>(queryId, CommandType.Text);
                    String prof_id = id.ToList()[0];
                    return StatusCode(200, "Successfully created " + model.first_name + " " + model.last_name + " with prof_id = " + prof_id);
                }                
            }
            //catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update professor by professor id</summary>
        /// <remarks>PUT request that updates the professor with specified professor id to be set to the new inputted values.</remarks>
        [HttpPut("professors/update/{prof_id}")]
        public async Task<ActionResult> UpdateProfessor(ProfessorDTO model, int prof_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE professor
                                 SET first_name = '" + model.first_name + "', last_name = '" + model.last_name + "', teach_load = " + model.teach_load +
                                 " WHERE prof_id = " + prof_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated professor");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}