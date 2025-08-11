using Capstone.Interface;
using Capstone.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Capstone.DataAccess
{
    public class TicketDAO : ITicketManager
    {
        //Define the conncetion string for MSSQL 
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog = UserAuth; Integrated Security = True; Connect Timeout = 30; Encrypt=False;Trust Server Certificate=False;Application Intent = ReadWrite; Multi Subnet Failover=False";




        /// <summary>
        /// Add a new ticket to the database
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int AddTicket(TicketModel ticket)
        {
            //define base variables for the query and result
            string query = "";
            int result = -1;

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //Define the query to insert a new ticket into the database
                query = "INSERT INTO Tickets (CreationDate, Subject, Info, Priority, CreationId, Comments) " +
                        "VALUES (@CreationDate, @Subject, @Info, @Priority, @CreationId, @Comments); " +
                        "SELECT SCOPE_IDENTITY();";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@CreationDate", ticket.CreationDate);
                    command.Parameters.AddWithValue("@Subject", ticket.Subject);
                    command.Parameters.AddWithValue("@Info", ticket.Info);
                    command.Parameters.AddWithValue("@Priority", ticket.Priority);
                    command.Parameters.AddWithValue("@CreationId", ticket.CreationId);

                    // Handle Comments as a JSON string or similar format if needed
                    // Autocomplete assisted with this likely using a newer or updated Newtonsoft.Json version, I personally have not used this package before ZChermak
                    var json = ticket.Comments != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ticket.Comments) : null;

                    //Add Comments parameter, handling null values
                    command.Parameters.AddWithValue("@Comments", (object)json ?? DBNull.Value);


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
        /// Delete a ticket from the database
        /// </summary>
        /// <param name="ticket"></param>
        public int DeleteTicket(TicketModel ticket)
        {
            //define base variables for the query
            string query = "";
            int result = -1;

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define the query to delete a ticket from the database
                query = "DELETE FROM Tickets WHERE TicketId = @Id";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@Id", ticket.Id);
                    result = command.ExecuteNonQuery(); // Execute the query to delete the ticket
                    return result;

                }

            }
        }

        /// <summary>
        /// Get all tickets assigned to an admin
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<TicketModel> GetAllAdminTickets(int adminId)
        {
            //Define a base variable for the query and a list to hold the results
            string query = "";
            List<TicketModel> tickets = new List<TicketModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //Define the query to select all tickets assigned to a specific admin
                query = "SELECT * FROM Tickets WHERE AdminId = @AdminId;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@AdminId", adminId);

                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketModel ticket = new TicketModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Info = reader.GetString(reader.GetOrdinal("Info")),
                                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                                CreationId = reader.GetInt32(reader.GetOrdinal("CreationId")),
                                AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId")),
                                LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")))
                            };
                            tickets.Add(ticket);
                        }
                    }
                    return tickets;

                }

            }
        }

        /// <summary>
        /// Get all tickets from the database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<TicketModel> GetAllTickets()
        {
            //define variables for query and ticket list 
            string query = "";
            List<TicketModel> tickets = new List<TicketModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //Define the query to select all tickets from the database
                query = "SELECT * FROM Tickets;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketModel ticket = new TicketModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Info = reader.GetString(reader.GetOrdinal("Info")),
                                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                                CreationId = reader.GetInt32(reader.GetOrdinal("CreationId")),
                                AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId")),
                                LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")))
                            };
                            tickets.Add(ticket);
                        }
                    }
                    return tickets;

                }

            }

        }

        /// <summary>
        /// Get all tickets by the creation ID of the user that created the ticket
        /// </summary>
        /// <param name="creationId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<TicketModel> GetAllTicketsByCreationId(int creationId)
        {
            //Define a base variable for the query and a list to hold the results
            string query = "";
            List<TicketModel> tickets = new List<TicketModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define query for all tickets by a creationId
                query = "SELECT * FROM Tickets WHERE CreationId = @CreationId;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@CreationId", creationId);

                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketModel ticket = new TicketModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Info = reader.GetString(reader.GetOrdinal("Info")),
                                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                                CreationId = reader.GetInt32(reader.GetOrdinal("CreationId")),
                                AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId")),
                                LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")))
                            };
                            tickets.Add(ticket);
                        }
                    }
                    return tickets;

                }

            }
        }

        /// <summary>
        /// Get all tickets by a specific priority level
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<TicketModel> GetAllTicketsByPriority(int priority)
        {
            //Define a base variable for the query and a list to hold the results
            string query = "";
            List<TicketModel> tickets = new List<TicketModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define query for all tickets by a priority level
                query = "SELECT * FROM Tickets WHERE Priority = @Priority;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@Priority", priority);

                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketModel ticket = new TicketModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Info = reader.GetString(reader.GetOrdinal("Info")),
                                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                                CreationId = reader.GetInt32(reader.GetOrdinal("CreationId")),
                                AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId")),
                                LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")))
                            };
                            tickets.Add(ticket);
                        }
                    }
                    return tickets;

                }

            }
        }

        /// <summary>
        /// Get all tickets that are not assigned to an admin
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<TicketModel> GetAllUnassignedTickets()
        {
            //Define a base variable for the query and a list to hold the results
            string query = "";
            List<TicketModel> tickets = new List<TicketModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define query for all tickets that are not assigned to an admin
                query = "SELECT * FROM Tickets WHERE AdminId IS NULL;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {


                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TicketModel ticket = new TicketModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Info = reader.GetString(reader.GetOrdinal("Info")),
                                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                                CreationId = reader.GetInt32(reader.GetOrdinal("CreationId")),
                                AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId")),
                                LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                                Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")))
                            };
                            tickets.Add(ticket);
                        }
                    }
                    return tickets;

                }

            }
        }

        /// <summary>
        /// Get a ticket by its ID  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TicketModel GetTicketById(int id)
        {
            //Define a base variable for the query and a ticket
            string query = "";
            TicketModel ticket = new TicketModel();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define a query to select a ticket by its ID
                query = "SELECT * FROM Tickets WHERE TicketId = @TicketId;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@TicketId", id);

                    //Using a SqlDataReader to read the results and populate the tickets list
                    //Execute Reader also sends query to database and return with query results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            ticket.Id = reader.GetInt32(reader.GetOrdinal("TicketId"));
                            ticket.CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                            ticket.Subject = reader.GetString(reader.GetOrdinal("Subject"));
                            ticket.Info = reader.GetString(reader.GetOrdinal("Info"));
                            ticket.Priority = reader.GetInt32(reader.GetOrdinal("Priority"));
                            ticket.CreationId = reader.GetInt32(reader.GetOrdinal("CreationId"));
                            ticket.AdminId = reader.IsDBNull(reader.GetOrdinal("AdminId")) ? null : reader.GetInt32(reader.GetOrdinal("AdminId"));
                            ticket.LastUpdate = reader.IsDBNull(reader.GetOrdinal("LastUpdate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastUpdate"));
                            ticket.Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Comments")));

                        }
                    }
                    return ticket;

                }

            }
        }

        /// <summary>
        /// Update a ticket in the database
        /// </summary>
        /// <param name="ticket"></param>
        public int UpdateTicket(TicketModel ticket)
        {
            //define base variables for the query
            string query = "";
            int result = -1;

            using (SqlConnection connection = new SqlConnection(conn))
            {
                //open the connection to database
                connection.Open();

                //define the query to update a ticket in the database by matching TicketID
                query = "UPDATE Tickets SET CreationDate = @CreationDate, Subject = @Subject, Info = @Info, " +
                        "Priority = @Priority, CreationId = @CreationId, AdminId = @AdminId, LastUpdate = @LastUpdate, Comments = @Comments WHERE TicketId = @TicketId;";

                //use sqlcommand to use command.paramerts.addwithvalue to input desired values
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //Add parameters to the command
                    command.Parameters.AddWithValue("@CreationDate", ticket.CreationDate);
                    command.Parameters.AddWithValue("@Subject", ticket.Subject);
                    command.Parameters.AddWithValue("@Info", ticket.Info);
                    command.Parameters.AddWithValue("@Priority", ticket.Priority);
                    command.Parameters.AddWithValue("@CreationId", ticket.CreationId);
                    command.Parameters.AddWithValue("@TicketId", ticket.Id);
                    command.Parameters.AddWithValue("@AdminId", (object)ticket.AdminId ?? DBNull.Value); // Handle nullable AdminId
                    //command.Parameters.AddWithValue("@LastUpdate", ticket.LastUpdate == DateTime.MinValue ? DBNull.Value: (object)ticket.LastUpdate); // Set LastUpdate to current time
                    command.Parameters.Add("@LastUpdate", SqlDbType.DateTime).Value = ticket.LastUpdate.HasValue ? (object)ticket.LastUpdate.Value : DBNull.Value;

                    // Handle Comments as a JSON string or similar format if needed
                    // Autocomplete assisted with this likely using a newer or updated Newtonsoft.Json version, I personally have not used this package before ZChermak
                    var json = ticket.Comments != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ticket.Comments) : null;

                    //Add Comments parameter, handling null values
                    command.Parameters.AddWithValue("@Comments", (object)json ?? DBNull.Value);

                    // Execute the query to update the ticket
                    result = command.ExecuteNonQuery();
                    return result;
                }

            }
        }
    }
}
