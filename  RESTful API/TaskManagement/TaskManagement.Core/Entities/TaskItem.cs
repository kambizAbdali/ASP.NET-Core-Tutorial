using System;
using System.Collections.Generic;

namespace TaskManagement.Core.Entities
{
    /// <summary>
    /// Represents a task entity in the task management system
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Priority level of the task (Low, Medium, High)
        /// Used for sorting and filtering tasks
        /// </summary>
        public string Priority { get; set; } = "Medium";
        public int UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// Soft delete flag for logical deletion
        /// When true, task is considered deleted but remains in database
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}