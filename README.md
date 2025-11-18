# Library Management - Minimal API (.NET 7)

This repository contains a **minimal** Library Management API with:

- JWT Authentication (register/login)
- External endpoints:
  - `GET /api/external/bookinfo/{isbn}` — fetches book info from external API (OpenLibrary) and caches it
  - `GET /api/external/logs` — shows saved external-call logs

Tech:
- .NET 7
- EF Core (SQLite)
- JWT Bearer Authentication
- In-memory caching (IMemoryCache)
- Swagger

## Quick start

1. Clone the repo and navigate to project folder.

2. Restore and run:
```bash
dotnet restore
dotnet run
```

3. The API will run (HTTPS). Open Swagger at `https://localhost:5001/swagger` (development).

## Database & Migrations
This minimal project uses EF Core + SQLite and will create `library_management.db` automatically on first run.
If you want migrations:
```
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Authentication
- Register: `POST /api/auth/register` with JSON `{ "username", "email", "password" }`
- Login: `POST /api/auth/login` with `{ "username", "password" }`
- Copy JWT from login response and set `Authorization: Bearer <token>` for protected endpoints.

## External API
`GET /api/external/bookinfo/{isbn}` will:
- Check cache
- If miss, call OpenLibrary API, save `ExternalBookInfo` and log the call (`ExternalApiLog`)
- Cache the response for 30 minutes

## Postman
Import `PostmanCollection.json` included at project root.

