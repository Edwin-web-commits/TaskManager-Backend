using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var createdAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete project documentation",
                    Description = "Finalize and submit the project documentation by end of the week.",
                    IsCompleted = true,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Implement user authentication",
                    Description = "Set up JWT authentication for the API.",
                    IsCompleted = false,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Design database schema",
                    Description = "Create the initial database schema using EF Core.",
                    IsCompleted = true,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 4,
                    Title = "Set up CI/CD pipeline",
                    Description = "Configure GitHub Actions for automated testing and deployment.",
                    IsCompleted = false,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 5,
                    Title = "Write unit tests",
                    Description = "Implement unit tests for the service layer.",
                    IsCompleted = false,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 6,
                    Title = "Create API documentation",
                    Description = "Generate API documentation using Swagger.",
                    IsCompleted = true,
                    CreatedAt = createdAt
                },
                new TaskItem
                {
                    Id = 7,
                    Title = "Deploy to production",
                    Description = "Deploy the application to the production environment.",
                    IsCompleted = false,
                    CreatedAt = createdAt
                }
            );
        }
    }
}

