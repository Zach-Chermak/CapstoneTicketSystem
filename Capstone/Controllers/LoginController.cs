using Capstone.Models;
using Capstone.Services.Business;
using Capstone.Services.Session;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static Capstone.Filters.SessionCheckFilters;

namespace Capstone.Controllers
{
    public class LoginController : Controller
    {
        static UserCollection users = new UserCollection();
        

        private readonly ISessionService _sessionService;

        public LoginController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        //class level user model
        UserModel searchResults = new UserModel();


        public IActionResult Index()
        {

            return View();

        }
        public IActionResult ProcessLogin(string username, string password)
        {

            //declare and init
            int result = -1;
            string userJson = "";
            UserModel user = null;






            user = users.CheckCredentials(username, password);
            //if the result is greater than 0 (means successful login)
            if (user.Id > 0)
            {
                ////Create a new instance of 'UserModel' with properties 'Id','Username',and 'PasswordHash'
                ////This represents the user data provided during the login attempt
                UserModel userData = new() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, PhoneNumber = user.PhoneNumber, UserName = user.UserName, IsAdmin = user.IsAdmin, Id = user.Id };

                //serialize the userdata
                userJson = JsonSerializer.Serialize(userData);

                //store the userdata in the session, key "User"
                HttpContext.Session.SetString("User", userJson); //Used for the original filter with redirect
                //newer Session services for storing and pulling bulk information
                _sessionService.SetString("User", userJson);
                _sessionService.SetUserId(userData.Id);
                _sessionService.SetString("UserFirstName", userData.FirstName);
                _sessionService.SetString("IsAdmin", userData.IsAdmin == true ? "true":"false");


                TicketController ticketController = new TicketController(_sessionService);

                //return to the login success, pass the user data as the model
                return RedirectToAction("Index", "Ticket");
            }
            else
            {
                ViewBag.LoginError = "Invalid username or password. Please try again.";
                return View("Index");
                
            }


        }//end process login

        [SessionCheckFilter]
        public IActionResult MembersOnly()
        {
            return View();
        }

        [HttpPost]
        //[SessionCheckFilter]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            _sessionService.ClearSession();
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            //return the view for the register page
            return View();
        }

        public IActionResult ProcessRegister(UserModel registerViewModel)
        {
            UserModel user = new UserModel();
            user.UserName = registerViewModel.UserName;
            user.SetPassword(registerViewModel.Password);
            user.Email = registerViewModel.Email;
            user.FirstName = registerViewModel.FirstName;
            user.LastName = registerViewModel.LastName;
            user.PhoneNumber = registerViewModel.PhoneNumber;
            user.IsAdmin = registerViewModel.IsAdmin;

            users.AddUser(user);

            
            //Login to the new user after creation
            return ProcessLogin(user.UserName, user.Password);
        }


    }
}
