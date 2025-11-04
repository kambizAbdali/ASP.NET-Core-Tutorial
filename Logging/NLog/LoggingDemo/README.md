# ASP.NET Core Logging Demo

A comprehensive logging demonstration project built with ASP.NET Core MVC 6.0, showcasing various logging techniques and configurations using NLog.

## 📋 Project Overview

This project demonstrates professional logging practices in ASP.NET Core, including multiple log output formats, structured logging, log archiving, and database integration.

## 🚀 Features

### Logging Capabilities
- **Multiple Log Levels**: Trace, Debug, Information, Warning, Error, Critical
- **Various Output Targets**:
  - Colored Console Output
  - File Logging with Archiving
  - JSON Structured Logging
  - Database Storage (SQL Server)
  - Custom Format Logs
- **Structured Logging**: Object serialization to JSON
- **Log Rotation**: Automatic archiving based on size and date
- **Async Logging**: Non-blocking log operations

### Application Features
- User Management (CRUD operations)
- Product Management
- Database connectivity testing
- NLog configuration diagnostics
- Error simulation for testing

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 6.0 MVC
- **Logging**: NLog 5.0
- **Database**: SQL Server
- **Frontend**: Bootstrap 5.1
- **ORM**: Microsoft Data SqlClient

## 📁 Project Structure

```
LoggingDemo/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models
├── Services/            # Business Logic Services
├── Views/               # Razor Views
├── wwwroot/            # Static Files
├── Database/           # SQL Scripts
├── Program.cs          # Application Entry Point
├── appsettings.json    # Configuration
├── NLog.config         # Logging Configuration
└── README.md           # This file
```

## ⚙️ Setup Instructions

### Prerequisites
- .NET 6.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone or Download the Project**
   ```bash
   git clone <repository-url>
   cd LoggingDemo
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Database Setup**
   - Run the SQL script in `Database/CreateLoggingDatabase.sql`
   - Or use the provided setup in the application

4. **Configure Connection String**
   - Update the connection string in `NLog.config` if needed
   - Default: `Server=.;Database=LoggingDb;Trusted_Connection=true`

5. **Run the Application**
   ```bash
   dotnet run
   ```
   Or press F5 in Visual Studio

## 🎯 Usage Guide

### Testing Logging Features

1. **Home Page** (`/`)
   - Test all log levels
   - Simulate errors and critical situations
   - Check database connectivity
   - View NLog targets

2. **User Management** (`/Users`)
   - Create, view, and delete users
   - Observe user-related logging

3. **Product Management** (`/Products`)
   - Create and edit products
   - Test different log levels based on product values

4. **API Endpoints**
   - `/api/test/database` - Test database connection
   - `/api/test/nlog-targets` - Check loaded NLog targets

### Log Output Locations

| Output Type | Location | Description |
|-------------|----------|-------------|
| Console | Application Console | Colored output (Development) |
| All Logs | `logs/all-{date}.log` | All logs (Debug and above) |
| Error Logs | `logs/error-{date}.log` | Warnings and Errors only |
| JSON Logs | `logs/json-{date}.json` | Structured JSON format |
| Custom Logs | `logs/custom-{date}.log` | Custom format for services |
| Database | `ApplicationLogs` table | Error logs in SQL Server |
| Internal Log | `internal-nlog.txt` | NLog configuration logs |

## 🔧 Configuration

### NLog.config Key Sections

- **Targets**: Define output destinations
- **Rules**: Route logs to specific targets
- **Variables**: Global configuration values
- **Archiving**: Automatic log rotation

### Customizing Log Levels

Modify the `NLog.config` rules section:
```xml
<!-- Example: Change log level for specific namespace -->
<logger name="LoggingDemo.Services.*" minlevel="Debug" writeTo="customFile" />
```

### Database Configuration

The application uses the following database structure:
```sql
CREATE TABLE ApplicationLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    TimeStamp DATETIME2 NOT NULL,
    Level NVARCHAR(50) NOT NULL,
    Logger NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX),
    Exception NVARCHAR(MAX),
    MachineName NVARCHAR(255),
    UserName NVARCHAR(255),
    ApplicationName NVARCHAR(100),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
```

## 📊 Log Analysis

### Useful SQL Queries

```sql
-- Get error statistics by day
SELECT CAST(TimeStamp AS DATE) as LogDate, Level, COUNT(*) as Count
FROM ApplicationLogs 
WHERE Level IN ('Error', 'Critical')
GROUP BY CAST(TimeStamp AS DATE), Level
ORDER BY LogDate DESC;

-- Find most frequent error sources
SELECT Logger, COUNT(*) as ErrorCount
FROM ApplicationLogs 
WHERE Level = 'Error'
GROUP BY Logger
ORDER BY ErrorCount DESC;
```

### JSON Log Structure

```json
{
  "timestamp": "2024-01-15 14:35:22",
  "level": "INFORMATION",
  "logger": "LoggingDemo.Services.UserService",
  "message": "User created successfully",
  "eventProperties": {
    "User": {
      "Id": 123,
      "FirstName": "John",
      "LastName": "Doe",
      "Email": "john@example.com"
    }
  }
}
```

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Errors**
   - Verify SQL Server is running
   - Check connection string in NLog.config
   - Ensure LoggingDb database exists

2. **NLog Configuration Errors**
   - Check `internal-nlog.txt` for detailed errors
   - Verify NLog.config XML syntax
   - Ensure all required packages are installed

3. **Log Files Not Created**
   - Check application permissions
   - Verify log directory exists
   - Review NLog internal logs

### Debugging Steps

1. Check `internal-nlog.txt` for NLog configuration issues
2. Use the test endpoints to verify connectivity
3. Review application event logs
4. Check database connection with the test controller

## 📈 Best Practices Demonstrated

1. **Structured Logging**: Using `{@Object}` syntax for object serialization
2. **Log Levels**: Appropriate use of different log levels
3. **Exception Handling**: Proper exception logging with context
4. **Performance**: Async logging to avoid blocking
5. **Security**: No sensitive data in logs
6. **Maintenance**: Log rotation and archiving

## 🎓 Learning Objectives

After studying this project, you should understand:

- How to configure NLog in ASP.NET Core
- Different log levels and when to use them
- Multiple output targets configuration
- Structured logging techniques
- Database logging implementation
- Log archiving and maintenance
- Error handling with proper logging

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

## 📄 License

This project is for educational purposes. Feel free to use and modify as needed.

## 🙏 Acknowledgments

- NLog team for excellent logging library
- ASP.NET Core team
- Bootstrap team for UI components

---

**Happy Logging! 🎉**