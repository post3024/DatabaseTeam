using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebAPITest.Helpers;
using MySql.Data.MySqlClient;
using Dapper;
using WebAPITest.Models;
using System.Data;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

// Some code taken from https://github.com/cornflourblue/dotnet-5-jwt-authentication-api
namespace WebAPITest.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<CreateUserResponse> CreateUserAsync(AuthenticateRequest model);
        Task<User> GetByIdAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly string connString;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            var host = "capstonedb01.mysql.database.azure.com";
            var port = "3306";
            var password = "DBadmin01!";
            var userid = "capstoneadmin";
            var usersDataBase = "classyschedule";

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            //grab user by username from users table
            var users = new List<User>();
            try
            {
                User user = new User();
                // create query string
                string query = @"SELECT * 
                                 FROM users
                                 WHERE user_email = '" + model.username + "';";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<User>(query, CommandType.Text);
                    users = result.ToList();
                }
                // if the classes exist, return the records
                if (users.Count > 0)
                {
                    user = users[0];
                }
                else
                {
                    return null;
                }

                //grab salt and then hash the given password
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: model.password,
                        salt: Convert.FromBase64String(user.salt),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));

                //compare salted and hashed password to the one stored in db
                if (hashed.Equals(user.user_password))
                {
                    // authentication successful so generate jwt token
                    var token = generateJwtToken(user);

                    return new AuthenticateResponse(user, token);
                }
            }
            // catch exception
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public async Task<CreateUserResponse> CreateUserAsync(AuthenticateRequest model)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: model.password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            string saltStr = Convert.ToBase64String(salt);
            try
            {
                // create the query string
                string query = @"INSERT INTO users (user_email, user_password, salt) " +
                                "VALUES ('" + model.username + "','" + hashed + "','" + saltStr + "');";
                string queryId = @"SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<User>(query, CommandType.Text);
                    var id = await connection.QueryAsync<String>(queryId, CommandType.Text);
                    int user_id = Convert.ToInt32(id.ToList()[0]);
                    return new CreateUserResponse(user_id, model.username);
                }
            }
            // catch the exceptions
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var users = new List<User>();
            try
            {
                // Create query string
                string query = @"SELECT * 
                                 FROM users 
                                 WHERE user_id = " + id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<User>(query, CommandType.Text);
                    users = result.ToList();
                }
                // If classes were returned from database, return them
                if (users.Count > 0)
                {
                    return users[0];
                }
                else
                {
                    return null;
                }
            }
            //catch exception
            catch (Exception)
            {
                return null;
            }
        }

        // helper methods
        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.user_id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}