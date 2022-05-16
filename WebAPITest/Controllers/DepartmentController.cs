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
    [Route("department-management")]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all departments</summary>
        /// <remarks>GET request that retrieves all departments.</remarks>
        [HttpGet("departments")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments()
        {
            var depts = new List<DepartmentDTO>();
            try
            {
                // create query string
                string query = @"SELECT * FROM department";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);    
                    depts = result.ToList();
                }
                // if departments exist, return them
                if (depts.Count > 0)
                {
                    return Ok(depts);
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

        /// <summary>Get department by department id</summary>
        /// <remarks>GET request that retrieves the department with specified department id.</remarks>
        [HttpGet("departments/{dept_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetDepartmentByName(int dept_id) {
            var depts = new List<DepartmentDTO>();
            try
            {
                // create query string
                string query = @"SELECT * FROM department WHERE dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(query, new { dept_id=dept_id});
                    depts = result.ToList();
                }
                // if departments exist, return them
                if (depts.Count > 0)
                {
                    return Ok(depts);
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

        /// <summary>Delete department by department id</summary>
        /// <remarks>DELETE request that deletes the department with specified department id.</remarks>
        [HttpDelete("departments/delete/{dept_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteDepartmentByName(int dept_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM department
                                 WHERE dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, new { dept_id = dept_id });
                }
                return StatusCode(200, "Successfully deleted department " + dept_id);
            }
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new department</summary>
        /// <remarks>POST request that creates a new department with the inputted information. Returns the auto-generated department id for the newly added department.</remarks>
        [HttpPost("departments/create")]
        [Authorize("admin")]
        public async Task<ActionResult<DepartmentDTO>> InsertDepartment(String dept_name)
        {
            try
            {
                // Create query string
                string query = @"INSERT INTO department (dept_name) " +
                                "VALUES (@dept_name);" +
                                "SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var id = await connection.QueryAsync<int>(query, new { dept_name = dept_name});
                    int dept_id = id.ToList()[0];
                    DepartmentDTO newDept = new(dept_id, dept_name);
                    return Ok(newDept);
                }
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update department by department id</summary>
        /// <remarks>PUT request that updates the department with specified department id to be set to the new inputted values.</remarks>
        [HttpPut("departments/update/{dept_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> UpdateDepartment(int dept_id, DepartmentDTO model)
        {
            try
            {
                // create the query string
                string query = @"UPDATE department
                                 SET dept_id = @new_dept_id, dept_name = @dept_name "+
                                 "WHERE dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new { new_dept_id = dept_id, dept_id = model.dept_id, dept_name = model.dept_name });
                }
                return StatusCode(200, "Successfully updated department");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
