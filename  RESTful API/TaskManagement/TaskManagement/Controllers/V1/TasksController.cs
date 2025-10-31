using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Core.Services;

namespace TaskManagement.API.Controllers.V1
{
    /// <summary>
    /// Tasks API controller for task management operations
    /// Implements RESTful principles with HATEOAS support
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TasksController : ControllerBase
    {
        protected readonly ITaskService _taskService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public TasksController(ITaskService taskService, IHttpContextAccessor httpContextAccessor)
        {
            _taskService = taskService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected int GetCurrentUserId()
        {
            // Check if user is authenticated
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            // Validate that the claim exists and can be parsed
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                throw new UnauthorizedAccessException("User ID claim not found in token");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID format in token");
            }

            return userId;
        }

        /// <summary>
        /// Retrieves all tasks for the current user with optional filtering
        /// </summary>
        /// <param name="isCompleted">Filter by completion status (true/false)</param>
        /// <response code="200">Returns list of tasks with HATEOAS links</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] bool? isCompleted = null)
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetUserTasksAsync(userId, isCompleted);

            var taskDtos = tasks.Select(task => MapToTaskDto(task)).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// Retrieves a specific task by ID
        /// </summary>
        /// <param name="id">Task identifier</param>
        /// <response code="200">Returns the requested task</response>
        /// <response code="404">If task is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
                return NotFound(new { Message = $"Task with ID {id} not found" });

            return Ok(MapToTaskDto(task));
        }

        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="createTaskDto">Task creation data</param>
        /// <response code="201">Returns the created task with location header</response>
        /// <response code="400">If the input data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.CreateTaskAsync(createTaskDto, userId);

            var taskDto = MapToTaskDto(task);

            // Generate URL for the created resource (RESTful practice)
            var url = Url.Action(nameof(GetTask), "Tasks", new { id = task.Id, version = "1.0" });

            // Return 201 Created with location header
            return Created(url, taskDto);
        }

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="id">Task identifier</param>
        /// <param name="updateTaskDto">Task update data</param>
        /// <response code="200">Returns the updated task</response>
        /// <response code="404">If task is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (task == null)
                return NotFound(new { Message = $"Task with ID {id} not found" });

            return Ok(MapToTaskDto(task));
        }

        /// <summary>
        /// Deletes a task (soft delete)
        /// </summary>
        /// <param name="id">Task identifier</param>
        /// <response code="204">Task successfully deleted</response>
        /// <response code="404">If task is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.DeleteTaskAsync(id, userId);

            if (!result)
                return NotFound(new { Message = $"Task with ID {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Searches tasks by title or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <response code="200">Returns matching tasks</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> SearchTasks([FromQuery] string searchTerm)
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.SearchTasksAsync(userId, searchTerm);

            var taskDtos = tasks.Select(task => MapToTaskDto(task)).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// Maps Task entity to TaskDto with HATEOAS links
        /// Implements Richardson Maturity Model Level 3
        /// </summary>
        protected TaskDto MapToTaskDto(TaskItem task)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedDate = task.CreatedDate,
                ModifiedDate = task.ModifiedDate,
                Priority = task.Priority,
                UserId = task.UserId
            };

            // Add HATEOAS links
            taskDto.Links.Add(new LinkDto
            {
                Href = $"{baseUrl}/api/v1.0/tasks/{task.Id}",
                Rel = "self",
                Method = "GET"
            });

            taskDto.Links.Add(new LinkDto
            {
                Href = $"{baseUrl}/api/v1.0/tasks/{task.Id}",
                Rel = "update",
                Method = "PUT"
            });

            taskDto.Links.Add(new LinkDto
            {
                Href = $"{baseUrl}/api/v1.0/tasks/{task.Id}",
                Rel = "delete",
                Method = "DELETE"
            });

            return taskDto;
        }
    }
}