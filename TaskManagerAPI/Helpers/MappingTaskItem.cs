using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Helpers
{
    public class MappingTaskItem : AutoMapper.Profile
    {
        public MappingTaskItem()
        {
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
            CreateMap<TaskItem, CreateTaskItemDto>().ReverseMap();
        }
    }
}
