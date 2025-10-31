using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IServices
{

    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId, bool? isCompleted = null);
        Task<TaskItem> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskItem> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<TaskItem> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
        Task<IEnumerable<TaskItem>> SearchTasksAsync(int userId, string searchTerm);
    }
}
