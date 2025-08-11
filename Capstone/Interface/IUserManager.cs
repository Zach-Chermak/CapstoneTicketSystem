using Capstone.Models;

namespace Capstone.Interface
{
    
    //Defines data exchange possibilites for Users


    public interface IUserManager
    {
        public List<UserModel> GetAllUsers(); //return all users stored in the system
        public UserModel GetUserById(int id); //given id number, find the matching user
        public int AddUser(UserModel user); //add a new user to the list 
        public void DeleteUser(UserModel user); //remove the user who matches
        public void UpdateUser(UserModel user); //find the user with matching id and replace
        public UserModel CheckCredentials(string username, string password); //verify logins
    }
}
