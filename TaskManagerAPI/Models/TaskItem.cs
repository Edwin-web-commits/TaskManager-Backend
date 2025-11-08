using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 70, ErrorMessage = "Task Title is too long")]
        public string Title { get; set; } = string.Empty;

        [StringLength(maximumLength: 250, ErrorMessage = "Task Description is too long")]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
