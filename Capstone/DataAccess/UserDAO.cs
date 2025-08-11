using Capstone.Interface;
using Capstone.Models;
using Microsoft.Data.SqlClient;


namespace Capstone.DataAccess
{
    public class UserDAO : IUserManager
    {

        //Define the conncetion string for MSSQL 
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAuth;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";



        //define the connection string for MySQL
        //static string serverName = "localHost";
        //static string username = "root";
        //static string password = "root";
        //static string dbname = "userauth";
        //static string port = "3306";

        //set the connection string
        //string interpolation
        //static string connStr = $"server={serverName};user={username};password={password};database={dbname};port={port};";

        SqlConnection connection = new SqlConnection(conn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(UserModel user)
        {//declare and initialize
            string query = "";
            int result = -1;

            //using is the opening and closing of a connection
            //garbage collector to assist with managing resources
            //creates a new SQL connection 
            using (connection)
            {
                //open the connection to the database
                connection.Open();

                //define the SQL query
                //to prevent SQL injection attacks
                query = @"INSERT INTO UserAccount 
                        (UserName, Password, Email, FirstName, LastName, PhoneNumber, IsAdmin) 
                        VALUES 
                        (@UserName, @Password, @Email, @FirstName, @LastName, @PhoneNumber, @IsAdmin);
                        SELECT SCOPE_IDENTITY();";

                //Create a SQL command object using the query and open connection 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //add parameters to the command to safely pass user input
                    //helping us avoid SQL injections
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);

                    //Execute the query and retrieve the new inserted ID using ExecuteScalar
                    if (int.TryParse(command.ExecuteScalar()?.ToString(), out result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to retrieve the inserted ID.");
                    }
                }
            }
        }

        /// <summary>
        /// This method will be used to verify a login attempt
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserModel CheckCredentials(string username, string password)
        {
            //declare and initialize 
            string query = "";

            //open a new SQL connection with using string
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                UserModel user = null;

                query = "SELECT * FROM UserAccount WHERE Username = @UserName AND Password = @MyPassword";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@MyPassword", password);

                    using (SqlDataReader reader = command.ExecuteReader())//ExecuteReader() does execute the command's query and populates results into the reader object
                    {
                        //check if any records were returned
                        if (reader.Read())
                        {
                            user = new UserModel()
                            {
                                //GetOrdinal lets you decide which column in the current row you wish to take data from
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
                            };
                        }
                        bool valid = false;
                        if (user != null) { 
                            valid = user.VerifyPassword(password);
                        }
                        //Empty usermodel for a null return
                        UserModel user2 = new UserModel() { Id = 0 };
                        return valid == true ? user : user2; // checks if password is correct for the returned user, return user.id or 0
                    }
                }
            }
        }//end of CheckCred

        /// <summary>
        /// Delete a user from the UserAccount table by Id
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(UserModel user)
        {
            string query = "";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                query = "DELETE FROM UserAccount WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Create a list returning each user as a usermodel from the UserAccount table
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();
            string query = "";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                query = "SELECT * FROM UserAccount";
                using (SqlCommand command = new SqlCommand(@query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())//ExecuteReader() does execute the command's query and populates results into the reader object
                    {
                        //check if any records were returned
                        if (reader.Read())
                        {
                            UserModel user = new UserModel()
                            {
                                //GetOrdinal lets you decide which column in the current row you wish to take data from
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
                            };
                            users.Add(user);
                        }
                    }
                    return users;
                }
            }
        }

        /// <summary>
        /// Get a user by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            string query = "";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                query = "SELECT * FROM UserAccount WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())//ExecuteReader() does execute the command's query and populates results into the reader object
                    {
                        //check if any records were returned
                        if (reader.Read())
                        {
                            UserModel user = new UserModel()
                            {
                                //GetOrdinal lets you decide which column in the current row you wish to take data from
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
                            };
                            return user;

                        }
                        else
                        {
                            //throw new InvalidOperationException(string.Format("Failed to retrieve the designated Id {0}", id));
                            UserModel user2 = new UserModel()
                            {
                                Id = 0
                            };
                            return user2;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update a table row with a full user model passed 
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(UserModel user)
        {
            string query = "";
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                //string format query for usermodel properties and table columns
                query = string.Format("Update UserAccount (Id, UserName, Password, Email, FirstName, LastName, PhoneNumber, IsAdmin) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", user.Id, user.UserName, user.Password, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.IsAdmin);

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Execute the query and returns how many lines were affected
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
