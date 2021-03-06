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
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ClassDTO>>> GetAllClasses()
        {
            var classes = new List<ClassDTO>();
            try
            {
                // create query string
                var query = @"SELECT * 
                                 FROM class";
                                 
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
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ClassDTO>>> GetClassByDeptAndNumber (int class_num, int dept_id)
        {
            var classes = new List<ClassDTO>();
            try
            {
                // Create query string
                var query = @"SELECT * 
                                 FROM class 
                                 WHERE class_num = @class_num 
                                 AND dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassDTO>(query, new { class_num=class_num, dept_id =dept_id});
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
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get class by class id</summary>
        /// <remarks>GET request that retrieves the classes with specified id</remarks>
        [HttpGet("classes/{class_id}")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<ClassDTO>>> GetClassById(int class_id)
        {
            var classes = new List<ClassDTO>();
            try
            {
                // Create query string
                var query = @"SELECT * 
                                 FROM class 
                                 WHERE class_id = @class_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ClassDTO>(query, new { class_id= class_id});
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
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>Get classes joined with department</summary>
        /// <remarks>GET request that retrieves the class with department name.</remarks>
        [HttpGet("classes/department")]
        [Authorize("admin", "user")]
        public async Task<ActionResult<List<DepartmentClassDTO>>> GetClassAndDepartmentTables()
        {
            var classes = new List<DepartmentClassDTO>();
            try
            {
                // Create query string
                var query = @"SELECT * 
                              FROM department INNER JOIN class
                                ON department.dept_id = class.dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<DepartmentClassDTO>(query, CommandType.Text);
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
            // catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }



        /// <summary>Delete class by class number</summary>
        /// <remarks>DELETE request that deletes the class with specified class number and department id.</remarks>
        [HttpDelete("classes/delete/{class_num}/{dept_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteClassByNameAndNumber(int class_num, int dept_id)
        {
            try
            {
                // create query string
                var deleteQuery = @"DELETE FROM class " +
                                   "WHERE class_num = @class_num " +
                                     "AND dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, new { class_num = class_num, dept_id = dept_id });
                }
                return StatusCode(200, "Successfully deleted class " + class_num);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete class by class id</summary>
        /// <remarks>DELETE request that deletes the class with specified class id</remarks>
        [HttpDelete("classes/delete/{class_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteClassById(int class_id)
        {
            try
            {
                // create query string
                var deleteQuery = @"DELETE FROM class " +
                                   "WHERE class_id = @class_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, new {class_id=class_id});
                }
                return StatusCode(200, "Successfully deleted class with id: " + class_id);
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
        [Authorize("admin", "user")]
        public async Task<ActionResult> InsertClass(ClassInsertDTO model)
        {
            try
            {
                // create the query string
                var query = @"INSERT INTO class (" +
                                    "class_num," +
                                    "dept_id," +
                                    "class_name," +
                                    "capacity," +
                                    "credits," +
                                    "is_lab," +
                                    "num_sections) " +
                             "VALUES (@class_num, @dept_id, @class_name, @capacity, @credits, @is_lab, @num_sections);" +
                             "SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<int>(query, new
                    {
                        class_num = model.class_num,
                        dept_id = model.dept_id,
                        class_name = model.class_name,
                        capacity = model.capacity,
                        credits = model.credits,
                        is_lab = model.is_lab,
                        num_sections = model.num_sections
                    });
                    int class_id = result.ToList()[0];

                    // Create new ClassDTO object and return it
                    ClassDTO newClass = new(
                        class_id,
                        model.class_num,
                        model.dept_id,
                        model.class_name,
                        model.capacity,
                        model.credits,
                        model.is_lab,
                        model.num_sections
                    );
                    return Ok(newClass);
                }
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
        [Authorize("admin")]
        public async Task<ActionResult> UpdateClass(ClassInsertDTO model, int class_num, int dept_id)
        {
            try
            {
                // create the query string
                var query = @"UPDATE class
                                 SET class_num = @class_num" +
                                 ", dept_id = @dept_id " +
                                 ", class_name = @class_name" +
                                 ", capacity = @capacity" +
                                 ", credits = @credits" +
                                 ", is_lab = @is_lab" +
                                 ", num_sections = @num_sections" +
                            " WHERE class_num = @class_num" +
                                 "AND dept_id = @dept_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ClassDTO>(query, new
                    {
                        class_num = model.class_num,
                        dept_id = model.dept_id,
                        class_name = model.class_name,
                        capacity = model.capacity,
                        credits = model.credits,
                        is_lab = model.is_lab,
                        num_sections = model.num_sections
                    });
                }
                return StatusCode(200, "Successfully updated class");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update class by class id</summary>
        /// <remarks>PUT request that updates the class with specified class id to be set to the new inputted values.</remarks>
        [HttpPut("classes/update/{class_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> UpdateClassById(ClassInsertDTO model, int class_id)
        {
            try
            {
                // create the query string
                var query = @"UPDATE class
                                 SET class_num = @class_num" + 
                                 ", dept_id = @dept_id "+ 
                                 ", class_name = @class_name" +
                                 ", capacity = @capacity" + 
                                 ", credits = @credits" + 
                                 ", is_lab = @is_lab" +
                                 ", num_sections = @num_sections" +
                            " WHERE class_id = @class_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ClassDTO>(query, new { class_num=model.class_num,
                        dept_id = model.dept_id, 
                        class_name = model.class_name,
                        capacity = model.capacity,
                        credits = model.credits,
                        is_lab = model.is_lab,
                        class_id = class_id,
                        num_sections = model.num_sections});
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
