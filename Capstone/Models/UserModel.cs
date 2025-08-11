using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Models
{
    public class UserModel
    {
        //Properties
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





        /// <summary>
        /// Verify the password is correct for the user
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool VerifyPassword(string pass)
        {
            if (Password == pass) return true;
            else return false;
        }
        /// <summary>
        /// Set a password for the user
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string pass)
        {
            Password = pass;
        }
    }
    
}

