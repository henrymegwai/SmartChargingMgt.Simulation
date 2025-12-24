# GreenFlux Smart Charging Assignment 

## Version 25

## Assignment Completion

## Overview

This is a RESTful HTTP API for managing a simplified smart charging domain. The solution implements a clean architecture with the following layers:

- **Domain Layer**: Core entities and business logic (Group, ChargeStation, Connector)
- **Application Layer**: Commands, Queries, Handlers, and Validation (using MediatR and FluentValidation)
- **Infrastructure Layer**: Data access and repositories (Entity Framework Core with In-Memory Database)
- **API Layer**: REST controllers and dependency injection configuration

## Architecture

The solution follows Clean Architecture principles:

- **Domain**: Contains entities with business rules
- **Application**: Contains use cases (CQRS with MediatR), DTOs, validators, and pipeline behaviors
- **Infrastructure**: Contains data access (EF Core, repositories, entityconfigurations)
- **API**: Contains controllers and API configuration

### Key Technologies

- **.NET 9.0**
- **MediatR**: For CQRS pattern implementation
- **FluentValidation**: For input validation
- **Entity Framework Core**: For data access
- **Entity Framework Core In-Memory**: For database storage (no setup required)
- **Swagger/OpenAPI**: For API documentation
- **xUnit, NSubstitute, FluentAssertions**: For unit testing

## Domain Model

The domain consists of three main entities:

1. **Group**
   - Unique Identifier (immutable)
   - Name (mutable)
   - Capacity in Amps (integer > 0, mutable)
   - Can contain multiple Charge Stations

2. **Charge Station**
   - Unique Identifier (immutable)
   - Name (mutable)
   - Must belong to exactly one Group
   - Must have 1-5 Connectors

3. **Connector**
   - Identifier (1-5, unique within Charge Station)
   - Max Current in Amps (integer > 0, mutable)
   - Must belong to exactly one Charge Station

## Business Rules

1. Groups, Charge Stations, and Connectors can be created, updated, and removed
2. If a Group is removed, all Charge Stations in the Group are removed (cascade delete)
3. Only one Charge Station can be added/removed to a Group in one call
4. A Charge Station can only be in one Group at a time
5. A Charge Station cannot exist without a Group
6. A Connector cannot exist without a Charge Station
7. The Capacity in Amps of a Group must always be greater than or equal to the sum of the Max Current in Amps of all Connectors indirectly belonging to the Group
8. All operations not meeting the above requirements are rejected

## Prerequisites

- **.NET 9.0 SDK** or later
- **Visual Studio 2022** (or later) or **Visual Studio Code** with C# extension
- **Git** (for cloning the repository)

## Setup Instructions

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd SmartChargingManagement
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the API**:
   ```bash
   dotnet run --project src/SmartChargingManagement.Api
   ```

   The API will start on `https://localhost:7151` or `http://localhost:5125` (ports may vary based on your configuration).

5. **Access Swagger UI**:
   - Navigate to `http://localhost:5125/index.html` (or the port shown in the console) or
   - Navigate to `https://localhost:7151/index.html` (or the port shown in the console)
   - The Swagger UI provides an interactive interface to test all API endpoints

## Database Setup

The solution uses **SQLite** database with a local file `SmartCharging.db`:
- The database file (`SmartCharging.db`) will be automatically created in the API project directory (`src/SmartChargingManagement.Api/`) when you first run the application
- The database file is included in the repository (not gitignored) for easy setup and data persistence
- No database server installation needed (SQLite is file-based)
- Data persists between application restarts
- The connection string is configured in `appsettings.json`: `Data Source=SmartCharging.db`
- The database schema is automatically created on first run using `EnsureCreated()`

## API Endpoints

### Groups

- `GET /api/groups` - Get all groups
- `GET /api/groups/{id}` - Get group by ID
- `POST /api/groups` - Create a new group
- `PUT /api/groups/{id}` - Update a group
- `DELETE /api/groups/{id}` - Delete a group

### Charge Stations

- `GET /api/chargestations` - Get all charge stations
- `GET /api/chargestations/{id}` - Get charge station by ID
- `POST /api/chargestations` - Create a new charge station
- `PUT /api/chargestations/{id}` - Update a charge station
- `DELETE /api/chargestations/{id}` - Delete a charge station

### Connectors

- `GET /api/connectors/{chargeStationId}/{id}` - Get connector by ID and charge station ID
- `POST /api/connectors` - Create a new connector
- `PUT /api/connectors/{chargeStationId}/{id}` - Update a connector
- `DELETE /api/connectors/{chargeStationId}/{id}` - Delete a connector

## Running Tests

To run the unit tests:

```bash
dotnet test
```

To run tests with detailed output:

```bash
dotnet test --verbosity normal
```

## Project Structure

```
SmartChargingManagement/
├── src/
│   ├── SmartChargingManagement.Api/          # API layer (controllers, Program.cs)
│   ├── SmartChargingManagement.Application/  # Application layer (commands, handlers, validators)
│   ├── SmartChargingManagement.Domain/       # Domain layer (entities)
│   └── SmartChargingManagement.Infrastructure/ # Infrastructure layer (repositories, DbContext)
└── tests/
    └── SmartChargingManagement.UnitTests/    # Unit tests
```

## Key Design Decisions

1. **Clean Architecture**: Separation of concerns with clear layer boundaries
2. **CQRS Pattern**: Commands and Queries separated using MediatR
3. **Pipeline Behaviors**: Validation and Logging implemented as cross-cutting concerns
4. **FluentValidation**: Input validation separated from business logic
5. **Repository Pattern**: Abstraction of data access for testability
6. **Domain-Driven Design**: Business logic encapsulated in domain entities
7. **In-Memory Database**: Simplifies setup while maintaining EF Core functionality

## Example Usage

### Creating a Group

```json
POST /api/groups
{
  "name": "Group A",
  "capacityInAmps": 100
}
```

### Creating a Charge Station

```json
POST /api/chargestations
{
  "name": "Station 1",
  "groupId": "<group-id>"
}
```

### Creating a Connector

```json
POST /api/connectors
{
  "id": 1,
  "maxCurrentInAmps": 50,
  "chargeStationId": "<charge-station-id>"
}
```

## Notes

- All validation errors return appropriate HTTP status codes (400 Bad Request, 404 Not Found, etc.)
- Business rule violations return 400 Bad Request with descriptive error messages
- The API uses RESTful conventions
- OpenAPI/Swagger documentation is automatically generated

## Future Extensibility

The architecture supports easy extension:
- Replace In-Memory database with SQL Server/PostgreSQL by changing the DbContext configuration
- Add new features by creating new commands/queries in the Application layer
- Add new validation rules in FluentValidation validators
- Add new pipeline behaviors for cross-cutting concerns (caching, authorization, etc.)
- Add integration tests in a separate test project

