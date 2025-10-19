using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityCompleteProject.Models;
using IdentityCompleteProject.ViewModels;
using IdentityCompleteProject.Data;

namespace IdentityCompleteProject.Services
{
    public interface IUserService
    {
        Task<PaginatedList<User>> GetUsersAsync(int page = 1, int pageSize = 10);
        Task<User> GetUserByIdAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(UserProfileViewModel model);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<PaginatedList<User>> GetUsersAsync(int page = 1, int pageSize = 10)
        {
            var query = _userManager.Users
                .Include(u => u.Addresses)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<User>(items, totalCount, page, pageSize);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email; // Keep username in sync with email
            user.PhoneNumber = model.PhoneNumber;
            user.NationalCode = model.NationalCode;
            user.BirthDate = model.BirthDate;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });
            }

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }
    }

    // Utility class for pagination
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}