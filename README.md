# Hotel Management API

A robust, enterprise-grade Hotel Management RESTful API built with **.NET 9**, **PostgreSQL**, and modern architectural patterns. This system handles everything from room availability and reservations to payment processing, user authentication, and advanced reporting.

---

## 🏗 Architecture & Design Patterns

This project is built using **Clean Architecture** principles to ensure a decoupled, testable, and maintainable codebase.

- **CQRS (Command Query Responsibility Segregation)**: Read and write operations are strictly separated using **MediatR**. Commands modify state (e.g., booking a room, processing payments), while Queries simply retrieve data (e.g., fetching available rooms, reporting stats).
- **REPR Pattern (Request-Endpoint-Response)**: Instead of bloated, traditional MVC Controllers, this API uses **FastEndpoints** to logically group endpoints by their specific features, keeping the application highly cohesive.
- **Repository Pattern**: Abstracted database access logic using a Generic Repository to handle operations with Entity Framework Core seamlessly.
- **Feature-Driven Structure (Hybrid Vertical Slicing)**: The application logic is organized vertically by features (`RoomManagement`, `ReservationManagement`, `PaymentManagement`, etc.) rather than by technical layers (e.g. Controllers, Services). This high-cohesion approach places endpoints, handlers, and validation together. It operates as a "Hybrid" approach, utilizing shared `Domain` and `Infrastructure` layers to simplify shared data mappings (Entity Framework Core) while slicing the actual request handling logic.

---

## 🛠 Technologies & Packages Used

- **Framework**: [.NET 9 (ASP.NET Core)](https://dotnet.microsoft.com/)
- **Database / ORM**: [PostgreSQL](https://www.postgresql.org/) via [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) with **Npgsql**
- **Endpoint Routing**: [FastEndpoints](https://fast-endpoints.com/) (Minimal APIs alternative)
- **Mediator Implementation**: [MediatR](https://github.com/jbogard/MediatR)
- **Data Validation**: [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- **Authentication**: JWT (JSON Web Tokens) with `Microsoft.AspNetCore.Authentication.JwtBearer` & `BCrypt.Net-Next` for password hashing
- **Object Mapping**: [AutoMapper](https://automapper.org/)
- **Background Jobs**: [Hangfire](https://www.hangfire.io/)
- **API Documentation**: Swagger / OpenAPI integration (Swashbuckle)

---

## 🚀 Key Features

### 1. 🏨 Room & Inventory Management
- Create, update, and fetch detailed room listings.
- Track room facilities and current availability status.

### 2. 📅 Reservation System
- Search for available rooms by date boundaries.
- Book reservations, update dates, and handle booking cancellations.

### 3. 💳 Payment Processing & Invoices
- Simulate payment processing against active reservations.
- Automatic generation of uniquely numbered invoices linked to guests and payments.

### 4. 🔒 User Management (Authentication & RBAC)
- Secure Guest & Staff registrations and logins.
- Token-based JWT authentication.
- Role-Based Access Control allowing specific endpoints to only be accessed by Guests, Staff, or Admins.

### 5. 📊 Reporting & Analytics
- **Occupancy Analytics**: Real-time evaluation of hotel occupancy percentages.
- **Revenue & Booking Reports**: Aggregated sums of pending, completed, and refunded revenue over specified date ranges.
- **Customer Analytics**: Track top-spending customers and booking behaviors.

### 6. ⭐ Customer Feedback (Reviews)
- Guests can leave 1-5 star ratings and written reviews for specific rooms they have stayed in.
- Staff members can issue official responses to reviews.

---

## 💻 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) server running locally or remotely

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/HotelManagement.git
   cd HotelManagement/HotelManagement
   ```

2. **Configure Database & Secrets:**
   Open `appsettings.json` and ensure your `ConnectionStrings:DefaultConnection` correctly points to your PostgreSQL instance. 
   *(Note: Remember to supply a secure value for your JWT `Key` before deploying to production!)*

3. **Apply Database Migrations:**
   Ensure the Entity Framework Core CLI tools are installed, then run:
   ```bash
   dotnet ef database update
   ```

4. **Run the API:**
   ```bash
   dotnet run
   ```

5. **Explore the Endpoints:**
   Open your browser and navigate to `https://localhost:<port>/swagger` to view and interact with the Auto-Generated Swagger UI documentation covering all available endpoints.

---

## 📖 Project Structure Highlight

```
HotelManagement/
├── Domain/                   # Enterprise Data Models & Enums
├── Infrastructure/
│   ├── Data/                 # ApplicationDbContext & Repositories
│   ├── Configuration/        # Entity Framework Fluent API configs
│   └── Migrations/           # EF Core Database Migrations
├── Features/                 # Vertical Slice Architecture
│   ├── AuthManagement/
│   ├── Common/               # Shared logic, base FastEndpoints, standardized results
│   ├── FeedbackManagement/
│   ├── PaymentManagement/
│   ├── ReportingManagement/
│   ├── ReservationManagement/
│   └── RoomManagement/
└── Program.cs                # Entry Point, DI Registrations, Middleware config
```