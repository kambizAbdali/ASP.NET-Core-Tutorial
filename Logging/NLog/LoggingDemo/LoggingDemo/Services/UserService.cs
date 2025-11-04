using LoggingDemo.Models;
using Microsoft.Extensions.Logging;

namespace LoggingDemo.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private static List<User> _users = new();
        private static int _nextId = 1;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;

            // Log service initialization
            _logger.LogInformation("UserService initialized successfully");
        }

        public User CreateUser(User user)
        {
            // Log method entry with parameters
            _logger.LogDebug("Creating new user with email: {UserEmail}", user.Email);

            try
            {
                user.Id = _nextId++;
                _users.Add(user);

                // Log successful creation with object details
                _logger.LogInformation("User created successfully: {@User}", user);

                return user;
            }
            catch (Exception ex)
            {
                // Log error with exception details and user data
                _logger.LogError(ex, "Failed to create user: {@User}", user);
                throw;
            }
        }

        public User GetUser(int id)
        {
            _logger.LogDebug("Retrieving user with ID: {UserId}", id);

            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                // Log warning for non-critical issue
                _logger.LogWarning("User with ID {UserId} not found", id);
                throw new ArgumentException($"User with ID {id} not found");
            }

            _logger.LogDebug("User retrieved: {@User}", user);
            return user;
        }

        public List<User> GetAllUsers()
        {
            _logger.LogDebug("Retrieving all users. Total count: {UserCount}", _users.Count);

            // Simulate performance issue for demonstration
            if (_users.Count > 100)
            {
                _logger.LogWarning("Large number of users detected: {UserCount}. Performance may be affected.", _users.Count);
            }

            return _users;
        }

        public void DeleteUser(int id)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Delete failed: User with ID {UserId} not found", id);
                throw new ArgumentException($"User with ID {id} not found");
            }

            _users.Remove(user);
            _logger.LogInformation("User deleted successfully: {@User}", user);
        }
    }
}