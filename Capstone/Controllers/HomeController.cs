using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Capstone.Filters.SessionCheckFilters;

namespace Capstone.Controllers
{
    public class HomeController : Controller
    {
        

        public List <UserRegisterModel> Users = new List<UserRegisterModel>();

        [SessionCheckFilter]
        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        public IActionResult RegisterUser(UserRegisterModel user)
        {
            Users.Add(user);
            return View("AccountInfo", Users[0]);
        }
    }
}
