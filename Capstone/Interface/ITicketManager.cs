using Capstone.Models;
namespace Capstone.Interface
{
    public interface ITicketManager
    {
        public List<TicketModel> GetAllTickets(); //return all tickets stored in the system
        public TicketModel GetTicketById(int id); //given id number, find the matching ticket
        public int AddTicket(TicketModel ticket); //add a new ticket to the list
        public int DeleteTicket(TicketModel ticket); //remove the ticket who matches
        public int UpdateTicket(TicketModel ticket); //find the ticket with matching id and replace
        public List<TicketModel> GetAllAdminTickets(int adminId); //return all tickets assigned to the admin
        public List<TicketModel> GetAllUnassignedTickets(); //return all tickets that are not assigned to an admin
        public List<TicketModel> GetAllTicketsByPriority(int priority); //return all tickets with a specific priority
        public List<TicketModel> GetAllTicketsByCreationId(int creationId); //return all tickets created by a specific user

    }
}
