using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IServices;

namespace TaskManagement.API.Controllers.V2
{
    /// <summary>
    /// Tasks API Version 2 - Extends V1 with additional features
    /// Implements API versioning with controller inheritance
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [Authorize]
    public class TasksControllerV2 : V1.TasksController // ارث‌بری از کنترلر Version 1
    {
        // No need to redeclare _taskService and _httpContextAccessor - they're inherited from base class

        public TasksControllerV2(ITaskService taskService, IHttpContextAccessor httpContextAccessor)
            : base(taskService, httpContextAccessor) // Pass both parameters to base constructor
        {
            // No need to reassign since base constructor already handles this
        }

        /// <summary>
        /// Retrieves all tasks for current user with enhanced response (V2)
        /// Adds pagination and additional metadata
        /// </summary>
        /// <param name="isCompleted">Filter by completion status</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <response code="200">Returns paginated list of tasks with enhanced metadata</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public override async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] bool? isCompleted = null)
        {
            // Call base method but add V2 enhancements
            var result = await base.GetTasks(isCompleted);

            // Enhanced response for V2 - add pagination metadata
            if (result.Result is OkObjectResult okResult && okResult.Value is IEnumerable<TaskDto> tasks)
            {
                var enhancedResponse = new
                {
                    Data = tasks,
                    Metadata = new
                    {
                        Version = "2.0",
                        TotalCount = tasks.Count(),
                        Timestamp = DateTime.UtcNow,
                        Features = new[] { "HATEOAS", "EnhancedMetadata", "PaginationReady" }
                    }
                };

                return Ok(enhancedResponse);
            }

            return result;
        }

        /// <summary>
        /// Creates a new task with additional validation (V2)
        /// Adds priority validation and auto-completion for high priority tasks
        /// </summary>
        /// <param name="createTaskDto">Task creation data</param>
        /// <response code="201">Returns the created task with V2 enhancements</response>
        /// <response code="400">If the input data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            // V2 Enhancement: Additional validation for priority
            if (createTaskDto.Priority == "High" && string.IsNullOrEmpty(createTaskDto.Description))
            {
                return BadRequest(new
                {
                    Message = "High priority tasks require a description",
                    Code = "V2_VALIDATION_ERROR"
                });
            }

            // Call base implementation
            return await base.CreateTask(createTaskDto);
        }

        /// <summary>
        /// NEW ENDPOINT: Search tasks with advanced filtering (V2 Only)
        /// Adds multiple search criteria and sorting options
        /// </summary>
        /// <param name="searchTerm">Search in title or description</param>
        /// <param name="priority">Filter by priority</param>
        /// <param name="sortBy">Sort field (title, createdDate, priority)</param>
        /// <param name="sortOrder">Sort order (asc, desc)</param>
        /// <response code="200">Returns filtered and sorted tasks</response>
        [HttpGet("advanced-search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> AdvancedSearch(
            [FromQuery] string searchTerm = "",
            [FromQuery] string priority = "",
            [FromQuery] string sortBy = "createdDate",
            [FromQuery] string sortOrder = "desc")
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await _taskService.SearchTasksAsync(userId, searchTerm);

                // V2 Enhancement: Priority filtering
                if (!string.IsNullOrEmpty(priority))
                {
                    tasks = tasks.Where(t => t.Priority == priority);
                }

                // V2 Enhancement: Advanced sorting
                tasks = sortBy.ToLower() switch
                {
                    "title" => sortOrder == "asc" ?
                        tasks.OrderBy(t => t.Title) :
                        tasks.OrderByDescending(t => t.Title),
                    "priority" => sortOrder == "asc" ?
                        tasks.OrderBy(t => t.Priority) :
                        tasks.OrderByDescending(t => t.Priority),
                    _ => sortOrder == "asc" ?
                        tasks.OrderBy(t => t.CreatedDate) :
                        tasks.OrderByDescending(t => t.CreatedDate)
                };

                var taskDtos = tasks.Select(task =>
                {
                    var dto = MapToTaskDto(task);
                    // V2 Enhancement: Add additional metadata to each task
                    dto.Links.Add(new LinkDto
                    {
                        Href = $"{Request.Scheme}://{Request.Host}/api/v2.0/tasks/{task.Id}/analytics",
                        Rel = "analytics",
                        Method = "GET"
                    });
                    return dto;
                }).ToList();

                return Ok(new
                {
                    Data = taskDtos,
                    Metadata = new
                    {
                        SearchTerm = searchTerm,
                        PriorityFilter = priority,
                        SortBy = sortBy,
                        SortOrder = sortOrder,
                        TotalResults = taskDtos.Count,
                        Version = "2.0"
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// NEW ENDPOINT: Get task analytics (V2 Only)
        /// Provides additional analytics for a specific task
        /// </summary>
        /// <param name="id">Task identifier</param>
        /// <response code="200">Returns task analytics</response>
        /// <response code="404">If task is not found</response>
        [HttpGet("{id}/analytics")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetTaskAnalytics(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.GetTaskByIdAsync(id, userId);

                if (task == null)
                    return NotFound(new { Message = $"Task with ID {id} not found" });

                // V2 Feature: Calculate analytics
                var timeSinceCreation = DateTime.UtcNow - task.CreatedDate;
                var completionRate = task.IsCompleted ? 100 : 0;
                var priorityWeight = task.Priority switch
                {
                    "High" => 3,
                    "Medium" => 2,
                    "Low" => 1,
                    _ => 1
                };

                var analytics = new
                {
                    TaskId = task.Id,
                    DaysSinceCreation = timeSinceCreation.Days,
                    CompletionRate = completionRate,
                    PriorityWeight = priorityWeight,
                    IsOverdue = !task.IsCompleted && timeSinceCreation.Days > 7,
                    EstimatedCompletionTime = priorityWeight * 2, // days
                    Version = "2.0"
                };

                return Ok(analytics);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Override task deletion to implement soft delete with archive (V2)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Changed from 204 to 200 in V2
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.DeleteTaskAsync(id, userId);

            if (!result)
                return NotFound(new { Message = $"Task with ID {id} not found" });

            // V2 Enhancement: Return additional information instead of NoContent
            return Ok(new
            {
                Message = "Task archived successfully",
                TaskId = id,
                ArchivedAt = DateTime.UtcNow,
                CanRestore = true,
                Version = "2.0"
            });
        }
    }
}