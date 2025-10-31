using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagement.Core.Services
{
    /// <summary>
    /// Service layer for task management business logic
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId, bool? isCompleted = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            return await _taskRepository.GetUserTasksAsync(userId, isCompleted);
        }

        public async Task<TaskItem> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            // Authorization check - users can only access their own tasks
            if (task == null || task.UserId != userId)
                return null;

            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Priority = createTaskDto.Priority,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                IsCompleted = false,
                IsDeleted = false
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveAsync();

            return task;
        }

        /// <summary>
        /// Updates existing task with partial update support
        /// Implements RESTful PUT semantics
        /// </summary>
        public async Task<TaskItem> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId)
        {
            var task = await GetTaskByIdAsync(taskId, userId);
            if (task == null)
                return null;

            // Apply partial updates - only modify provided properties
            if (!string.IsNullOrEmpty(updateTaskDto.Title))
                task.Title = updateTaskDto.Title;

            if (!string.IsNullOrEmpty(updateTaskDto.Description))
                task.Description = updateTaskDto.Description;

            if (updateTaskDto.IsCompleted.HasValue)
                task.IsCompleted = updateTaskDto.IsCompleted.Value;

            if (!string.IsNullOrEmpty(updateTaskDto.Priority))
                task.Priority = updateTaskDto.Priority;

            task.ModifiedDate = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveAsync();

            return task;
        }

        /// <summary>
        /// Performs logical delete (soft delete) on task
        /// </summary>
        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await GetTaskByIdAsync(taskId, userId);
            if (task == null)
                return false;

            // Soft delete implementation
            task.IsDeleted = true;
            task.ModifiedDate = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetUserTasksAsync(userId);

            return await _taskRepository.SearchTasksAsync(userId, searchTerm);
        }
    }
}