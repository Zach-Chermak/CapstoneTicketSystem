using Capstone.Models;
using Capstone.DataAccess;
using Capstone.Interface;

namespace Capstone.Services.Business
{
    public class TicketColllection : ITicketManager
    {
        //in memory list of tickets
        private List<TicketModel>? _tickets;
        //data access object for ticket operations
        TicketDAO data = new TicketDAO();

        /// <summary>
        /// Instantiate ticket list and pull data from database 
        /// </summary>
        public TicketColllection()
        {
            
            _tickets = new List<TicketModel>();
            _tickets = data.GetAllTickets();
        }

        /// <summary>
        /// Add a new ticket to database
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public int AddTicket(TicketModel ticket)
        {
            _tickets.Add(ticket);
            int newTicketId = data.AddTicket(ticket);

            return newTicketId;
        }

        /// <summary>
        /// Delete a ticket from the database
        /// </summary>
        /// <param name="ticket"></param>
        public int DeleteTicket(TicketModel ticket)
        {
            return data.DeleteTicket(ticket);
        }

        /// <summary>
        /// Get all tickets assigned to an admin
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public List<TicketModel> GetAllAdminTickets(int adminId)
        {
            return data.GetAllAdminTickets(adminId);
        }

        /// <summary>
        /// Get all tickets from database
        /// </summary>
        /// <returns></returns>
        public List<TicketModel> GetAllTickets()
        {
            return data.GetAllTickets();
        }

        /// <summary>
        /// Get all tickets created by a specific user based on their creationId
        /// </summary>
        /// <param name="creationId"></param>
        /// <returns></returns>
        public List<TicketModel> GetAllTicketsByCreationId(int creationId)
        {
            return data.GetAllTicketsByCreationId(creationId);
        }

        /// <summary>
        /// Get all tickets with the same   priority level
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public List<TicketModel> GetAllTicketsByPriority(int priority)
        {
            return data.GetAllTicketsByPriority(priority);
        }

        /// <summary>
        /// Get all tickets that are not assigned to an admin
        /// </summary>
        /// <returns></returns>
        public List<TicketModel> GetAllUnassignedTickets()
        {
            return data.GetAllUnassignedTickets();
        }

        /// <summary>
        /// Get a ticket by its id number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TicketModel GetTicketById(int id)
        {
            return data.GetTicketById(id);
        }

        /// <summary>
        /// Update a ticket in the database
        /// </summary>
        /// <param name="ticket"></param>
        public int UpdateTicket(TicketModel ticket)
        {
            return data.UpdateTicket(ticket);
        }
    }
}
