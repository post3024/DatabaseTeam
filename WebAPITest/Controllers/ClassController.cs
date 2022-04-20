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
    //[Authorize]
    [ApiController]
    [Route("class-management")]
    public class ClassController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public ClassController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }


        /// <summary>Get all classes</summary>
        /// <remarks>GET request that retrieves all classes.</remarks>
        [HttpGet("classes")]
        public async Task<ActionResult<List<ClassDTO>>> GetAllClasses()
        {
            var classes = new List<ClassDTO>();
            try
            {
                // create query string
                string query = @"SELECT * FROM class";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                // if the classes exist, return the records
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

        /// <summary>Get class by class number and department id</summary>
        /// <remarks>GET request that retrieves the class with specified class number and department id.</remarks>
        [HttpGet("classes/{class_num}/{dept_id}")]
        public async Task<ActionResult<List<ClassDTO>>> GetClassByDeptAndNumber (int class_num, int dept_id)
        {
            var classes = new List<ClassDTO>();
            try
            {
                // Create query string
                string query = @"SELECT * 
                                 FROM class 
                                 WHERE class_num = " + class_num + " " +
                                    "AND dept_id = " + dept_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                // If classes were returned from database, return them
                if (classes.Count > 0)
                {
                    return Ok(classes);
                }
                else
                {
                    return NotFound();
                }
            }
            //catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete class by class number</summary>
        /// <remarks>DELETE request that deletes the class with specified class number.</remarks>
        [HttpDelete("classes/delete/{class_num}/{dept_id}")]
        public async Task<ActionResult> DeleteClassByNameAndNumber(int class_num, int dept_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM class " +
                                      "WHERE class_num = " + class_num + " " +
                                        "AND dept_id = " + dept_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted class " + class_num);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new class</summary>
        /// <remarks>POST request that creates a new class with the inputted information.</remarks>
        [HttpPost("classes/create")]
        public async Task<ActionResult> InsertClass(ClassDTO model)
        {
            try
            {
                // create the query string
                string query = @"INSERT INTO class (class_num, dept_id, class_name, capacity, credits) " +
                                "VALUES (" + model.class_num + "," + model.dept_id + ",'" + model.class_name + "'," + model.capacity + "," + model.credits + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully created class " + model.class_num);
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update class by class number and department id</summary>
        /// <remarks>PUT request that updates the class with specified class number and department id to be set to the new inputted values.</remarks>
        [HttpPut("classes/update/{class_num}/{dept_id}")]
        public async Task<ActionResult> UpdateClass(ClassDTO model, int class_num, int dept_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE class
                                 SET class_num = " + model.class_num + ", dept_id = " + model.dept_id + ", class_name = '" + model.class_name +
                                 "', capacity = " + model.capacity + ", credits = " + model.credits +
                                 " WHERE class_num = " + class_num + " AND dept_id = " + dept_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated class");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
