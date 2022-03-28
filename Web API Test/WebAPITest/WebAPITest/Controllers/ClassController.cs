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
    [Route("class-management")]
    public class ClassController : ControllerBase
    {
        //code here
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private readonly ILogger<ClassController> _logger;

        public ClassController(IConfiguration configuration, ILogger<ClassController> logger)
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

        [HttpGet("classes")]
        public async Task<ActionResult<List<ClassDTO>>> GetAllClasses()
        {
            var classes = new List<ClassDTO>();
            try
            {
                string query = @"SELECT class_num, dept_id, class_name, capacity, credits FROM class";
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                if (classes.Count > 0)
                {
                    return Ok(classes);
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
