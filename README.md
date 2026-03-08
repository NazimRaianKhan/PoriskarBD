# Smart Waste Management System

A backend REST API for managing city waste collection built with **.NET 10**, **C#**, **Entity Framework Core**, and **SQL Server**. Citizens can report garbage problems, admins assign collectors, and collectors mark them as done — all tracked digitally with JWT-based role authentication.

---

## Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Database Design](#-database-design)
- [API Endpoints](#-api-endpoints)
- [Getting Started](#-getting-started)
- [Testing with Postman](#-testing-with-postman)
- [Deployment](#-deployment)

---

## Features

- **JWT Authentication** with role-based access control
- **Three user roles** — Citizen, Collector, Admin
- **Waste report submission** by citizens with location and description
- **Report tracking** with status flow: `Reported → Assigned → Collected`
- **Collector assignment** by admin to reported issues
- **Collection status updates** by collectors after completing tasks
- **Zone-based management** to organize areas of the city
- **Collection schedule management** per zone and day
- **Admin dashboard** with system-wide stats and zone summaries
- **Logout with token blacklisting** so invalidated tokens can't be reused
- **Digital record keeping** with full collection logs

---

## Tech Stack

| Technology | Purpose |
|---|---|
| .NET 10 | Main framework |
| C# | Programming language |
| ASP.NET Core Web API | HTTP routing and controllers |
| Entity Framework Core 10 | ORM — talks to the database |
| SQL Server | Database |
| JWT (JSON Web Tokens) | Authentication |
| BCrypt.Net | Password hashing |
| Swashbuckle / Swagger UI | Interactive API docs |

### NuGet Packages

| Package | Version |
|---|---|
| BCrypt.Net-Next | 4.1.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.3 |
| Microsoft.AspNetCore.OpenApi | 10.0.3 |
| Microsoft.EntityFrameworkCore | 10.0.3 |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.3 |
| Microsoft.EntityFrameworkCore.Tools | 10.0.3 |
| Swashbuckle.AspNetCore | 10.1.4 |
| System.IdentityModel.Tokens.Jwt | 8.16.0 |

---

## Project Structure

```
PoriskarBD/
│
├── Controllers/						# Thin — receives request, calls service, returns response
│   ├── AuthController.cs				# login, register, logout
│   ├── CollectionLogsController.cs     # logs, admin stats
│   ├── SchedulesController.cs          # schedules 
│   ├── UsersController.cs				# user management
│   ├── WasteReportsController.cs		# submit, assign, collect reports
│   └── ZonesControllers.cs				# zone management
│
├── Services/							# All business logic lives here
│   ├── AuthService.cs
│   ├── CollectionLogAndAdminService.cs
│   ├── ScheduleService.cs
│   ├── UserService.cs
│   ├── WasteReportService.cs
│   ├── ZoneService.cs            
│
├── Interfaces/                     # Contracts that services implement
│   └── IServices.cs                # IAuthService, IUserService, IZoneService, etc.
│
├── Models/                         # Entity classes (one class = one DB table)
│   └── Models.cs
│
├── DTOs/                           # What data goes in and out of the API
│   └── DTOs.cs
│
├── Data/                           # Database configuration
│   ├── AppDbContext.cs             # EF Core setup and table relationships
│   └── DbSeeder.cs                 # Seeds default admin on first run
│
├── Helpers/
│   └── JwtHelper.cs                # Creates and reads JWT tokens
│
├── Program.cs                      # App startup, DI registrations, middleware
├── appsettings.json                # Connection string and JWT config
└── PoriskarBD.csproj				# Package references
```

---

## 🗄Database Design

### Tables

| Table | Description |
|---|---|
| Users | All users (Citizens, Collectors, Admins) |
| Zones | Geographic zones/areas of the city |
| WasteReports | Garbage complaints submitted by citizens |
| CollectionSchedules | When garbage is collected in each zone |
| CollectionLogs | Record of completed collections |
| BlacklistedTokens | Invalidated JWT tokens (for logout) |

### Report Status Flow

```
Reported  ──(Admin assigns collector)──►  Assigned  ──(Collector collects)──►  Collected
```

### Role Values

| Value | Role |
|---|---|
| 0 | Citizen |
| 1 | Collector |
| 2 | Admin |

---

## API Endpoints

### Auth
| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Create a new account |
| POST | `/api/auth/login` | Public | Login and get JWT token |
| POST | `/api/auth/logout` | Any | Invalidate current token |

### Users
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/users` | Admin | Get all users (optional `?role=Citizen`) |
| GET | `/api/users/collectors` | Admin | Get all collectors |
| GET | `/api/users/me` | Any | Get your own profile |
| GET | `/api/users/{id}` | Admin | Get user by ID |
| DELETE | `/api/users/{id}` | Admin | Delete a user |

### Zones
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/zones` | Any | Get all zones |
| GET | `/api/zones/{id}` | Any | Get zone by ID |
| POST | `/api/zones` | Admin | Create a zone |
| PUT | `/api/zones/{id}` | Admin | Update a zone |
| DELETE | `/api/zones/{id}` | Admin | Delete a zone |

### Waste Reports
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/wastereports` | Any* | Get reports (filtered by role) |
| GET | `/api/wastereports/{id}` | Any* | Get one report |
| POST | `/api/wastereports` | Citizen | Submit a garbage report |
| PATCH | `/api/wastereports/{id}/assign` | Admin | Assign a collector |
| PATCH | `/api/wastereports/{id}/collect` | Collector | Mark as collected |
| GET | `/api/wastereports/filter?status=X` | Admin | Filter by status |

> *Citizens see only their own reports. Collectors see only their assigned reports. Admins see all.

### Schedules
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/schedules` | Any | Get all schedules |
| GET | `/api/schedules/zone/{id}` | Any | Get schedules by zone |
| POST | `/api/schedules` | Admin | Create a schedule |
| PUT | `/api/schedules/{id}` | Admin | Update a schedule |
| DELETE | `/api/schedules/{id}` | Admin | Delete a schedule |

### Collection Logs
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/collectionlogs` | Admin | Get all logs |
| GET | `/api/collectionlogs/collector/{id}` | Admin, Collector | Get logs by collector |

### Admin Dashboard
| Method | Endpoint | Access | Description |
|---|---|---|---|
| GET | `/api/admin/stats` | Admin | System-wide statistics |
| GET | `/api/admin/zone-summary` | Admin | Report summary per zone |

---

## Getting Started

### Prerequisites

- [Visual Studio 2022+](https://visualstudio.microsoft.com/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express is free)
- [SSMS](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (SQL Server Management Studio)

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/your-username/PoriskarBD.git
cd PoriskarBD
```

**2. Update the connection string**

Open `appsettings.json` and set your SQL Server name:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PoriskarBD;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
make sure u open microsoft sql server studio with windows authentication and your server name is correct.	
> Your server name is shown in the SSMS connection dialog (e.g. `localhost`, `.\SQLEXPRESS`, `DESKTOP-ABC\SQLEXPRESS`)

**3. Apply migrations**

Right click on your project name and click open in terminal to open terminal. First you have to make sure you have dotnet ef cli installed. If not, run:
```	
dotnet tool install --global dotnet-ef

```
If Installed then write the following commands in terminal to create database and tables:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**4. Run the project**

Press `F5` in Visual Studio. Swagger UI will open automatically at the root URL.

### Default Admin Account

The admin account is created automatically on first run:

```
Email:    admin@waste.com
Password: Admin@123
```

---

## Testing with Postman

### How to use JWT in Postman
1. Call the login endpoint and copy the `token` from the response
2. In Postman, go to the **Authorization** tab
3. Set Type to **Bearer Token**
4. Paste your token

### Quick Test Flow

```
1. POST /api/auth/login           → login as admin, save token
2. POST /api/zones                → create a zone (admin token)
3. POST /api/auth/register        → register a citizen (role: 0)
4. POST /api/auth/register        → register a collector (role: 1)
5. POST /api/auth/login           → login as citizen, save token
6. POST /api/wastereports         → citizen submits a report
7. PATCH /api/wastereports/1/assign → admin assigns collector
8. POST /api/auth/login           → login as collector, save token
9. PATCH /api/wastereports/1/collect → collector marks as collected
10. GET /api/admin/stats          → admin views stats
11. POST /api/auth/logout         → logout (invalidates token)
```

### Sample Request Bodies

**Register**
```json
{
  "name": "Rahim Khan",
  "email": "rahim@gmail.com",
  "password": "Rahim@123",
  "role": 0,
  "zoneId": 1
}
```

**Submit a Report**
```json
{
  "title": "Overflowing Bin",
  "description": "The garbage bin near Road 5 is overflowing",
  "location": "Road 5, Dhanmondi"
}
```

**Assign Collector**
```json
{
  "collectorId": 2
}
```

**Create Schedule**
```json
{
  "zoneId": 1,
  "dayOfWeek": 0,
  "timeSlot": "8:00 AM - 10:00 AM"
}
```
> `dayOfWeek`: 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday

---

## Deployment

**Not Deployed Yet** — This project is currently intended for local development and testing.

---

## Role Permission Summary

| Endpoint | Citizen | Collector | Admin |
|---|---|---|---|
| Submit report | ✅ | ❌ | ❌ |
| View own reports | ✅ | ❌ | ❌ |
| View assigned reports | ❌ | ✅ | ❌ |
| View all reports | ❌ | ❌ | ✅ |
| Assign collector | ❌ | ❌ | ✅ |
| Mark as collected | ❌ | ✅ | ❌ |
| Manage zones | ❌ | ❌ | ✅ |
| Manage schedules | ❌ | ❌ | ✅ |
| View schedules | ✅ | ✅ | ✅ |
| View stats | ❌ | ❌ | ✅ |
| Logout | ✅ | ✅ | ✅ |