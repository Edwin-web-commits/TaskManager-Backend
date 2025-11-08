using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;
        private readonly ILogger<TaskItemsController> _logger;
        public TaskItemsController(ITaskItemService taskItemService, ILogger<TaskItemsController> logger)
        {
            _taskItemService = taskItemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskItems([FromQuery] bool? completed)
        {
            _logger.LogTrace("Get taskItems");

            var taskItems = await _taskItemService.GetAllTaskItemsAsync(completed);

            return Ok(taskItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskItem(int id)
        {
            _logger.LogInformation("Fetching taskItem with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID {Id} provided for fetching taskItem", id);
                return BadRequest("Invalid ID");
            }
            var taskItem = await _taskItemService.GetTaskByIdAsync(id);
            if (taskItem == null)
            {
                _logger.LogWarning("TaskItem with ID {Id} not found", id);
                return NotFound();
            }
            return Ok(taskItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskItemDto createTaskDto)
        {
            _logger.LogTrace("Creating new taskItem with data: {@CreateTaskItemDto}", createTaskDto);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating taskItem");
                return BadRequest(ModelState);
            }

            var createdTaskItem = await _taskItemService.CreateTaskAsync(createTaskDto);
            _logger.LogInformation("Created new taskItem with ID: {Id}", createdTaskItem.Id);
            return CreatedAtAction(nameof(GetTaskItem), new { id = createdTaskItem.Id }, createdTaskItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(int id, [FromBody] CreateTaskItemDto updateTaskItemDto)
        {
            _logger.LogTrace("Updating taskItem with ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid request for updating taskItem with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            var success = await _taskItemService.UpdateTaskAsync(id, updateTaskItemDto);
            if (!success)
            {
                _logger.LogWarning("TaskItem with ID {Id} not found for update", id);
                return NotFound();
            }

            _logger.LogInformation("Updated taskItem with ID: {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            _logger.LogTrace("Deleting taskItem with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID {Id} for deletion", id);
                return BadRequest("Invalid ID");
            }

            var success = await _taskItemService.DeleteTaskAsync(id);
            if (!success)
            {
                _logger.LogWarning("TaskItem with ID {Id} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted taskItem with ID: {Id}", id);
            return NoContent();
        }
    }
}
