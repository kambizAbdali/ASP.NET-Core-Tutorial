using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.DTOs
{/// <summary>
 /// Data Transfer Object for creating new tasks
 /// </summary>
    public class CreateTaskDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [RegularExpression("Low|Medium|High")]
        public string Priority { get; set; } = "Medium";
    }

    /// <summary>
    /// Data Transfer Object for updating existing tasks
    /// </summary>
    public class UpdateTaskDto
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool? IsCompleted { get; set; }

        [RegularExpression("Low|Medium|High")]
        public string Priority { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for task responses with HATEOAS links
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Priority { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// HATEOAS links for RESTful API compliance
        /// </summary>
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

}