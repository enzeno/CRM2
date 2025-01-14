# CRM2 Project

A Customer Relationship Management system built with:
- .NET 8
- C#
- SQL Server

## Project Structure
- `CRM2.API`: Web API project containing the REST endpoints and business logic

## Prerequisites
- .NET 8 SDK
- SQL Server (2019 or later recommended)

## Getting Started
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project CRM2.API
   ```

## Development Notes
- The project uses Entity Framework Core for database operations
- Swagger UI is available at `/swagger` endpoint when running in development mode
- Global Invariant is disabled to support SQL Server date/time operations 