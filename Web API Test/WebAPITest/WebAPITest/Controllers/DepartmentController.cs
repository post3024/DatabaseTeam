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
    [Route("department-management")]
    public class DepartmentController : ControllerBase
    {
    //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IConfiguration configuration, ILogger<DepartmentController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }
        
        [HttpGet("departments")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments()
        {
            var depts = new List<DepartmentDTO>();
            try
            {
                string query = @"SELECT dept_name FROM department";
                using (var connection = new MySqlConnection(connString))
                {
                    
                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);    
                    depts = result.ToList();
                }
                if (depts.Count > 0)
                {
                    return Ok(depts);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        [HttpPost("departments")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetDepartmentByName(String dept_name) {
            var depts = new List<DepartmentDTO>();
            try
            {
                string query = @"SELECT dept_name FROM department WHERE dept_name = '" + dept_name + "'";

                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<DepartmentDTO>(query, CommandType.Text);
                    depts = result.ToList();
                }
                if (depts.Count > 0)
                {
                    return Ok(depts);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}
