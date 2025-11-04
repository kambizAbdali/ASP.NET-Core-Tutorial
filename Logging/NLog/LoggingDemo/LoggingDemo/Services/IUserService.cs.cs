using LoggingDemo.Models;

namespace LoggingDemo.Services
{
    public interface IUserService
    {
        User CreateUser(User user);
        User GetUser(int id);
        List<User> GetAllUsers();
        void DeleteUser(int id);
    }
}