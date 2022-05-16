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
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebAPITest.Services;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace WebAPITest.Controllers
{
    [ApiController]
    [Route("professor-management")]
    public class ProfessorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private IUserService _userService;

        public ProfessorController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            var host = _configuration["DBHOST"] ?? "capstonedb01.mysql.database.azure.com";
            var port = _configuration["DBPORT"] ?? "3306";
            var password = _configuration["MYSQL_PASSWORD"] ?? "DBadmin01!";
            var userid = _configuration["MYSQL_USER"] ?? "capstoneadmin";
            var usersDataBase = _configuration["MYSQL_DATABASE"] ?? "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        /// <summary>Get all professsors</summary>
        /// <remarks>GET request that retrieves all professors.</remarks>
        [HttpGet("professors")]
        [Authorize("admin")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetAllProfessors()
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                // create the query string
                string query = @"SELECT * FROM professor";
                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ProfessorDTO>(query, CommandType.Text);
                    profs = result.ToList();
                }
                // If there are professors, return them
                if (profs.Count > 0)
                {
                    return Ok(profs);
                }
                // Else, respond with 404 error
                else
                {
                    return NotFound();
                }
            }
            // Catch any exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Get professor by professor id</summary>
        /// <remarks>GET request that retrieves the professor with specified professor id.</remarks>
        [HttpGet("professors/{prof_id}")]
        [Authorize("admin")]
        public async Task<ActionResult<List<ProfessorDTO>>> GetProfessorById(string prof_id)
        {
            var profs = new List<ProfessorDTO>();
            try
            {
                // Create the query string
                string query = @"SELECT * 
                                 FROM professor 
                                 WHERE professor_id = @prof_id";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(query, new {prof_id = prof_id});
                    profs = result.ToList();
                }
                // If the prof exists, return the record
                if (profs.Count > 0)
                {
                    return Ok(profs);
                }
                // else, send error
                else
                {
                    return NotFound();
                }
            }
            // Catch any exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Delete professor by professor id</summary>
        /// <remarks>DELETE request that deletes the professor with specified professor id.</remarks>
        [HttpDelete("professors/delete/{prof_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> DeleteProfessorById(string prof_id)
        {
            try
            {
                // Create the query string
                string deleteQuery = @"DELETE FROM professor " +
                                      "WHERE professor_id = @prof_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query string
                    var result = await connection.QueryAsync<ProfessorDTO>(deleteQuery, new
                    {
                        prof_id = prof_id
                    });
                }
                return StatusCode(200, "Successfully deleted " + prof_id);
            }
            // Catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Create a new professor</summary>
        /// <remarks>POST request that creates a new professor with inputted information. Returns all fields for the newly created professor
        /// including the auto-generated professor id and randomly created password for the new user.</remarks>
        [HttpPost("professors/create")]
        [Authorize("admin")]
        public async Task<ActionResult<CreateProfessorDTO>> InsertProfessor(ProfessorInsertDTO model)
        {
            // generate random 32-bit password
            var passwordStr = _userService.GeneratePassword(16, 1);

            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string saltStr = Convert.ToBase64String(salt);

            // hash password for storage
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: passwordStr,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            
            try
            {
                var prof = new CreateProfessorDTO();
                // create query string
                string query = @"INSERT INTO professor (first_name, last_name, teach_load, user_email, user_password, salt, user_role) " +
                                "VALUES (@first_name, @last_name, @teach_load, @user_email,'" + hashed + "','" + saltStr + "', 'user'); " +
                                "SELECT * FROM professor WHERE professor_id = LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute the query string
                    var result = await connection.QueryAsync<CreateProfessorDTO>(query, new { first_name = model.first_name, last_name = model.last_name, teach_load = model.teach_load, user_email = model.user_email});
                    prof = result.ToList()[0];
                }
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("classyscheduleUST@gmail.com"));
                email.To.Add(MailboxAddress.Parse(model.user_email));
                email.Subject = "Login Information for ClassySchedule";
                string messageText = "Hello " + model.first_name + " " + model.last_name + ",\n\nYour University of St. Thomas administrator has created an account for you with ClassySchedule. " +
                    "Please use your St. Thomas email and this temporary password to login.\n\n" + "Password = " + passwordStr + "\n\nFeel free to change this password in your account settings.\n\nThank you,\nClassySchedule";
                email.Body = new TextPart(TextFormat.Plain) { Text = messageText };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("classyscheduleUST@gmail.com", "@ryb5rgLQb7J");
                smtp.Send(email);
                smtp.Disconnect(true);

                return Ok(prof);
            }
            //catch exception
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>Update professor by professor id</summary>
        /// <remarks>PUT request that updates the professor with specified professor id to be set to the new inputted values.</remarks>
        [HttpPut("professors/update/{prof_id}")]
        [Authorize("admin")]
        public async Task<ActionResult> UpdateProfessor(ProfessorInsertDTO model, int prof_id)
        {
            try
            {
                // create the query string
                string query = @"UPDATE professor
                                 SET first_name = @first_name, last_name = @last_name, teach_load = @teach_load" +
                                 ", user_email = @user_email WHERE professor_id = @prof_id;";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync(query, new { first_name = model.first_name, last_name = model.last_name, teach_load = model.teach_load, user_email = model.user_email, prof_id = prof_id});
                }
                return StatusCode(200, "Successfully updated professor");
            }
            // catch the exceptions
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}