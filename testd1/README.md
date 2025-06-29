# Enterprise CRUD API - Employee Management System

## Overview

A comprehensive Employee Management API built with **.NET 9**, implementing **Clean Architecture** principles and enterprise-grade best practices. This project showcases professional software development standards suitable for large-scale enterprise applications.

## Architecture

This application follows **Clean Architecture** principles with clear separation of concerns:

```
├── EnterpriseCrudApp.Domain/          # Business entities and interfaces
├── EnterpriseCrudApp.Application/     # Business logic and use cases
├── EnterpriseCrudApp.Infrastructure/  # Data access and external concerns
└── EnterpriseCrudApp.API/            # Presentation layer and API endpoints
```

### Key Architectural Patterns:
- **Clean Architecture** - Clear dependency inversion and separation of concerns
- **Repository Pattern** - Abstraction over data access
- **Unit of Work Pattern** - Transaction management
- **CQRS-like separation** - Clear command/query responsibilities
- **Domain-Driven Design** - Rich domain models with business logic

## Features

### Core Functionality
- **Full CRUD Operations** - Create, Read, Update, Delete employees
- **Advanced Querying** - Search by ID, email, department, active status
- **Business Operations** - Activate/Deactivate employees
- **Data Validation** - Comprehensive input validation with FluentValidation
- **Error Handling** - Global exception handling with proper HTTP status codes

### Enterprise Features
- **Comprehensive Logging** - Structured logging with Serilog-style patterns
- **Health Checks** - Database and application health monitoring
- **Security Headers** - XSS protection, content type options, frame options
- **CORS Support** - Cross-origin resource sharing configuration
- **API Documentation** - Swagger/OpenAPI documentation
- **AutoMapper** - Object-to-object mapping
- **In-Memory Database** - Entity Framework Core with seeded data

### Data Model
```csharp
Employee Entity:
- Id (int) - Primary key
- FirstName (string) - Required, max 100 chars
- LastName (string) - Required, max 100 chars  
- Email (string) - Required, unique, max 255 chars
- PhoneNumber (string) - Optional, max 20 chars
- Department (string) - Required, validated list
- Position (string) - Required, max 100 chars
- Salary (decimal) - Required, positive value
- HireDate (DateTime) - Required, not future date
- IsActive (bool) - Default true
- CreatedAt (DateTime) - Auto-generated
- UpdatedAt (DateTime) - Auto-updated
- FullName (computed) - FirstName + LastName
```

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Framework** | .NET 9, ASP.NET Core |
| **Database** | Entity Framework Core (In-Memory) |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Documentation** | Swagger/OpenAPI |
| **Testing** | Postman Collection |
| **Architecture** | Clean Architecture, DDD |

## Project Structure

```
EnterpriseCrudApp/
├── src/
│   ├── EnterpriseCrudApp.Domain/
│   │   ├── Entities/           # Domain entities
│   │   └── Interfaces/         # Repository interfaces
│   ├── EnterpriseCrudApp.Application/
│   │   ├── DTOs/              # Data transfer objects
│   │   ├── Services/          # Business logic services
│   │   ├── Interfaces/        # Service interfaces
│   │   ├── Mappings/          # AutoMapper profiles
│   │   └── Validators/        # FluentValidation rules
│   ├── EnterpriseCrudApp.Infrastructure/
│   │   ├── Data/              # DbContext and configurations
│   │   ├── Repositories/      # Repository implementations
│   │   └── UnitOfWork/        # Transaction management
│   └── EnterpriseCrudApp.API/
│       ├── Controllers/       # API controllers
│       └── Program.cs         # Application startup
├── Enterprise_CRUD_API.postman_collection.json
└── README.md
```



### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EnterpriseCrudApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   cd src/EnterpriseCrudApp.API
   dotnet run
   ```

5. **Access the application**
   - API Base URL: `http://localhost:5064`
   - Swagger UI: `http://localhost:5064/swagger`
   - Health Check: `http://localhost:5064/health`

## API Endpoints

### Employee CRUD Operations

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/employees` | Get all employees |
| `GET` | `/api/employees/{id}` | Get employee by ID |
| `GET` | `/api/employees/email/{email}` | Get employee by email |
| `GET` | `/api/employees/department/{dept}` | Get employees by department |
| `GET` | `/api/employees/active` | Get active employees |
| `POST` | `/api/employees` | Create new employee |
| `PUT` | `/api/employees/{id}` | Update employee |
| `DELETE` | `/api/employees/{id}` | Delete employee |
| `PATCH` | `/api/employees/{id}/activate` | Activate employee |
| `PATCH` | `/api/employees/{id}/deactivate` | Deactivate employee |
| `HEAD` | `/api/employees/{id}` | Check if employee exists |

### System Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/` | API welcome information |
| `GET` | `/health` | Health check status |
| `GET` | `/swagger` | API documentation |

## Testing with Postman

### Import the Collection
1. Open Postman
2. Click **Import**
3. Select `Enterprise_CRUD_API.postman_collection.json`

### Sample Requests

**Get All Employees:**
```http
GET http://localhost:5064/api/employees
Accept: application/json
```

**Create Employee:**
```http
POST http://localhost:5064/api/employees
Content-Type: application/json

{
  "firstName": "manoj",
  "lastName": "yt",
  "email": "manojyt@company.com",
  "phoneNumber": "+91-9876543213",
  "department": "Engineering",
  "position": "Software Engineer",
  "salary": 85000,
  "hireDate": "2024-01-15T00:00:00Z"
}
```

## Enterprise Features

### Validation Rules
- **Names**: Letters and spaces only, 1-100 characters
- **Email**: Valid format, unique, max 255 characters  
- **Phone**: Valid international format, max 20 characters
- **Department**: Must be from predefined list
- **Salary**: Positive number, reasonable limits
- **Hire Date**: Cannot be in the future, after 1900

### Error Handling
- **400 Bad Request** - Invalid input data
- **404 Not Found** - Resource not found
- **409 Conflict** - Duplicate email addresses
- **500 Internal Server Error** - Server errors

### Security
- **HTTPS Enforced** - Secure connections only
- **Security Headers** - XSS protection, content type options
- **CORS Enabled** - Cross-origin support
- **Input Validation** - Prevents injection attacks

## Monitoring & Health

### Health Checks
Access `/health` to get system status:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "EnterpriseCrudApp.Infrastructure.Data.ApplicationDbContext": {
      "status": "Healthy",
      "duration": "00:00:00.0098765"
    }
  }
}
```

