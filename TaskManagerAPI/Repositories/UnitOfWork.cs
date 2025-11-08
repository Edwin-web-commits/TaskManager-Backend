using TaskManagerAPI.Data;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ITaskItemRepository TaskItems { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            TaskItems = new TaskItemRepository(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
