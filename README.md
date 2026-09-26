# SpyderByteAPI

A comprehensive .NET 8 RESTful API solution for managing games and related functionality. The project follows a layered architecture pattern with separated concerns across multiple projects.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Architecture](#architecture)
- [Features](#features)
- [Testing](#testing)
- [Security](#security)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

## Overview

SpyderByteAPI is a robust, production-ready API built with .NET 8 that provides endpoints for managing games and related operations. The solution emphasizes clean architecture, security, and comprehensive testing.

### Key Highlights

- **Latest Framework**: Built on .NET 8
- **Clean Architecture**: Well-organized layer separation (API, Services, Data Access, Resources)
- **Authentication & Authorization**: JWT-based security with feature flags
- **Rate Limiting**: Built-in IP-based rate limiting
- **Cloud Integration**: Azure services integration (Key Vault, Storage, Application Insights)
- **API Versioning**: Support for multiple API versions
- **Comprehensive Testing**: xUnit-based test suite with mocking and fixtures

## Project Structure

```
SpyderByteAPI/
├── SpyderByteAPI/                    # Main ASP.NET Core Web API project
│   ├── Controllers/                  # API endpoints
│   ├── Program.cs                    # Application configuration and startup
│   └── appsettings.json             # Configuration files
├── SpyderByteServices/               # Business logic and service layer
│   ├── Interfaces/                   # Service contracts
│   ├── Implementations/              # Service implementations
│   └── Mappers/                      # AutoMapper profiles
├── SpyderByteDataAccess/             # Data access layer with EF Core
│   ├── DbContexts/                   # Entity Framework contexts
│   ├── Repositories/                 # Data repository implementations
│   └── Migrations/                   # Database migrations
├── SpyderByteResources/              # Shared resources and utilities
│   ├── DTOs/                         # Data Transfer Objects
│   ├── Extensions/                   # Extension methods
│   ├── Enums/                        # Shared enumerations
│   └── Constants/                    # Application constants
├── SpyderByteAPITest/                # Unit and integration tests
│   ├── API/                          # API endpoint tests
│   ├── Services/                     # Service layer tests
│   └── DataAccess/                   # Data access layer tests
└── Directory.Packages.props          # Centralized package version management
```

## Tech Stack

### Core Framework
- **.NET 8**: Latest .NET runtime
- **ASP.NET Core**: Web API framework

### Data Access
- **Entity Framework Core**: ORM
- **SQLite**: Development database
- **Microsoft.EntityFrameworkCore.Sqlite**: Database provider

### Authentication & Security
- **JWT (JSON Web Tokens)**: Token-based authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT integration
- **Argon2**: Password hashing library
- **Azure Key Vault**: Secrets management
- **Azure Identity**: Azure authentication

### Azure Services
- **Azure Storage Blobs**: File storage
- **Application Insights**: Monitoring and diagnostics
- **Azure Key Vault**: Secure configuration management

### API & Documentation
- **Swashbuckle.AspNetCore**: OpenAPI/Swagger support
- **Asp.Versioning.Mvc**: API versioning
- **AspNetCoreRateLimit**: Rate limiting

### Additional Libraries
- **AutoMapper**: Object-to-object mapping
- **Newtonsoft.Json**: JSON serialization
- **Microsoft.FeatureManagement**: Feature flags

### Testing
- **xUnit**: Testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertion library
- **AutoFixture**: Test data generation
- **Coverlet**: Code coverage
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for testing

## Prerequisites

- **.NET 8 SDK** or later
- **Visual Studio 2022** (Community or higher) or **Visual Studio Code**
- **Git**
- (Optional) **Azure subscription** for cloud features

## Getting Started

### 1. Clone the Repository

```
git clone https://github.com/ojn17041991/SpyderByteAPI.git
cd SpyderByteAPI
```

### 2. Restore Dependencies

```
dotnet restore
```

### 3. Build the Solution

```
dotnet build
```

### 4. Run Migrations

```
dotnet ef database update --project SpyderByteDataAccess --startup-project SpyderByteAPI
```

### 5. Run the API

```
dotnet run --project SpyderByteAPI
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

### 6. Access Swagger Documentation

Navigate to `https://localhost:5001/swagger` to view the interactive API documentation.

## Configuration

### appsettings.json

Configure the following sections:

```
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=spyderbyte.db"
  },
  "Authentication": {
	"JwtSecret": "your-secret-key-here"
  },
  "Azure": {
	"KeyVault": {
	  "VaultUri": "https://your-vault.vault.azure.net/"
	}
  },
  "ApplicationInsights": {
	"InstrumentationKey": "your-instrumentation-key"
  }
}
```

### User Secrets (Development)

For sensitive configuration in development:

```
dotnet user-secrets set "Authentication:JwtSecret" "your-secret-key"
```

### Environment Variables

Set the following for production:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection`
- `Authentication__JwtSecret`
- `Azure__KeyVault__VaultUri`

## Architecture

### Layered Architecture Pattern

The solution follows a clean layered architecture:

```
┌─────────────────────────────────┐
│   SpyderByteAPI (Presentation)  │ ← HTTP Requests
├─────────────────────────────────┤
│  SpyderByteServices (Business)  │ ← Business Logic
├─────────────────────────────────┤
│  SpyderByteDataAccess (Data)    │ ← Data Operations
├─────────────────────────────────┤
│  SpyderByteResources (Shared)   │ ← DTOs & Models
└─────────────────────────────────┘
```

### Key Layers

- **API Layer**: Handles HTTP requests/responses, routing, and controller logic
- **Service Layer**: Implements business logic, data transformation, and orchestration
- **Data Access Layer**: Manages database operations using Entity Framework Core
- **Resources Layer**: Contains shared DTOs, enums, extensions, and utilities

## Features

### ✅ Implemented Features

- **RESTful API Endpoints**: Comprehensive CRUD operations
- **API Versioning**: Support for multiple API versions (v1.4 and others)
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Fine-grained access control
- **Rate Limiting**: IP-based request throttling
- **CORS Support**: Cross-origin resource sharing configuration
- **Swagger/OpenAPI**: Interactive API documentation
- **Azure Integration**: Key Vault, Storage, and Insights
- **Feature Flags**: Dynamic feature management
- **Error Handling**: Comprehensive error and exception handling
- **Logging**: Application Insights integration
- **Database Migrations**: Version-controlled schema management

### 🎮 Games Controller

The API provides endpoints for managing games:
- `GET /api/v{version}/games` - List all games
- `GET /api/v{version}/games/{id}` - Get a specific game
- `POST /api/v{version}/games` - Create a new game
- `PUT /api/v{version}/games/{id}` - Update a game
- `DELETE /api/v{version}/games/{id}` - Delete a game

## Testing

### Run All Tests

```
dotnet test
```

### Run Specific Test Project

```
dotnet test SpyderByteAPITest
```

### Run with Code Coverage

```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Structure

Tests are organized by layer:
- `API/GamesControllerTests/` - Controller endpoint tests
- `Services/` - Business logic tests
- `DataAccess/` - Repository and data access tests

Each test class uses:
- **xUnit** for test framework
- **Moq** for mocking dependencies
- **FluentAssertions** for readable assertions
- **AutoFixture** for test data generation

## Security

### Authentication

The API uses JWT (JSON Web Tokens) for authentication:

1. User credentials are validated
2. A JWT token is issued upon successful authentication
3. Token is included in subsequent requests via the `Authorization` header
4. Token is validated on protected endpoints

### Password Security

Passwords are hashed using **Argon2**, a modern password hashing algorithm providing excellent security.

### Rate Limiting

IP-based rate limiting protects against abuse:
- Configured per endpoint
- Can be customized in configuration
- Returns 429 (Too Many Requests) when limits are exceeded

### CORS

Cross-Origin Resource Sharing is configured to allow requests from authorized origins only.

### Azure Key Vault

Sensitive configuration (secrets, connection strings) is stored in Azure Key Vault:
- Centralized secret management
- Role-based access control
- Audit logging

## Deployment

### Local Development

```
dotnet run --project SpyderByteAPI
```

### Docker (if configured)

```
docker build -t spyderbyte-api .
docker run -p 5001:8080 spyderbyte-api
```

### Azure App Service

1. Create an App Service in Azure
2. Configure connection strings and secrets
3. Deploy using Visual Studio publish or `dotnet publish`

### Environment Configuration

Update `appsettings.{Environment}.json` for different environments:
- `appsettings.Development.json`
- `appsettings.Production.json`
- `appsettings.Staging.json`

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/AmazingFeature`
3. **Commit changes**: `git commit -m 'Add AmazingFeature'`
4. **Push to branch**: `git push origin feature/AmazingFeature`
5. **Open a Pull Request**

### Code Style

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Write unit tests for new features
- Ensure all tests pass before submitting PR

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For questions or issues, please:
1. Check existing GitHub issues
2. Review the Swagger documentation at `/swagger`
3. Contact the maintainers

---

**Repository**: [https://github.com/ojn17041991/SpyderByteAPI](https://github.com/ojn17041991/SpyderByteAPI)  
**Framework**: .NET 8  
**Last Updated**: September 2026
