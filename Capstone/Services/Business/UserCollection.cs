using Capstone.DataAccess;
using Capstone.Interface;
using Capstone.Models;

namespace Capstone.Services.Business
{
    public class UserCollection : IUserManager
    {
        //this is an in memory list of variables, later would be a database.
        private List<UserModel>? _users;

        /// <summary>
        /// Constructor for the List of user model
        /// </summary>
        public UserCollection()
        {
            UserDAO DataAccess = new UserDAO();
            _users = new List<UserModel>();
            _users = DataAccess.GetAllUsers();
            //creat some user accounts
            //GenerateUserData();
        }

        /// <summary>
        /// Creates two users and adds them to the _user List
        /// </summary>
        private void GenerateUserData()
        {
            UserModel user1 = new UserModel();
            user1.UserName = "1";
            user1.SetPassword("2");
            user1.IsAdmin = true;
            AddUser(user1);

            UserModel user2 = new UserModel();
            user2.UserName = "x";
            user2.SetPassword("x");
            AddUser(user2);
        }
        /// <summary>
        /// Add a new user to the _user List and increment index count
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(UserModel user)
        {
            //set the user's id to the next available number
            user.Id = _users.Count + 1;
            _users.Add(user);
            UserDAO DataAccess = new UserDAO();
            DataAccess.AddUser(user);
            return user.Id;
        }
        /// <summary>
        /// Verify both the UserName and Password for a specific user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserModel CheckCredentials(string username, string password)
        {
            //given a username and password, find a matching user
            //instantiate the UserDAO
            UserDAO DataAccess = new UserDAO();

            //handles verification of password and invalid login attempts
            return (DataAccess.CheckCredentials(username, password));
        }
        /// <summary>
        /// Remove a user from the _user List
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(UserModel user)
        {

            UserDAO DataAccess = new UserDAO();
            DataAccess.DeleteUser(user);

        }
        /// <summary>
        /// Return the full _user List
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            UserDAO DataAccess = new UserDAO();
            return DataAccess.GetAllUsers();
        }

        /// <summary>
        /// given an id number find the user with the matching id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            UserDAO DataAccess = new UserDAO();
            return DataAccess.GetUserById(id);
        }
        /// <summary>
        /// Replace a new user into a specific index of another user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(UserModel user)
        {
            //find the user with the matching id and replace it
            _users[_users.FindIndex(u => u.Id == user.Id)] = user;
        }
    }
}
