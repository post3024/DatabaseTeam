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
    [Route("class-management")]
    public class ClassController : ControllerBase
    {
        //code here
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


        /**
         * This get method will return all class records in the database
         */
        [HttpGet("classes")]
        public async Task<ActionResult<List<ClassDTO>>> GetAllClasses()
        {
            var classes = new List<ClassDTO>();
            try
            {
                //create query string
                string query = @"SELECT * FROM class";
                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                //if the classes exist, return the records
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
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method will get a class based on the inputed class_name and class_num
         */
        [HttpPost("class/NameAndNumber")]
        public async Task<ActionResult<List<ClassDTO>>> GetClassByNameAndNumber (string class_name, string class_num)
        {
            var classes = new List<ClassDTO>();
            try
            {
                //Create query string
                string query = @"SELECT * 
                                 FROM class 
                                 WHERE class_name = '" + class_name + "' " +
                                  "AND class_num = " + class_num;

                using (var connection = new MySqlConnection(connString))
                {
                    //execute query
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                    classes = result.ToList();
                }
                //If classes were returned from database, return them
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
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This method will delete a class record based on the inputed class_name and class_num
         */
        [HttpPost("class/delete")]
        public async Task<ActionResult<List<ClassDTO>>> DeleteClassByNameAndNumber(string class_name, string class_num)
        {
            try
            {
                //create query string
                string deleteQuery = @"DELETE FROM class " +
                                      "WHERE class_name = '" + class_name + "' " +
                                        "AND class_num = " + class_num;

                using (var connection = new MySqlConnection(connString))
                {
                    //execute query string
                    var result = await connection.QueryAsync<DepartmentDTO>(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted " + class_num + ' ' + class_name);
            }
            //catch exceptions
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        /**
         * This post method will create a new class based on the inputed class_num, dept_id, class_name, capacity, and credits
         */
        [HttpPost("class/create")]
        public async Task<ActionResult<List<ClassDTO>>> InsertClass(string class_num, string dept_id, string class_name, string capacity, string credits)
        {
            try
            {
                //create the query string
                string query = @"INSERT INTO class (class_num, dept_id, class_name, capacity, credits) " +
                                "VALUES (" + class_num + "," + dept_id + ",'" + class_name + "'," + capacity + "," + credits + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    //Execute the query
                    var result = await connection.QueryAsync<ClassDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully created class ");
            }
            //catch the exceptions
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}
