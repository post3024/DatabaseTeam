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
    [Route("professor-management")]
    public class ProfessorController : ControllerBase
    {
        //code here
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

        /**
         * This get request will gather all professor records that exist in the database
         */
        [HttpGet("professor")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetAllProfessors()
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                //create the query string
                string query = @"SELECT professor_id, first_name, last_name, teach_load FROM professor";
                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                //If there are professors, return them
                if (profs.Count > 0)
                {
                    return Ok(profs);
                }
                //Else, respond with 404 error
                else
                {
                    return NotFound();
                }
            }
            //Catch any exceptions
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method will return a professor record with the inputed prof_id
         */
        [HttpPost("professor/id")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetProfessorById(string prof_id)
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                //Create the query string
                string query = @"SELECT * 
                                 FROM professor 
                                 WHERE professor_id = '" + prof_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                //If the prof exists, return the record
                if (profs.Count > 0)
                {
                    return Ok(profs);
                }
                //else, send error
                else
                {
                    return NotFound();
                }
            }
            //Catch any exceptions
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method deletes a professor record based on the prof_id
         */
        [HttpPost("professor/delete")]
        public async Task<ActionResult<List<ProfessorDTO>>> DeleteProfessorById(string prof_id)
        {
            try
            {
                //Create the query string
                string deleteQuery = @"DELETE FROM professor " +
                                      "WHERE professor_id = '" + prof_id + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted " + prof_id);
            }
            //Catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method will create a new professor record based on the inputed first_name, last_name, and teach_load
         */
        [HttpPost("professor/create")]
        public async Task<ActionResult<List<ProfessorDTO>>> InsertClass(string first_name, string last_name, string teach_load)
        {
            try
            {
                //create query string
                string query = @"INSERT INTO professor (first_name, last_name, teach_load) " +
                                "VALUES ('" + first_name + "','" + last_name + "'," + teach_load + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    //execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                }

                return StatusCode(200, "Successfully created " + first_name + " " + last_name);
            }
            //catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}