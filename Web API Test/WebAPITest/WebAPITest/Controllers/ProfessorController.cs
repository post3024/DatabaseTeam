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
        private readonly ILogger<ProfessorController> _logger;

        public ProfessorController(IConfiguration configuration, ILogger<ProfessorController> logger)
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

        [HttpGet("professors")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetAllProfessors()
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                string query = @"SELECT professor_id, first_name, last_name, teach_load FROM professor";
                using (var connection = new MySqlConnection(connString))
                {

                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                if (profs.Count > 0)
                {
                    return Ok(profs);
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