using AutoMapper;
using Moq;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests
{
    public class TaskItemServiceTests
    {
        
            private readonly Mock<IUnitOfWork> _mockUnitOfWork;
            private readonly Mock<IMapper> _mockMapper;
            private readonly Mock<ICacheHelper> _mockCache;
            private readonly TaskItemService _service;
            private readonly Mock<ITaskItemRepository> _mockTaskItemRepo;

        public TaskItemServiceTests()
            {
                _mockUnitOfWork = new Mock<IUnitOfWork>();
                _mockMapper = new Mock<IMapper>();
                _mockCache = new Mock<ICacheHelper>();
                _mockTaskItemRepo = new Mock<ITaskItemRepository>();
            _mockUnitOfWork.Setup(u => u.TaskItems).Returns(_mockTaskItemRepo.Object);
            _service = new TaskItemService(_mockUnitOfWork.Object, _mockMapper.Object, _mockCache.Object);
            }

            [Fact]
            public async Task GetAllTaskItemsAsync_ReturnsCachedData_IfExists()
            {
                var cached = new List<TaskItemDto> { new TaskItemDto { Id = 1, Title = "Cached Task" } };
                _mockCache.Setup(c => c.GetCacheAsync<IEnumerable<TaskItemDto>>("TaskItems_"))
                          .ReturnsAsync(cached);

                var result = await _service.GetAllTaskItemsAsync(null);

                Assert.Single(result);
                _mockUnitOfWork.Verify(u => u.TaskItems.GetAllTaskItemsAsync(It.IsAny<bool?>()), Times.Never);
            }

            [Fact]
            public async Task GetAllTaskItemsAsync_FetchesFromRepo_IfCacheEmpty()
            {
                _mockCache.Setup(c => c.GetCacheAsync<IEnumerable<TaskItemDto>>(It.IsAny<string>()))
                          .ReturnsAsync((IEnumerable<TaskItemDto>)null);

                var mockEntities = new List<TaskItem> { new TaskItem { Id = 2, Title = "DB Task" } };
                _mockUnitOfWork.Setup(u => u.TaskItems.GetAllTaskItemsAsync(null))
                               .ReturnsAsync(mockEntities);

                var mapped = new List<TaskItemDto> { new TaskItemDto { Id = 2, Title = "Mapped" } };
                _mockMapper.Setup(m => m.Map<IEnumerable<TaskItemDto>>(mockEntities)).Returns(mapped);

                var result = await _service.GetAllTaskItemsAsync(null);

                Assert.Single(result);
                _mockCache.Verify(c => c.SetCacheAsync<IEnumerable<TaskItemDto>>(It.IsAny<string>(), mapped, It.IsAny<TimeSpan>()),Times.Once);
            }

            [Fact]
            public async Task GetTaskByIdAsync_ReturnsCachedItem_IfExists()
            {
                var cached = new TaskItemDto { Id = 5, Title = "Cached Task" };
                _mockCache.Setup(c => c.GetCacheAsync<TaskItemDto>("taskItem_5")).ReturnsAsync(cached);

                var result = await _service.GetTaskByIdAsync(5);
                Assert.Equal(5, result.Id);
            }

            [Fact]
            public async Task GetTaskByIdAsync_ReturnsNull_IfNotFound()
            {
                _mockCache.Setup(c => c.GetCacheAsync<TaskItemDto>(It.IsAny<string>()))
                          .ReturnsAsync((TaskItemDto)null);
                _mockUnitOfWork.Setup(u => u.TaskItems.GetByIdAsync(1))
                               .ReturnsAsync((TaskItem)null);

                var result = await _service.GetTaskByIdAsync(1);
                Assert.Null(result);
            }

            [Fact]
            public async Task CreateTaskAsync_CreatesTask_AndClearsCache()
            {
                var createDto = new CreateTaskItemDto { Title = "New Task" };
                var entity = new TaskItem { Id = 1, Title = "New Task" };
                var dto = new TaskItemDto { Id = 1, Title = "New Task" };

                _mockMapper.Setup(m => m.Map<TaskItem>(createDto)).Returns(entity);
                _mockMapper.Setup(m => m.Map<TaskItemDto>(entity)).Returns(dto);

                var result = await _service.CreateTaskAsync(createDto);

                Assert.Equal(1, result.Id);
                _mockUnitOfWork.Verify(u => u.TaskItems.AddAsync(entity), Times.Once);
                _mockCache.Verify(c => c.RemoveCacheAsync("TaskItems_"), Times.Once);
            }

            [Fact]
            public async Task DeleteTaskAsync_ReturnsFalse_IfNotFound()
            {
                _mockUnitOfWork.Setup(u => u.TaskItems.GetByIdAsync(99))
                               .ReturnsAsync((TaskItem)null);

                var result = await _service.DeleteTaskAsync(99);
                Assert.False(result);
            }

            [Fact]
            public async Task DeleteTaskAsync_RemovesTask_AndClearsCache()
            {
                var entity = new TaskItem { Id = 3, Title = "Task" };
                _mockUnitOfWork.Setup(u => u.TaskItems.GetByIdAsync(3))
                               .ReturnsAsync(entity);

                var result = await _service.DeleteTaskAsync(3);

                Assert.True(result);
                _mockUnitOfWork.Verify(u => u.TaskItems.Remove(entity), Times.Once);
                _mockCache.Verify(c => c.RemoveCacheAsync(It.IsAny<string>()), Times.AtLeastOnce);
            }

            [Fact]
            public async Task UpdateTaskAsync_ReturnsFalse_IfNotFound()
            {
                _mockUnitOfWork.Setup(u => u.TaskItems.GetByIdAsync(1)).ReturnsAsync((TaskItem)null);
                var result = await _service.UpdateTaskAsync(1, new CreateTaskItemDto());
                Assert.False(result);
            }

            [Fact]
            public async Task UpdateTaskAsync_UpdatesEntity_AndClearsCache()
            {
                var entity = new TaskItem { Id = 2, Title = "Old" };
                _mockUnitOfWork.Setup(u => u.TaskItems.GetByIdAsync(2)).ReturnsAsync(entity);

                var result = await _service.UpdateTaskAsync(2, new CreateTaskItemDto { Title = "Updated" });

                Assert.True(result);
                _mockUnitOfWork.Verify(u => u.TaskItems.Update(entity), Times.Once);
                _mockCache.Verify(c => c.RemoveCacheAsync(It.IsAny<string>()), Times.AtLeastOnce);
            }
    }
    
}
