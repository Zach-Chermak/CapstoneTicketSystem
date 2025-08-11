using System.ComponentModel.DataAnnotations;
using Capstone.Models;

namespace Capstone.Models
{
    public class UserRegisterModel
    {
        //Properties
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //EXTRA EXTRA EXTRA
        //Primary Key
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; } = false;

        public UserRegisterModel()
        {
            Id = 0;
            UserName = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            PhoneNumber = string.Empty;
        }
    }


}
