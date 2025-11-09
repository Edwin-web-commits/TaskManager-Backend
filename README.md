# 📇Task manager App Backend

A Task manager App Backend web application built with **ASP.NET Core Web API**. t provides CRUD operations for managing tasks (create, edit, delete, toggle complete, filter, etc).

---

## 📦 Technologies Used

### 🔧 Backend (ASP.NET Core)
- .NET 8 Web API
- Entity Framework Core
- SQL Server (LocalDB or any)
- Repository Pattern with Unit of Work
- DTOs & AutoMapper
- Global Error Handling & Logging (with Serilog)
- Rate Limiting & Throttling (for production readiness)
- Caching with Redis

---

## 🚀 Features

- 🧑‍💼  create, edit, delete, toggle complete, filter tasks
- 🔍  Filter by completion status
- 📦  Global error handling and Caching
- 📊 Logging and rate limiting in production
- 📦 Modular, scalable architecture

---

## 📁 Project Structure

Backend
taskmanager-backend/
├── Controllers/
├── DTOs/
├── Helpers/
├── Interfaces/
├── Models/
├── Repositories/
├── Services/
├── appsettings.json

## Backend Setup
Clone the repository:
git clone <https://github.com/Edwin-web-commits/TaskManager-Backend.git 

cd taskmanager-backend
cd taskmanagerAPI

Update the appsettings.json file with your database and Redis configurations.

Apply database migrations:

dotnet ef database update

Run the backend:
dotnet run

API Endpoints
tasks

GET /api/tasks’ – Get all tasks (with optional query parameter ‘status=completed|pending’)
GET /api/tasks/{id}’ – Get a single task
POST /api/tasks’ – Create a new task
PUT /api/tasks/{id}’ – Update a task
DELETE /api/tasks/{id}’ – Delete a task

## Unit Testing
Backend Tests
Navigate to the test project directory:
cd TaskManagerAPI.Tests

Run the tests:
dotnet test

License

This project is licensed under the MIT License.

Contributors
Edwin Motlokwa - Developer

Feel free to contribute to this project by submitting issues or pull requests!
