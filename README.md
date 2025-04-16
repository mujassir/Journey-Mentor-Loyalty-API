# JourneyMentor Loyalty API

A modular loyalty points management system built with Clean Architecture principles, supporting customer rewards, point tracking, and redemption features.

---

## 🎯 Project Goals

- **Modularity**: Cleanly separated layers for domain logic, application features, infrastructure, and presentation.
- **Maintainability**: Built using best practices and patterns (CQRS, Dependency Injection, etc.).
- **Scalability**: Easily extendable to support new reward types, customer actions, and integrations.
- **Developer Experience**: Easy to set up, develop, test, and deploy with Docker and .NET tooling.

---

## 🧱 Key Features

- **Customer Loyalty Management**  
  Earn and redeem points for defined actions or purchases.

- **RESTful API with Swagger**  
  Fully documented endpoints for smooth integration with mobile apps, web frontends, or third-party systems.

- **Database Flexibility**  
  Ships with SQLite for development, but supports other providers (SQL Server, PostgreSQL, etc.).

- **Caching Support**  
  Redis integration for performance optimization.

- **Robust Testing**  
  Built-in support for unit and integration tests following clean test patterns.

---

## ⚙️ Technologies Used

- **.NET 8** – Cross-platform, high-performance backend framework.
- **Entity Framework Core** – ORM for database operations.
- **FluentValidation** – Input and business rule validation.
- **Docker** – Containerized deployments.
- **Redis** – Caching layer for improved performance.
- **Swagger / OpenAPI** – API documentation and testing.

---

## 🚀 Setup & Installation

### Prerequisites
- .NET 8 SDK
- Docker (optional for containerized deployment)
- SQLite (or configure another DB provider)

### Installation
1. **Clone the repository**
   ```
   git clone https://github.com/mujassir/Journey-Mentor-Loyalty-API.git
   cd JourneyMentor.Loyalty
   ```

2. **Restore dependencies**
   ```
   dotnet restore
   ```

3. **Database setup**  
   The system uses SQLite by default (auto-created on first run). For other databases:
   - Update `appsettings.json` connection strings
   - Run migrations via EF Core

## ▶️ Running the Project

### Development Mode
```
dotnet run --project JourneyMentor.Loyalty.API
```
Access: `http://localhost:5000/swagger`

### Docker Deployment
```
docker-compose up --build
```
Access: `http://localhost:8080`

## 📁 Project Structure
```
JourneyMentor.Loyalty/
├── API/                  # Presentation layer
│   ├── Controllers/      # API endpoints
│   ├── Middleware/       # Custom middleware
│   └── Logs/             # Application logs
│
├── Application/          # Business logic
│   ├── Features/         # CQRS implementations
│   └── Validators/       # FluentValidation rules
│
├── Domain/               # Core models
│   └── Entities/         # Aggregate roots
│
├── Infrastructure/       # Cross-cutting concerns
│   └── Redis/            # Caching implementations
│
├── Persistence/          # Data access
│   ├── Context/          # DbContext
│   ├── Migrations/       # Database migrations
│   └── Repositories/     # Data patterns
│
├── Tests/                # Unit/Integration tests
│   └── Handlers/         # CQRS handler tests
│
├── docker-compose.yml    # Container orchestration
└── Dockerfile            # API container config
```

## 🔧 Configuration

Key config files:
- `appsettings.json` - Database connections, logging
- `appsettings.Development.json` - Debug settings
- `docker-compose.yml` - Container ports/volumes

## 🛠️ Development Workflow

 Add domain models in `Domain/Entities/`
 - Implement business logic in `Application/Features/`
 - Create migrations:
```
dotnet ef migrations add [Name] --project JourneyMentor.Loyalty.Persistence
```
