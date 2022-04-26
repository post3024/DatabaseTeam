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
        Task<AuthenticateResponse> AuthenticateAdminAsync(AuthenticateRequest model);
        Task<AuthenticateResponse> AuthenticateUserAsync(AuthenticateRequest model);
        Task<CreateUserResponse> CreateAdminUserAsync(AuthenticateRequest model);
        Task<User> GetAdminByIdAsync(int id);
        Task<User> GetUserByIdAsync(int id);
        string GeneratePassword(int length, int numberOfNonAlphanumericCharacters);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly string connString;
        private static readonly char[] Punctuations = "!@#$%^&*".ToCharArray();
        private static readonly char[] StartingChars = new char[] { '<', '&' };

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

        public async Task<AuthenticateResponse> AuthenticateAdminAsync(AuthenticateRequest model)
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

        public async Task<AuthenticateResponse> AuthenticateUserAsync(AuthenticateRequest model)
        {
            //grab user by username from users table
            var users = new List<ProfUser>();
            try
            {
                User user = new();
                // create query string
                string query = @"SELECT professor_id, user_email, user_password, salt, user_role
                                 FROM professor
                                 WHERE user_email = '" + model.username + "';";
                using (var connection = new MySqlConnection(connString))
                {
                    // execute query string
                    var result = await connection.QueryAsync<ProfUser>(query, CommandType.Text);
                    users = result.ToList();
                }
                // if the classes exist, return the records
                if (users.Count > 0)
                {
                    user = new User(users[0]);
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

        public async Task<CreateUserResponse> CreateAdminUserAsync(AuthenticateRequest model)
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
                string query = @"INSERT INTO users (user_email, user_password, salt, user_role) " +
                                "VALUES ('" + model.username + "','" + hashed + "','" + saltStr + "','admin');";
                string queryId = @"SELECT LAST_INSERT_ID();";

                using (var connection = new MySqlConnection(connString))
                {
                    // Execute the query
                    var result = await connection.QueryAsync<ProfUser>(query, CommandType.Text);
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

        public async Task<User> GetAdminByIdAsync(int id)
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

        public async Task<User> GetUserByIdAsync(int id)
        {
            var users = new List<ProfUser>();
            try
            {
                // Create query string
                string query = @"SELECT professor_id, user_email, user_password, salt, user_role
                                 FROM professor 
                                 WHERE professor_id = " + id + ";";

                using (var connection = new MySqlConnection(connString))
                {
                    // execute query
                    var result = await connection.QueryAsync<ProfUser>(query, CommandType.Text);
                    users = result.ToList();
                }
                // If classes were returned from database, return them
                if (users.Count > 0)
                {
                    return new User(users[0]);
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
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.user_id.ToString()), new Claim("role", user.user_role) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Code from System.Web.Security under MIT license - https://github.com/Microsoft/referencesource/blob/master/LICENSE.txt
        public  string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            string s;
            int matchIndex;
            do
            {
                var data = new byte[length];
                var chArray = new char[length];
                var num1 = 0;
                new RNGCryptoServiceProvider().GetBytes(data);
                for (var index = 0; index < length; ++index)
                {
                    var num2 = (int)data[index] % 70;
                    if (num2 < 10)
                        chArray[index] = (char)(48 + num2);
                    else if (num2 < 36)
                        chArray[index] = (char)(65 + num2 - 10);
                    else if (num2 < 62)
                    {
                        chArray[index] = (char)(97 + num2 - 36);
                    }
                    else
                    {
                        chArray[index] = Punctuations[num2 - 62];
                        ++num1;
                    }
                }
                if (num1 < numberOfNonAlphanumericCharacters)
                {
                    var random = new Random();
                    for (var index1 = 0; index1 < numberOfNonAlphanumericCharacters - num1; ++index1)
                    {
                        int index2;
                        do
                        {
                            index2 = random.Next(0, length);
                        }
                        while (!char.IsLetterOrDigit(chArray[index2]));
                        chArray[index2] = Punctuations[random.Next(0, Punctuations.Length)];
                    }
                }
                s = new string(chArray);
            }
            while (IsDangerousString(s, out matchIndex));
            return s;
        }

        internal static bool IsDangerousString(string s, out int matchIndex)
        {
            //bool inComment = false;
            matchIndex = 0;

            for (var i = 0; ;)
            {

                // Look for the start of one of our patterns 
                var n = s.IndexOfAny(StartingChars, i);

                // If not found, the string is safe
                if (n < 0) return false;

                // If it's the last char, it's safe 
                if (n == s.Length - 1) return false;

                matchIndex = n;

                switch (s[n])
                {
                    case '<':
                        // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                        if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                        break;
                    case '&':
                        // If the & is followed by a #, it's unsafe (e.g. &#83;) 
                        if (s[n + 1] == '#') return true;
                        break;
                }

                // Continue searching
                i = n + 1;
            }
        }

        private static bool IsAtoZ(char c)
        {
            if ((int)c >= 97 && (int)c <= 122)
                return true;
            if ((int)c >= 65)
                return (int)c <= 90;
            return false;
        }
    }
}