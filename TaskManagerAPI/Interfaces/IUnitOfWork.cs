namespace TaskManagerAPI.Interfaces
{
    public interface IUnitOfWork
    {
        ITaskItemRepository TaskItems { get; }
        Task<int> CompleteAsync();
    }
}
