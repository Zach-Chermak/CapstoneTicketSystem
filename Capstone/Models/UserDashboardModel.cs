namespace Capstone.Models
{
    public class UserDashboardModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public List<TicketModel>? Tickets { get; set; }

        public List<TicketModel>? AdminTickets { get; set; }

        public List<TicketModel>? UnassignedTickets { get; set; }
    }
}
