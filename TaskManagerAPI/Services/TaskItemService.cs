using AutoMapper;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheHelper _cacheHelper;
        public TaskItemService(IUnitOfWork unitOfWork, IMapper mapper, ICacheHelper cacheHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheHelper = cacheHelper;
        }

        public async Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto createTaskDto)
        {
            var taskItem = _mapper.Map<TaskItem>(createTaskDto);
            await _unitOfWork.TaskItems.AddAsync(taskItem);
            await _unitOfWork.CompleteAsync();

            await _cacheHelper.RemoveCacheAsync("TaskItems_");

            return _mapper.Map<TaskItemDto>(taskItem);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var taskItem = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (taskItem == null)
            {
                return false;
            }

            _unitOfWork.TaskItems.Remove(taskItem);
            await _unitOfWork.CompleteAsync();

            await _cacheHelper.RemoveCacheAsync($"taskItem_{id}");
            await _cacheHelper.RemoveCacheAsync("TaskItems_True");
            await _cacheHelper.RemoveCacheAsync("TaskItems_False");
            await _cacheHelper.RemoveCacheAsync("TaskItems_");

            return true;
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllTaskItemsAsync(bool? completed = null)
        {
            string cacheKey = $"TaskItems_{completed}";

            // Try to get TaskItems from the cache
            var cachedTaskItems = await _cacheHelper.GetCacheAsync<IEnumerable<TaskItemDto>>(cacheKey);
            if (cachedTaskItems != null)
            {
                return cachedTaskItems;
            }

            // Fetch taskItems from the database
            var taskItems = await _unitOfWork.TaskItems.GetAllTaskItemsAsync(completed);
            var taskItemDtos = _mapper.Map<IEnumerable<TaskItemDto>>(taskItems);

            await _cacheHelper.SetCacheAsync(cacheKey, taskItemDtos, TimeSpan.FromMinutes(10));

            return taskItemDtos;
        }

        public async Task<TaskItemDto> GetTaskByIdAsync(int id)
        {
            string cacheKey = $"taskItem_{id}";

            var cachedTaskItem = await _cacheHelper.GetCacheAsync<TaskItemDto>(cacheKey);
            if (cachedTaskItem != null)
            {
                return cachedTaskItem;
            }

            var taskItem = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (taskItem == null)
            {
                return null;
            }

            var taskItemDto = _mapper.Map<TaskItemDto>(taskItem);

            await _cacheHelper.SetCacheAsync(cacheKey, taskItemDto, TimeSpan.FromMinutes(10));

            return taskItemDto;
        }

        public async Task<bool> UpdateTaskAsync(int id, CreateTaskItemDto updateTaskDto)
        {
            var existingTaskItem = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (existingTaskItem == null)
            {
                return false;
            }

            _mapper.Map(updateTaskDto, existingTaskItem);
            _unitOfWork.TaskItems.Update(existingTaskItem);
            await _unitOfWork.CompleteAsync();

            await _cacheHelper.RemoveCacheAsync($"taskItem_{id}");
            await _cacheHelper.RemoveCacheAsync("TaskItems_True");
            await _cacheHelper.RemoveCacheAsync("TaskItems_False");
            await _cacheHelper.RemoveCacheAsync("TaskItems_");

            return true;
        }

    }
}
