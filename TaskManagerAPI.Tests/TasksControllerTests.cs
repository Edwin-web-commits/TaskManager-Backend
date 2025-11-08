using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskItemService> _mockService;
        private readonly Mock<ILogger<TaskItemsController>> _mockLogger;
        private readonly TaskItemsController _controller;

        public TasksControllerTests()
        {
            _mockService = new Mock<ITaskItemService>();
            _mockLogger = new Mock<ILogger<TaskItemsController>>();
            _controller = new TaskItemsController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTaskItems_ReturnsOk_WithTaskList()
        {
            // Arrange
            var mockTasks = new List<TaskItemDto>
            {
                new TaskItemDto { Id = 1, Title = "Task 1", Description = "Test", IsCompleted = false }
            };
            _mockService.Setup(s => s.GetAllTaskItemsAsync(null)).ReturnsAsync(mockTasks);

            // Act
            var result = await _controller.GetTaskItems(null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetTaskItem_ReturnsBadRequest_WhenInvalidId()
        {
            var result = await _controller.GetTaskItem(0);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid ID", bad.Value);
        }

        [Fact]
        public async Task GetTaskItem_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetTaskByIdAsync(999)).ReturnsAsync((TaskItemDto)null!);
            var result = await _controller.GetTaskItem(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTaskItem_ReturnsOk_WhenFound()
        {
            var dto = new TaskItemDto { Id = 1, Title = "Found Task" };
            _mockService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(dto);

            var result = await _controller.GetTaskItem(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TaskItemDto>(ok.Value);
            Assert.Equal(1, value.Id);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WhenModelInvalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var result = await _controller.CreateTask(new CreateTaskItemDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreated_WhenValid()
        {
            var dto = new CreateTaskItemDto { Title = "New Task" };
            var created = new TaskItemDto { Id = 5, Title = "New Task" };

            _mockService.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(created);

            var result = await _controller.CreateTask(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsType<TaskItemDto>(createdResult.Value);
            Assert.Equal(5, value.Id);
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.UpdateTaskAsync(99, It.IsAny<CreateTaskItemDto>())).ReturnsAsync(false);
            var result = await _controller.UpdateTaskItem(99, new CreateTaskItemDto());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsNoContent_WhenSuccessful()
        {
            _mockService.Setup(s => s.UpdateTaskAsync(1, It.IsAny<CreateTaskItemDto>())).ReturnsAsync(true);
            var result = await _controller.UpdateTaskItem(1, new CreateTaskItemDto());
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsBadRequest_WhenInvalidId()
        {
            var result = await _controller.DeleteTaskItem(0);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid ID", bad.Value);
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(false);
            var result = await _controller.DeleteTaskItem(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsNoContent_WhenSuccessful()
        {
            _mockService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(true);
            var result = await _controller.DeleteTaskItem(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
