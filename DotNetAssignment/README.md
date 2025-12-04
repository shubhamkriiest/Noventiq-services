# DotNetAssignment

A clean ASP.NET Core 8 Web API for user and role management featuring JWT authentication, role‑based authorization, multilingual responses (English/Hindi), Swagger, and a secure refresh‑token flow.

## Overview
- Tech: ASP.NET Core 8, Entity Framework Core (SQL Server), JWT
- Architecture: Repository + Service layers, DTOs, Controllers
- Auth: Access tokens (JWT) + rotating refresh tokens
- i18n: Simple JSON localization using `Resources/i18n/en.json` and `hi.json`
- Documentation: Swagger UI

## Requirements
- .NET SDK 8.0
- SQL Server instance (local or remote)
- PowerShell or Visual Studio/VS Code

## Configuration
1. Update `appsettings.json`:
   - `ConnectionStrings:DefaultConnection` with your SQL Server details
   - `JwtSettings`: `SecretKey`, `Issuer`, `Audience`, `ExpiryMinutes`
2. Optional: Confirm roles seeded in `ApplicationDbContext` (Admin, User)

## Setup
```powershell
# From the DotNetAssignment folder
dotnet restore
# Install EF Core CLI (once per machine)
dotnet tool install --global dotnet-ef

# Create the initial migration (run once, or when your models change)
# Replace "InitialCreate" with a meaningful name if you prefer
dotnet ef migrations add InitialCreate

# Apply migrations to create/update the database
dotnet ef database update
```

## Run
```powershell
dotnet build
dotnet run
```
- Swagger UI: `http://localhost:<port>/swagger`
- API base: `http://localhost:<port>/api`

## Project Structure (key folders)
- `Controllers/` – API endpoints (`Auth`, `Users`, `Roles`)
- `Services/` – Business logic (`AuthService`, `UserService`, `RoleService`, `SimpleLocalizer`)
- `Repositories/` – Data access (`UserRepository`, `RoleRepository`, `RefreshTokenRepository`)
- `Models/` – Entities (`User`, `Role`, `RefreshToken`)
- `DTOs/` – Request/response models
- `Resources/i18n/` – Localization JSON (`en.json`, `hi.json`)
- `Data/ApplicationDbContext.cs` – EF Core context & relationships

## Authentication & Authorization
- Login to obtain a JWT access token
- Use `Authorization: Bearer <access_token>` to call protected endpoints
- Role restrictions via attributes, e.g. `Authorize(Roles = "Admin")`

### Endpoints (Auth)
- `POST /api/Auth/login`
  - Body:
    ```json
    { "username": "<name>", "password": "<password>" }
    ```
  - Returns: `token`, `expiresAt`, `refreshToken`, `username`, `email`, `role`

- `POST /api/Auth/register`
  - Body:
    ```json
    { "username": "<name>", "email": "<email>", "password": "<password>", "roleId": 2 }
    ```
  - Returns: localized `message`

- `POST /api/Auth/refresh`
  - Body:
    ```json
    { "refreshToken": "<the_refresh_token_from_login_response>" }
    ```
  - Returns: new `token`, `expiresAt`, and rotated `refreshToken`

### Endpoints (Users)
- `GET /api/Users` (authorized)
- `GET /api/Users/{id}` (authorized)
- `POST /api/Users` (Admin)
- `PUT /api/Users/{id}` (Admin)
- `DELETE /api/Users/{id}` (Admin)

### Endpoints (Roles)
- `GET /api/Roles` (authorized)
- `GET /api/Roles/{id}` (authorized)
- `POST /api/Roles` (Admin)
- `PUT /api/Roles/{id}` (Admin)
- `DELETE /api/Roles/{id}` (Admin)

## Multilingual Responses
- Include `Accept-Language` header in requests:
  - English: `Accept-Language: en`
  - Hindi: `Accept-Language: hi`
- Messages are sourced from `Resources/i18n/en.json` and `Resources/i18n/hi.json`
- The `SimpleLocalizer` reads the header and provides the appropriate translation

## Refresh Token Flow
- On successful login, the API issues an access token and a refresh token
- Use `POST /api/Auth/refresh` with the latest refresh token to get a new access token
- Rotation: refresh tokens are single‑use; the old token is revoked after refresh
- Storage: `RefreshToken` entity persisted via `RefreshTokenRepository`


## License
This project is provided for educational and assignment purposes.
