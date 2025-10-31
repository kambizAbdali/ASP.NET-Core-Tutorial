# Task Management API

A comprehensive RESTful API for task management built with ASP.NET Core, following Clean Architecture principles and implementing industry best practices.

## 🚀 Features

- **RESTful API Design** - Fully compliant with REST principles
- **JWT Authentication** - Secure token-based authentication with refresh tokens
- **API Versioning** - Support for multiple API versions with backward compatibility
- **HATEOAS** - Hypermedia-driven API responses (Level 3 REST maturity)
- **Clean Architecture** - Separation of concerns with proper layer isolation
- **Entity Framework Core** - Data access with In-Memory database
- **Swagger Documentation** - Interactive API documentation
- **Soft Delete** - Logical deletion with data preservation
- **Comprehensive Testing** - Ready for unit and integration tests

## 🏗️ Architecture

The project follows Clean Architecture with clear separation of concerns:

```
TaskManagementSolution/
├── TaskManagement.API/           # Presentation Layer
├── TaskManagement.Core/          # Domain Layer (Entities, Interfaces, Business Logic)
├── TaskManagement.Infrastructure/# Data Layer (Repositories, EF Core, External Services)
└── TaskManagement.Tests/         # Test Projects
```

### Technology Stack

- **Framework**: ASP.NET Core 6.0
- **Authentication**: JWT Bearer Tokens
- **Database**: Entity Framework Core with In-Memory Provider
- **API Documentation**: Swagger/OpenAPI
- **Versioning**: ASP.NET API Versioning
- **Security**: JWT, Password Hashing (PBKDF2)

## 📋 Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🛠️ Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/TaskManagementAPI.git
cd TaskManagementAPI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Application

```bash
dotnet run --project TaskManagement.API
```

The API will be available at:
- **API**: https://localhost:7187
- **Swagger UI**: https://localhost:7187/swagger

## 🔑 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Follow these steps to authenticate:

### 1. Register a New User

```http
POST /api/v1.0/account/register
Content-Type: application/json

{
  "username": "yourusername",
  "password": "yourpassword",
  "email": "your@email.com"
}
```

### 2. Login to Get Access Token

```http
POST /api/v1.0/account/login
Content-Type: application/json

{
  "username": "yourusername",
  "password": "yourpassword"
}
```

Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "username": "yourusername"
}
```

### 3. Use the Access Token

Include the token in subsequent requests:

```http
Authorization: Bearer your_access_token_here
```

## 📚 API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Version |
|--------|----------|-------------|---------|
| POST | `/api/v1.0/account/register` | Register new user | v1.0 |
| POST | `/api/v1.0/account/login` | User login | v1.0 |
| POST | `/api/v1.0/account/refresh-token` | Refresh access token | v1.0 |
| POST | `/api/v1.0/account/logout` | User logout | v1.0 |

### Task Management Endpoints

#### Version 1.0

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1.0/tasks` | Get all user tasks |
| GET | `/api/v1.0/tasks/{id}` | Get task by ID |
| POST | `/api/v1.0/tasks` | Create new task |
| PUT | `/api/v1.0/tasks/{id}` | Update task |
| DELETE | `/api/v1.0/tasks/{id}` | Delete task (soft delete) |
| GET | `/api/v1.0/tasks/search` | Search tasks |

#### Version 2.0 (Enhanced Features)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2.0/tasks` | Get tasks with enhanced metadata |
| GET | `/api/v2.0/tasks/advanced-search` | Advanced search with filtering & sorting |
| GET | `/api/v2.0/tasks/{id}/analytics` | Get task analytics |

## 🔄 API Versioning

The API supports versioning through URL path:

- **Version 1.0**: `/api/v1.0/[controller]`
- **Version 2.0**: `/api/v2.0/[controller]`

Version 2.0 inherits from Version 1.0 and adds enhanced features while maintaining backward compatibility.

## 🎯 HATEOAS Implementation

The API implements HATEOAS (Hypermedia as the Engine of Application State) following Richardson Maturity Model Level 3. Each response includes relevant links for available actions:

```json
{
  "id": 1,
  "title": "Complete API Documentation",
  "description": "Write comprehensive documentation",
  "isCompleted": false,
  "priority": "High",
  "links": [
    {
      "href": "/api/v1.0/tasks/1",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "/api/v1.0/tasks/1",
      "rel": "update",
      "method": "PUT"
    },
    {
      "href": "/api/v1.0/tasks/1",
      "rel": "delete",
      "method": "DELETE"
    }
  ]
}
```

## 🗄️ Database

### In-Memory Database

The project uses Entity Framework Core with In-Memory database for development and testing:

- **No database setup required**
- **Automatic seeding with sample data**
- **Reset on application restart**

### Sample Data

On application startup, the database is seeded with:
- 2 sample users (admin/user1)
- 4 sample tasks with different priorities and statuses

### Entities

- **User**: User accounts with authentication
- **TaskItem**: Task entities with priority and status tracking
- **UserToken**: JWT refresh token management

## ⚙️ Configuration

### appsettings.json

```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyForJWTTokenGeneration",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "ExpireMinutes": 60,
    "RefreshTokenExpireDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `Jwt__Secret`: Override JWT secret key

## 🔒 Security Features

- **JWT Token Authentication**
- **Password Hashing** with PBKDF2 and salt
- **Refresh Token Rotation**
- **Soft Delete** for data preservation
- **Input Validation** with Data Annotations
- **CORS** configuration
- **HTTPS Redirection**

## 🚀 Deployment

### Development

```bash
dotnet run --project TaskManagement.API --environment Development
```

### Production

```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet TaskManagement.API.dll --environment Production
```

## 📖 Code Structure

### Core Layer (`TaskManagement.Core`)
- **Entities**: Domain models (TaskItem, User, UserToken)
- **Interfaces**: Repository and service contracts
- **DTOs**: Data transfer objects with validation
- **Services**: Business logic implementation
- **Exceptions**: Custom exception types

### Infrastructure Layer (`TaskManagement.Infrastructure`)
- **Data**: DbContext and entity configurations
- **Repositories**: EF Core repository implementations
- **Services**: Concrete implementations (PasswordHasher, TokenService)
- **Seed**: Database seeder with sample data

### API Layer (`TaskManagement.API`)
- **Controllers**: API endpoints with versioning
- **Program.cs**: Application configuration and startup
- **Middleware**: Authentication and error handling

## 🔧 Development Guidelines

### Adding New Features

1. **Start with Core Layer**: Define interfaces and DTOs
2. **Implement in Infrastructure**: Add repository and service implementations
3. **Expose in API Layer**: Create or update controllers
4. **Add Tests**: Write unit and integration tests

### Code Style

- Use meaningful names for classes and methods
- Follow RESTful conventions for API design
- Implement proper error handling
- Use async/await for I/O operations
- Apply dependency injection principles

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b ...`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Swagger Documentation](https://localhost:7187/swagger)
2. Review the API logs for detailed error information
3. Create an issue in the GitHub repository

## 🙏 Acknowledgments

- ASP.NET Core Team
- Entity Framework Core Team
- Swagger/OpenAPI Community
- Clean Architecture principles by Robert C. Martin

---

**Happy Coding!** 🚀

For more information, visit the [API Documentation](https://localhost:7187/swagger) or check the source code comments.