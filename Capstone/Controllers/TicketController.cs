using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Capstone.Services.Session;
using static Capstone.Filters.SessionCheckFilters;
using Capstone.Services.Business;

namespace Capstone.Controllers
{
    public class TicketController : Controller
    {
        private readonly ISessionService _sessionService;

        private static TicketColllection _ticketCollection = new TicketColllection();



        // Constructor that accepts ISessionService for dependency injection
        public TicketController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }



        /// <summary>
        /// Index action that retrieves all tickets for the logged-in user
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.Message = null;

            int userId = _sessionService.GetUserId() ?? 0;
            List<TicketModel> tickets = _ticketCollection.GetAllTicketsByCreationId(userId);
            string userFirstName = _sessionService.GetString("UserFirstName") ?? "Guest";

            UserDashboardModel userDashboard = new UserDashboardModel
            {
                Id = userId,
                FirstName = userFirstName,
                Tickets = tickets
            };

            //CHECK FOR ADMIN or REGULAR USER

            if (_sessionService.IsAdmin() == true)
            {
                userDashboard.AdminTickets = _ticketCollection.GetAllAdminTickets(userId);
                userDashboard.UnassignedTickets = _ticketCollection.GetAllUnassignedTickets();
                return View("AdminIndex", userDashboard);
            }
            else
            {
                return View(userDashboard);
            }

        }

        /// <summary>
        /// Process the ticket submission from the user, INITIAL CREATION OF TICKET
        /// </summary>
        /// <param name="newTicket"></param>
        /// <returns></returns>
        public IActionResult ProcessTicket(TicketModel newTicket)
        {
            TicketModel ticket = new TicketModel
            {
                CreationDate = DateTime.Now,
                Subject = newTicket.Subject,
                Info = newTicket.Info,
                Priority = newTicket.Priority,
                CreationId = _sessionService.GetUserId() ?? 0
            };

            int ticketCount = _ticketCollection.AddTicket(ticket);
            return View("TicketSubmitted");
        }

        /// <summary>
        /// Display the view for creating a new ticket
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateTicket()
        {
            return View();
        }


        /// <summary>
        /// Ticket submitted view after a successful ticket creation
        /// </summary>
        /// <returns></returns>
        public IActionResult TicketSubmitted()
        {
            return View();
        }

        /// <summary>
        /// Details action to view a specific ticket by its ID, different for admin and regular users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            TicketModel? ticket = _ticketCollection.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View("Details", ticket);
        }

        /// <summary>
        /// Assign an admin to a specific ticket by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public IActionResult AssignAdmin(int id, int adminId)
        {
            TicketModel? ticket = _ticketCollection.GetTicketById(id);
            ticket.AdminId = adminId;
            _ticketCollection.UpdateTicket(ticket);
            return RedirectToAction("Index", "Ticket");
        }

        /// <summary>
        /// Edit view for a specific ticket by its ID-----ADMIN
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditTicketAdmin(int id)
        {
            TicketModel? ticket = _ticketCollection.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View("EditTicketAdmin", ticket);

        }

        /// <summary>
        /// Edit ticket for normal user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult EditTicketUser(int id)
        {
            TicketModel? ticket = _ticketCollection.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View("EditTicketUser", ticket);
        }

        /// <summary>
        /// Update a ticket with new info in the database, also datetime if the user is an admin
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateTicket(TicketModel ticket, string? NewComment)
        {
            if (_sessionService.IsAdmin() == true)
            {
                ticket.LastUpdate = DateTime.Now;
                if (NewComment != null)
                {
                    ticket.AddComment(NewComment);
                }
                
            }

            int result = _ticketCollection.UpdateTicket(ticket);
            if (result != -1)
            {
                TempData["Message"] = "Ticket updated successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to update ticket.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete a ticket in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteTicket(int id)
        {
            TicketModel? ticket = _ticketCollection.GetTicketById(id);
            int result = _ticketCollection.DeleteTicket(ticket);
            if (result != -1)
            {
                TempData["Message"] = "Ticket deleted successfully.";
            }
            else
            {
                TempData["Message"] = "Failed to delete ticket.";
            }

            return RedirectToAction("Index");
        }
    }
}
