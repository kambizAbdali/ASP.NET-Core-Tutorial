using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    /// <summary>
    /// Task-specific repository extending generic repository
    /// </summary>
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId, bool? isCompleted = null);
        Task<IEnumerable<TaskItem>> SearchTasksAsync(int userId, string searchTerm);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(int userId, string priority);
    }
}