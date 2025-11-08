using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemDto>> GetAllTaskItemsAsync(bool? completed = null);
        Task<TaskItemDto> GetTaskByIdAsync(int id);
        Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto createTaskDto);
        Task<bool> UpdateTaskAsync(int id, CreateTaskItemDto updateTaskDto);
        Task<bool> DeleteTaskAsync(int id);
    }
}
