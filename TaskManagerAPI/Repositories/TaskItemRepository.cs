using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public class TaskItemRepository : GenericRepository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetAllTaskItemsAsync(bool? completed = null)
        {
            var query = _context.Tasks.AsQueryable();

            if (completed.HasValue)
            {
                query = query.Where(p => p.IsCompleted == completed.Value);
            }
            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
    }
}
