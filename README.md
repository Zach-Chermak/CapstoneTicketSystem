# CapstoneTicketSystem
This is my GCU Senior Capstone project. I made a asp.net web application that allows you to submit tickets. Allowing admins to assign their id number to them and timestamp their interactions.

# Project Portfolio
## Database Diagrams
- User Account Database <br> <img width="369" height="218" alt="image" src="https://github.com/user-attachments/assets/e77c3dfa-be44-428e-a6e7-142d8fc8b744" /> 
- Ticket Database <br> <img width="375" height="238" alt="image" src="https://github.com/user-attachments/assets/155f0078-8cc2-45f8-9858-08b6917d06c1" /> <br>

## User Features
- Create a ticket
- Edit your tickets
- Delete your tickets
- View all your current tickets on a dashboard
<br> <img width="583" height="387" alt="image" src="https://github.com/user-attachments/assets/ac4f964e-956d-488d-bc88-bff1b3a2b72b" />


## Admin Features
- Create a ticket
- Edit any ticket
- Delete any ticket
- Assign an AdminId to any ticket
- Add comments to tickets
- All actions are timestamped when updated
- View your assigned tickets
- View all unassigned tickets
- View tickets you created
<br> <img width="582" height="826" alt="image" src="https://github.com/user-attachments/assets/e38b3c03-5e1d-4f8a-8364-e066be2c798a" />


## Login/Account Features
- Register new account
- Login to existing account
- Logout of account
- Homepage and nav bar update to you login state
<br> <img width="198" height="40" alt="image" src="https://github.com/user-attachments/assets/3518680b-8ccd-4e57-b6a2-a5d0f6f95b95" />
<br> <img width="708" height="314" alt="image" src="https://github.com/user-attachments/assets/f7dc49c2-2501-44df-b9d2-55cebc0efc1a" />



## Session Services
- Session Services handled key pairs to hold a login state accross all pages in the application
- Clearing the session data acts as the logout feature
- Able to pull session data instead of data from the DB to fill views the user account info
```C#
  namespace Capstone.Services.Session
{
    public interface ISessionService
    {
        int? GetUserId();
        void SetUserId(int userId);
        string? GetString(string key);
        void SetString(string key, string value);
        void Remove(string key);
        bool? IsAdmin();
        void ClearSession();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId() => _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

        public void SetUserId(int userId) =>
            _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId);

        public string? GetString(string key) =>
            _httpContextAccessor.HttpContext?.Session.GetString(key);

        public void SetString(string key, string value) =>
            _httpContextAccessor.HttpContext?.Session.SetString(key, value);

        public void Remove(string key) =>
            _httpContextAccessor.HttpContext?.Session.Remove(key);

        public bool? IsAdmin() =>
            _httpContextAccessor.HttpContext?.Session.GetString("IsAdmin") == "true" ? true : false;

        public void ClearSession()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }
    }
}
```
## Controllers - Ticket Controller 
- Handles Dashboard for each user and determines if the user has admin priviledges. <br> 
```C#
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
```
- UpdateTicket method has the timestamping feature<br>
```C#
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
```


