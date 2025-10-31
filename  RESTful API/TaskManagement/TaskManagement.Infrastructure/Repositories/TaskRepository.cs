using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{

    /// <summary>
    /// Task repository implementation with task-specific queries
    /// </summary>
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(TaskManagementContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId, bool? isCompleted = null)
        {
            var query = _dbSet.Where(t => t.UserId == userId && !t.IsDeleted);

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            return await query.OrderByDescending(t => t.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetUserTasksAsync(userId);

            var lowerSearchTerm = searchTerm.ToLower();
            return await _dbSet
                .Where(t => t.UserId == userId &&
                           !t.IsDeleted &&
                           (t.Title.ToLower().Contains(lowerSearchTerm) ||
                            t.Description.ToLower().Contains(lowerSearchTerm)))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(int userId, string priority)
        {
            return await _dbSet
                .Where(t => t.UserId == userId &&
                           !t.IsDeleted &&
                           t.Priority == priority)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
