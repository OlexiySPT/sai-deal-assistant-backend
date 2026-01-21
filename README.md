# Deal Assistant

A **sales management CRM** designed to help sales managers work with potential clients, track deals, and close opportunities to generate revenue. This is a **pet project** demonstrating modern .NET development practices and architectural patterns.

> **⚠️ Project Status**: Under active development. Developed according to the "Idea First" startup paradigm — core features are prioritized, with infrastructure and advanced features planned for future iterations.

---

## 🎯 Purpose

**Sai Deal Assistant** enables sales teams to:
- Manage potential clients and deals throughout the sales pipeline
- Track events, meetings, and touchpoints with prospects
- Organize deal metadata (tags, contact persons, deal types, etc.)
- Monitor deal states and progress toward closing

This project showcases:
- Clean Architecture principles
- CQRS pattern with MediatR
- Domain-Driven Design (DDD)
- Repository pattern with generic implementations
- Entity Framework Core with PostgreSQL
- RESTful API design

---

## 🏗️ Architecture

The project follows **Clean Architecture** with clear separation of concerns across four layers:

### Layers

```
┌─────────────────────────────────────────┐
│         WebApi (Presentation)           │  ← Controllers, Middleware, Program.cs
├─────────────────────────────────────────┤
│       Application (Use Cases)           │  ← CQRS Commands/Queries, DTOs, Handlers
├─────────────────────────────────────────┤
│          Domain (Business)              │  ← Entities, Repositories (interfaces), Exceptions
├─────────────────────────────────────────┤
│      Infrastructure (Data Access)       │  ← DbContext, Repositories (impl), Migrations
└─────────────────────────────────────────┘
│            Common (Shared)              │  ← Configuration, Extensions
└─────────────────────────────────────────┘
```

#### **1. Domain Layer**
- Core business entities: `Deal`, `Event`, `ContactPerson`, `DealTag`, `EventNote`
- Read-only reference entities: `DealType`, `DealState`, `EventType`, `EventState`
- Repository interfaces (`ICrudRepository<T>`, `IReadRepository<T>`)
- Domain exceptions (`InvalidSortColumnException`, etc.)
- No dependencies on other layers

#### **2. Application Layer**
- CQRS implementation using **MediatR**
- Commands: `AddDealTagCommand`, `UpdateDealCommand`, `UpdateEventCommand`
- Queries: `GetDealEventsQuery`, etc.
- DTOs for data transfer
- Business logic orchestration
- Depends on: Domain

#### **3. Infrastructure Layer**
- Entity Framework Core `AppDbContext`
- PostgreSQL integration with Npgsql
- Generic repository implementations (`CrudRepository<T>`, `ReadRepository<T>`)
- Database migrations and seeding
- Entity configurations (Fluent API)
- Features:
  - PostgreSQL `citext` extension for case-insensitive text
  - Expression indexes for optimized queries
  - Optimistic concurrency handling with retry logic
- Depends on: Application, Domain

#### **4. WebApi Layer**
- ASP.NET Core Web API
- RESTful controllers (e.g., `DealsController`)
- CORS policy configuration
- Global exception handling
- Swagger/OpenAPI documentation
- Depends on: Application, Infrastructure, Common

#### **5. Common Layer**
- Shared configuration (`AppConfiguration`)
- Dependency injection setup
- Cross-cutting concerns

---

## 🛠️ Technology Stack

| Category | Technology |
|----------|-----------|
| **Framework** | .NET 8 |
| **Language** | C# |
| **Database** | PostgreSQL |
| **ORM** | Entity Framework Core 8 |
| **Database Provider** | Npgsql (PostgreSQL) |
| **API Pattern** | RESTful |
| **Architecture Pattern** | Clean Architecture |
| **Design Patterns** | CQRS, Repository, Mediator |
| **Mediator Library** | MediatR |
| **Mapping** | AutoMapper |
| **API Documentation** | Swagger/OpenAPI |
| **Concurrency** | Optimistic Concurrency with EF Core |

---

## 📦 Domain Model

### Core Entities

- **Deal**: Represents a sales opportunity with name, description, industry, status, type, and state
- **Event**: Sales activities/meetings tied to deals (with agenda, result, date, contact person)
- **ContactPerson**: Individuals associated with deals
- **DealTag**: Tags for organizing deals
- **EventNote**: Notes attached to events

### Reference Entities (Read-Only)

- **DealType**: One-time Service, Series, Long-time Collaboration
- **DealState**: New, Contacted, In Progress, Proposal, Closed - Won, Closed - Lost
- **EventType**: Call, Meeting, Email, etc.
- **EventState**: Scheduled, Completed, Cancelled

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **PostgreSQL** 12+ (local or remote instance)
- **Docker** (optional, for containerized deployment)

### Configuration

The application uses `.env` file and `appsettings.json` for configuration.

**Example `.env` file:**

ConnectionStrings__AppConnection=Host=192.168.1.245;Database=dealassistantdatabase;Username=dealassiatantuser;Password=YourPassword
ConnectionStrings__MigrationConnection=Host=192.168.1.245;Database=dealassistantdatabase;Username=dealassiatant_migrator;Password=YourPassword
AllowedCorsOrigins=http://localhost:3000;https://localhost:3001


**Notes:**
- `AppConnection`: Used for runtime database operations
- `MigrationConnection`: Used for database migrations (typically requires elevated privileges)
- `AllowedCorsOrigins`: Semicolon-separated list of allowed CORS origins

### Database Setup

1. Ensure PostgreSQL is running
2. Update connection strings in `.env`
3. Run the application — migrations run automatically on startup:


dotnet run --project Sai.DealAssistant.WebApi


The application automatically:
- Applies pending migrations
- Seeds initial reference data (DealTypes, DealStates, etc.)
- Seeds test data in Development environment

### Running the Application


cd Sai.DealAssistant.WebApi
dotnet run


**Swagger UI**: Navigate to `https://localhost:{port}/swagger` to explore the API.

---

## 🧪 Key Features Implemented

✅ **CQRS Pattern**: Clear separation of read and write operations  
✅ **Repository Pattern**: Generic CRUD and read repositories  
✅ **Clean Architecture**: Domain-centric design with dependency inversion  
✅ **Database Migrations**: EF Core Code-First migrations  
✅ **Seeding**: Automated data seeding for reference and test data  
✅ **Optimistic Concurrency**: Retry logic for concurrent updates  
✅ **PostgreSQL Advanced Features**: `citext` extension, expression indexes  
✅ **CORS Support**: Configured for frontend integration  
✅ **Global Exception Handling**: Centralized error handling middleware  

---

## 🔄 CQRS Implementation

All business operations follow the **Command Query Responsibility Segregation** pattern:

### Commands (Write Operations)
- Modify state
- Return void or simple acknowledgment
- Examples: `AddDealTagCommand`, `UpdateDealCommand`, `UpdateEventCommand`

### Queries (Read Operations)
- Retrieve data without side effects
- Return DTOs
- Examples: `GetDealEventsQuery`

All commands and queries are handled by **MediatR** handlers in the Application layer.

---

## 📂 Project Structure


Sai.DealAssistant.Backend/
├── Sai.DealAssistant.Domain/
│   ├── Entities/
│   ├── Repositories/
│   └── Exceptions/
├── Sai.DealAssistant.Application/
│   ├── Entities/
│   │   ├── Commands/
│   │   └── Queries/
│   └── System/
├── Sai.DealAssistant.Infrastructure/
│   ├── Persistence/
│   │   ├── EntityConfigurations/
│   │   └── Migrations/
│   └── Repositories/
├── Sai.DealAssistant.WebApi/
│   ├── Controllers/
│   ├── Extensions/
│   └── Program.cs
└── Sai.DealAssistant.Common/
    └── Configuration/


---

## 🐳 Docker Support

Dockerfile is included for containerized deployment (infrastructure layer will include Docker Compose in future iterations).

---

## 🚧 Roadmap / Planned Features

This project is under active development. Upcoming features include:

- [ ] Authentication & Authorization (JWT, Identity)
- [ ] Advanced filtering and search capabilities
- [ ] Deal analytics and reporting dashboard
- [ ] Email integration for event tracking
- [ ] Real-time notifications (SignalR)
- [ ] File attachments for deals and events
- [ ] CI/CD pipelines (GitHub Actions)
- [ ] Comprehensive unit and integration tests
- [ ] Docker Compose for multi-container orchestration
- [ ] Frontend application integration
- [ ] Advanced auditing and logging

---

## 📄 License

This project is for educational and portfolio purposes.

---

## 👤 Author

**Olexiy** — [GitHub Profile](https://github.com/OlexiySPT)

---

## 🙏 Acknowledgments

Built with modern .NET practices and inspired by Clean Architecture principles from Robert C. Martin and Jason Taylor's reference implementations.
