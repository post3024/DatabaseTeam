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
    [Route("plan-management")]
    public class PlanController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;

        public PlanController (IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }


        /// <summary>Get all plans</summary>
        /// <remarks>GET request that retrieves all plans.</remarks>
        [HttpGet("plans")]
        public async Task<ActionResult<List<PlanDTO>>> GetAllPlans()
        {
            var plans = new List<PlanDTO>();
            try
            {
                // create query string
                string query = @"SELECT * FROM plan";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<PlanDTO>(query, CommandType.Text);
                    plans = result.ToList();
                }
                // if there are any plans, return the records
                if (plans.Count > 0)
                {
                    return Ok(plans);
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

        /// <summary>Get plan by plan id</summary>
        /// <remarks>GET request that retrieves the plan with specified plan id.</remarks>
        [HttpGet("plans/{plan_id}")]
        public async Task<ActionResult<List<PlanDTO>>> GetPlanByPlanId(int plan_id)
        {
            var plan = new List<PlanDTO>();
            try
            {
                // Create query string
                string query = @"SELECT * 
                                 FROM plan 
                                 WHERE plan_id = " + plan_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<PlanDTO>(query, CommandType.Text);
                    plan = result.ToList();
                }
                // If plan exists, return it
                if (plan.Count > 0)
                {
                    return Ok(plan);
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

        /// <summary>Delete plan by plan id</summary>
        /// <remarks>DELETE request that deletes the plan with specified plan id.</remarks>
        [HttpDelete("plans/delete/{plan_id}")]
        public async Task<ActionResult> DeletePlanById(int plan_id)
        {
            try
            {
                // create query string
                string deleteQuery = @"DELETE FROM plan " +
                                      "WHERE plan_id = " + plan_id;

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync(deleteQuery, CommandType.Text);
                }
                return StatusCode(200, "Successfully deleted plan with id: " + plan_id);
            }
            // catch exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new plan</summary>
        /// <remarks>POST request that creates a new plan with the inputted information.</remarks>
        [HttpPost("plans/create")]
        public async Task<ActionResult> InsertPlan(PlanInsertDTO model)
        {
            try
            {
                // create the query string
                string query = @"INSERT INTO plan (plan_name, plan_description, semester_year, semester_num) " +
                                "VALUES ('" + model.plan_name + "','" + model.plan_description + "'," + model.semester_year + "," + model.semester_num + ");";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<PlanDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully created plan");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update plan by plan id</summary>
        /// <remarks>PUT request that updates the plan with specified plan number to be set to the new inputted values.</remarks>
        [HttpPut("plans/update/{plan_id}")]
        public async Task<ActionResult> UpdatePlan(PlanDTO model, int plan_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE plan
                                 SET plan_id = " + model.plan_id + ", plan_name = '" + model.plan_name + "', plan_description = '" + model.plan_description +
                                 "', semester_year = " + model.semester_year + ", semester_num = " + model.semester_num +
                                 " WHERE plan_id = " + plan_id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<PlanDTO>(query, CommandType.Text);
                }
                return StatusCode(200, "Successfully updated plan");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
