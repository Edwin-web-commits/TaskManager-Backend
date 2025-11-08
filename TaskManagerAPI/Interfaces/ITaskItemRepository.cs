using TaskManagerAPI.Models;

namespace TaskManagerAPI.Interfaces
{
    public interface ITaskItemRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetAllTaskItemsAsync(bool? completed = null);
    }
}
