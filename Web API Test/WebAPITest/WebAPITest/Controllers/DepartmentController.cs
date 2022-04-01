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
        
        /**
         * Get all of the department records
         */
        [HttpGet("departments")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments()
        {
            var depts = new List<DepartmentDTO>();
            try
            {
                //create query string
                string query = @"SELECT * FROM department";
                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);    
                    depts = result.ToList();
                }
                //if departments exist, return them
                if (depts.Count > 0)
                {
                    return Ok(depts);
                }
                else
                {
                    return NotFound();
                }
            }
            //catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post request will gather a department record based on the inputed dept_name
         */ 
        [HttpPost("departments/name")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetDepartmentByName(String dept_name) {
            var depts = new List<DepartmentDTO>();
            try
            {
                //create query string
                string query = @"SELECT * FROM department WHERE dept_name = '" + dept_name + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);
                    depts = result.ToList();
                }
                //if departments exist, return them
                if (depts.Count > 0)
                {
                    return Ok(depts);
                }
                else
                {
                    return NotFound();
                }
            }
            //catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method will delete a department record based on the inputed dept_name
         */
        [HttpPost("departments/delete")]
        public async Task<ActionResult<List<DepartmentDTO>>> DeleteDepartmentByName(string dept_name)
        {
            try
            {
                //create query string
                string deleteQuery = @"DELETE FROM department
                                 WHERE dept_name = '" + dept_name + "'";

                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted " + dept_name);
            }
            //catch exception
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
        
        /**
         * This post method will create a new department record based on the inputed dept_name
         */
        [HttpPost("departments/create")]
        public async Task<ActionResult<List<DepartmentDTO>>> InsertDepartment(string dept_name)
        {
            try
            {
                //Create query string
                string query = @"INSERT INTO department (dept_name) " +
                                "VALUES ('" + dept_name + "');";

                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully created department ");
            }
            //catch exceptions
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}
