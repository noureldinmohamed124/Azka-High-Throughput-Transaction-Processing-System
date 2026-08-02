# Enterprise Transaction Processing System

A production-ready **ASP.NET Core 8 Web API** that simulates a high-throughput financial transaction processing platform capable of safely handling concurrent transaction requests while guaranteeing globally unique receipt numbers.

The project was developed as part of the **Software Engineering Internship Assessment** and follows enterprise software engineering practices including Clean Architecture, SOLID Principles, Repository Pattern, Dependency Injection, Entity Framework Core, and SQL Server.

---

# Features

- High-throughput transaction processing
- Thread-safe receipt number generation
- Database-backed pessimistic concurrency control
- ACID-compliant transaction persistence
- Transaction lifecycle management (Pending, Settled, Cancelled)
- Daily transaction reporting and business analytics
- Search transactions by receipt number, customer, and date
- Performance benchmarked with k6
- Global exception handling
- Unified API response contract
- JWT Authentication & Authorization
- Swagger API Documentation
- Entity Framework Core Migrations
- Unit Testing
- Production-oriented Clean Architecture

---

# Technology Stack

| Technology            | Version      |
| --------------------- | ------------ |
| .NET                  | 8            |
| ASP.NET Core Web API  | 8            |
| C#                    | 12           |
| Entity Framework Core | 8            |
| SQL Server            | 2022         |
| Swagger (Swashbuckle) | Latest       |
| JWT Authentication    | Bearer Token |
| xUnit                 | Latest       |

---

# Architecture Decisions

The following architectural decisions were intentionally made:

- Clean Architecture to separate business logic from infrastructure.
- Repository Pattern for data access abstraction.
- Unit of Work for transactional consistency.
- Database-backed pessimistic locking for receipt sequence generation.
- SQL projection for reporting queries to minimize memory allocations.
- Centralized exception handling middleware.
- Dependency Injection throughout the application.

---

# Architecture

The solution follows **Clean Architecture** to ensure maintainability, testability, and separation of concerns.

```
Presentation (API)
│
├── Controllers
├── Middlewares
├── DTOs
└── Commons
      ├── ApiResponse
      └── Base Api Controller

↓

Application
│
├── Abstractions
│        ├── Presistance
│        └── Services (IReceipteGenerator)
├── Common
│     ├── DTOs
│     ├── Extensions
│     └── Interfaces
├── Exceptions
└── Modules
      ├── Customers
      └── Transactions

↓

Domain
│
├── Entities
└── Enums

↓

Infrastructure
│
├── Persistence
│       ├── Configurations
│       └── DbContext
├── Repositories
├── Security
└── Services

↓

SQL Server
```

---

# Project Structure

```
src/

├── EnterpriseTransactionProcessing.API
├── EnterpriseTransactionProcessing.Application
├── EnterpriseTransactionProcessing.Domain
├── EnterpriseTransactionProcessing.Infrastructure

```

---

# Database Design

## Main Entities

- Customer
- Transaction
- Branch
- PaymentMethod
- ReceiptSequence

### Relationships

```
Customer
    │
    │ 1
    │
    ├──────────────┐
                   │ *
              Transaction
                   │
      ┌────────────┴────────────┐
      │                         │
      ▼                         ▼

Branch             │    PaymentMethod
                   │
                   │
                   ▼

           ReceiptSequence
```

---

# Receipt Number Format

Receipt numbers are generated using the following format:

```
PREFIX-YYYYMMDD-USERID-SEQUENCE
```

Example

```
PAY-20260730-15-000012
```

---

# Business Rules

The system implements the following core business rules:

- Receipt numbers are globally unique.
- Receipt sequences restart every day for each prefix.
- Failed transactions do not consume receipt numbers unless committed.
- Settled transactions cannot be modified.
- Reports display only committed transactions.
- All transaction operations execute atomically inside database transactions.

---

# Concurrent Receipt Number Generation

## Overview

One of the primary objectives of this project is to guarantee that every financial transaction receives a **globally unique receipt number**, even when multiple requests are processed concurrently.

To achieve this, the application combines SQL Server pessimistic locking, transactional consistency, database constraints, and automatic retry logic.

---

## Receipt Number Format

Each transaction is assigned a receipt number using the following format:

```text
PREFIX-YYYYMMDD-CUSTOMERID-SEQUENCE
```

Example:

```text
PAY-20260730-15-000021
```

Where:

| Segment      | Description                                               |
| ------------ | --------------------------------------------------------- |
| `PREFIX`     | Transaction type prefix (e.g. PAY).                       |
| `YYYYMMDD`   | UTC transaction date.                                     |
| `CUSTOMERID` | Identifier of the authenticated customer.                 |
| `SEQUENCE`   | Six-digit sequence number unique for the prefix and date. |

---

## Receipt Sequence Storage

Receipt sequences are maintained in a dedicated `ReceiptSequence` table.

Each record represents the latest generated sequence for a specific transaction prefix on a particular day.

| Column         | Purpose                                                  |
| -------------- | -------------------------------------------------------- |
| `Prefix`       | Transaction type prefix.                                 |
| `Date`         | UTC business date.                                       |
| `LastSequence` | Last generated sequence number for that prefix and date. |

This allows every transaction type to maintain its own independent daily sequence.

---

## Transaction Workflow

Receipt generation is executed within the same database transaction used to persist the financial transaction.

```text
Client
    │
    ▼
Controller
    │
    ▼
CreateTransactionUseCase
    │
    ▼
Validate Request
    │
    ▼
Begin Database Transaction
    │
    ▼
Acquire Pessimistic Lock (UPDLOCK, ROWLOCK)
    │
    ▼
Load Receipt Sequence
    │
    ▼
Increment Sequence
    │
    ▼
Generate Receipt Number
    │
    ▼
Create Transaction
    │
    ▼
SaveChanges()
    │
    ▼
Commit Transaction
    │
    ▼
Return Response
```

If any step fails before the transaction is committed, the entire operation is rolled back, ensuring that no partial data is persisted.

---

## Concurrency Strategy

The application uses **SQL Server pessimistic locking** to synchronize concurrent access to the receipt sequence.

During receipt generation, the corresponding sequence row is retrieved using:

```sql
SELECT *
FROM ReceiptSequences WITH (UPDLOCK, ROWLOCK)
WHERE Prefix = @Prefix
AND [Date] = @Date
```

This guarantees that only one transaction at a time can update the sequence for a specific prefix and business date.

As a result:

- Two concurrent requests cannot generate the same sequence number.
- Receipt numbers remain globally unique.
- Concurrent updates are serialized at the database level.

---

### Receipt Sequence Generation

Receipt numbers are generated using a dedicated `ReceiptSequence` table.

To guarantee uniqueness under heavy concurrent workloads, the implementation uses:

- SQL Server `UPDLOCK`
- SQL Server `ROWLOCK`
- Explicit Unit of Work transactions
- Retry mechanism for concurrent first-row creation
- Unique database constraint on `(Prefix, Date)`

This guarantees:

- No duplicate receipt numbers
- Daily sequence reset
- Thread-safe generation
- ACID-compliant persistence

---

## Handling First-Time Daily Requests

When the first transaction of a new day is processed, no sequence record exists for that prefix.

In this case, the application attempts to create a new sequence starting at `1`.

If multiple requests attempt to create the same sequence simultaneously, SQL Server's unique constraint prevents duplicate records.

The resulting constraint violation is detected and translated into a domain-specific exception.

The operation is then automatically retried using a fresh database transaction.

This retry mechanism allows only one request to successfully create the daily sequence while the remaining requests transparently continue using the newly created record.

---

## Automatic Retry Strategy

To handle transient concurrency conflicts during sequence initialization, the application retries receipt generation up to three times.

Each retry performs the entire workflow inside a brand-new database transaction.

If all retry attempts fail, a business exception is returned to the client.

This approach improves reliability without exposing transient database conflicts to API consumers.

---

## Transaction Consistency

Receipt generation and transaction creation are executed as a single atomic unit of work.

The following operations occur inside the same database transaction:

- Receipt sequence retrieval
- Sequence increment
- Receipt number generation
- Transaction creation
- Database persistence

Only after every operation succeeds is the transaction committed.

If any operation fails, the transaction is rolled back and the database remains unchanged.

This guarantees that receipt sequences and financial transactions always remain synchronized.

---

## Database-Level Guarantees

Application logic is reinforced by database constraints to provide defense in depth.

The database enforces:

- Unique receipt numbers
- Unique daily receipt sequence records per transaction prefix
- ACID-compliant transactions
- Referential integrity through foreign keys

These guarantees ensure that data integrity is preserved even under high levels of concurrency.

---

## Design Rationale

This strategy was chosen because it provides a balance between correctness, simplicity, and reliability.

Instead of relying solely on application logic, the design delegates concurrency control to SQL Server while using the application layer to coordinate transactions, retries, and business rules.

The result is a receipt generation mechanism that is deterministic, resilient under concurrent load, and suitable for transactional systems where uniqueness and consistency are critical requirements.

---

# Why This Design?

The primary technical challenge of this project was guaranteeing **globally unique receipt numbers** while supporting concurrent transaction processing.

Rather than focusing only on generating receipt numbers, the solution was designed to ensure **correctness**, **consistency**, and **reliability** under concurrent database access. The following sections explain the architectural decisions behind the implementation.

---

## Why Pessimistic Locking Instead of Optimistic Concurrency?

Receipt number generation is a highly contention-sensitive operation where multiple concurrent requests compete to update the same sequence.

Optimistic concurrency assumes conflicts are relatively uncommon and detects them only when changes are committed. While this approach works well for many business scenarios, receipt generation requires deterministic sequencing and cannot risk two requests calculating the same next sequence number before a conflict is detected.

For this reason, the application uses **pessimistic locking**, allowing SQL Server to serialize access to the sequence record. Only one transaction can increment the sequence at a time, eliminating race conditions before they occur.

This approach prioritizes correctness and consistency over maximum write concurrency, which is appropriate for financial transaction processing.

---

## Why SQL Server Locking Hints Instead of Application-Level Locks?

Concurrency is enforced directly by SQL Server using the `UPDLOCK` and `ROWLOCK` locking hints.

This approach was selected because the database is the authoritative source of truth for receipt sequences.

Using database locks provides several advantages:

- Synchronization works across all application instances.
- No shared in-memory state is required.
- Locking remains effective even in distributed deployments.
- The locking strategy is coordinated directly by SQL Server's transaction manager.

Application-level synchronization mechanisms (such as `lock`, `SemaphoreSlim`, or static objects) only protect a single application instance and cannot guarantee consistency when multiple servers or processes access the same database.

---

## Why an Automatic Retry Mechanism?

A unique concurrency scenario exists when the first transaction of a new day is processed.

At that moment, no `ReceiptSequence` record exists for the requested prefix and date. Multiple requests may attempt to create that record simultaneously.

Instead of failing the request, the application detects database constraint violations caused by concurrent sequence creation and automatically retries the operation using a new database transaction.

This retry strategy allows one request to successfully create the sequence while the remaining requests transparently continue using the newly created record.

As a result, transient concurrency conflicts are handled automatically without exposing implementation details to API consumers.

---

## Why Use Database Transactions?

Receipt generation and transaction creation represent a single business operation.

Separating these operations could leave the system in an inconsistent state if one succeeds while the other fails.

To prevent this, the application executes the following operations inside a single database transaction:

- Validate business references.
- Acquire the receipt sequence lock.
- Generate the next receipt number.
- Create the financial transaction.
- Persist all changes.
- Commit the transaction.

If any step fails, the transaction is rolled back, ensuring that neither the receipt sequence nor the financial transaction is partially committed.

This guarantees atomicity and maintains consistency across all related data.

---

## Why a Dedicated ReceiptSequence Table?

Instead of calculating the next sequence number by querying the `Transactions` table, the application maintains a dedicated `ReceiptSequence` table.

This design offers several advantages:

- Constant-time retrieval of the current sequence.
- No expensive aggregate queries (`MAX`) over historical transactions.
- Better scalability as transaction volume grows.
- Simpler concurrency management.
- Independent daily sequences for each transaction prefix.

The dedicated sequence table isolates the responsibility of sequence management, resulting in a cleaner and more maintainable design.

---

## Why Use Unique Constraints in Addition to Application Logic?

Although the application prevents duplicate receipt numbers through pessimistic locking, database constraints provide an additional layer of protection.

The database enforces uniqueness independently of the application, ensuring that invalid data cannot be persisted even if an unexpected application defect or concurrency issue occurs.

This "defense in depth" strategy combines application-level safeguards with database-level guarantees to maximize data integrity.

---

## Why a Dedicated Receipt Generator Service?

Receipt generation is implemented as a dedicated service rather than being embedded inside the transaction use case.

This separation provides several benefits:

- Single responsibility for receipt generation.
- Reusable logic across multiple transaction workflows.
- Easier unit testing.
- Clear separation between business workflow orchestration and receipt generation logic.
- Improved maintainability if receipt generation rules evolve in the future.

The transaction use case coordinates the overall business process, while the receipt generator focuses exclusively on generating valid receipt numbers.

---

## Why a Lightweight CQRS Implementation?

The project follows the CQRS architectural style by separating commands and queries into independent feature modules.

Each feature contains its own request models, response models, and use case implementation.

Instead of introducing MediatR, handlers, and pipeline behaviors, the project adopts a lightweight implementation that preserves the core principles of CQRS while avoiding additional complexity.

This approach keeps the architecture easy to understand, reduces unnecessary abstractions, and remains appropriate for the size and scope of the assessment project.

---

# Performance Testing

The solution includes load testing using **k6**.

The benchmark validates:

- Concurrent transaction creation
- Receipt number uniqueness
- Average response time
- P95 latency
- Error rate
- Throughput

Results are available in:

docs/Performance/Performance Benchmark Report.md

---

# Performance Optimizations

Several optimizations were implemented to satisfy the performance requirements:

- Async/Await throughout the application
- SQL projections for read-only queries
- Server-side aggregation for reports
- Database indexing
- Minimal EF Core tracking for read operations
- Database-backed pessimistic locking for receipt generation
- Repository Pattern with Unit of Work
- Optimized LINQ queries

---

---

## Overall Design Philosophy

The architecture was designed around a simple principle:

> **Critical business rules should be enforced as close to the data as possible, while business workflows remain clean, modular, and easy to maintain.**

Rather than relying solely on application logic, the solution combines SQL Server locking, database transactions, unique constraints, custom exceptions, and modular use cases to ensure correctness under concurrent load.

This layered approach provides strong consistency guarantees while keeping the implementation readable, testable, and aligned with enterprise software engineering practices.

---

# API Documentation

Swagger UI is available after running the application.

```
https://localhost:5001/swagger
```

The API supports JWT Bearer Authentication.

Click the **Authorize** button inside Swagger and provide a valid JWT access token to access protected endpoints.

---

# Authentication

The API uses JWT Bearer Authentication.

Authentication flow:

```
Login

↓

JWT Token

↓

Authorize Button (Swagger)

↓

Protected Endpoints
```

---

# Error Handling

The application implements a centralized exception handling mechanism through a custom **Global Exception Middleware**. All unhandled exceptions are intercepted in a single place, ensuring that API consumers always receive a consistent response format.

Business and application errors are represented using strongly typed custom exceptions derived from a common `AppException` base class. Each exception defines its corresponding HTTP status code, allowing the middleware to automatically translate domain errors into the appropriate HTTP responses.

Implemented exception types include:

| Exception               |      HTTP Status |
| ----------------------- | ---------------: |
| `BadRequestException`   |  400 Bad Request |
| `ValidationException`   |  400 Bad Request |
| `UnauthorizedException` | 401 Unauthorized |
| `ForbiddenException`    |    403 Forbidden |
| `NotFoundException`     |    404 Not Found |
| `ConflictException`     |     409 Conflict |
| `BusinessRuleException` |     409 Conflict |

Unexpected exceptions are logged and returned as **500 Internal Server Error** responses without exposing internal implementation details.

### Example Error Response

```json
{
  "success": false,
  "message": "Customer not found.",
  "data": null,
  "errors": null
}
```

### Validation Error Example

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": [
    "Amount must be greater than zero.",
    "Transaction amount exceeds the allowed limit."
  ]
}
```

---

# API Response Contract

All endpoints return a unified response structure to provide a consistent experience for API consumers.

## Successful Response

```json
{
  "success": true,
  "message": "Transaction created successfully.",
  "data": {
    "transactionId": 15,
    "receiptNumber": "PAY-20260730-15-000021"
  },
  "errors": null
}
```

## Error Response

```json
{
  "success": false,
  "message": "Branch not found.",
  "data": null,
  "errors": null
}
```

The response contract consists of the following properties:

| Property  | Description                                                  |
| --------- | ------------------------------------------------------------ |
| `success` | Indicates whether the request completed successfully.        |
| `message` | Human-readable message describing the operation result.      |
| `data`    | Contains the requested resource or operation result.         |
| `errors`  | Collection of validation or business errors when applicable. |

Controllers inherit from a shared `BaseApiController`, which provides helper methods for generating standardized responses (`Ok`, `Created`, and `NoContent`). This ensures all endpoints return a consistent API contract throughout the application.

---

# Performance Considerations

The application has been designed with scalability and reliability in mind while remaining simple and maintainable.

Implemented optimizations include:

- Asynchronous database operations using `async` / `await`
- Pessimistic locking to guarantee safe concurrent receipt number generation
- Database transactions to ensure atomic operations
- Entity Framework Core query projections where appropriate
- Repository Pattern to separate persistence concerns
- Unit of Work to execute related operations as a single transaction
- Dependency Injection throughout the application
- Feature-based modular organization for improved maintainability
- Centralized exception handling to reduce duplicated error-handling logic

---

# Architectural Decisions

| Decision                                | Reason                                                                                                                   |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Clean Architecture + Onion Architecture | Keeps business logic independent from infrastructure while maintaining a clear dependency flow toward the domain layer.  |
| Feature-Based Modular Structure         | Organizes related commands, queries, DTOs, and use cases together, making features easier to develop and maintain.       |
| CQRS Style (without MediatR)            | Separates commands from queries without introducing unnecessary complexity or additional dependencies.                   |
| Use Cases                               | Encapsulate business logic for each feature, keeping controllers lightweight and focused on HTTP concerns.               |
| Repository Pattern                      | Abstracts data access, improves maintainability, and simplifies testing.                                                 |
| Unit of Work                            | Coordinates multiple repository operations within a single database transaction.                                         |
| Service Layer                           | Encapsulates reusable infrastructure services such as receipt number generation.                                         |
| Entity Framework Core                   | Provides a modern ORM with excellent integration into ASP.NET Core and SQL Server.                                       |
| SQL Server                              | Ensures reliable ACID-compliant transactional storage suitable for financial operations.                                 |
| Pessimistic Locking                     | Prevents duplicate receipt numbers during concurrent transaction processing by synchronizing access to shared resources. |
| Custom Exceptions                       | Provides expressive domain-specific error handling while keeping business logic clean.                                   |
| Global Exception Middleware             | Centralizes exception handling and guarantees consistent API responses.                                                  |
| Unified API Response                    | Ensures all endpoints return a predictable response contract for easier client integration.                              |
| JWT Authentication                      | Relies on externally issued JWT tokens while extracting the authenticated customer's identity from claims.               |

---

# Running the Project

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or later

---

## Clone the Repository

```bash
git clone https://github.com/your-username/EnterpriseTransactionProcessing.git
```

---

## Configure Application Settings

Update the following configuration values inside `appsettings.json`:

- SQL Server connection string
- JWT Secret Key
- JWT Issuer
- JWT Audience

---

## Database

Default database name:

```
Azka_TPS_DB
```

Apply the database migrations:

```powershell
Update-Database
```

Or create the initial migration if needed:

```powershell
Add-Migration InitialCreate -OutputDir Persistence/Migrations
```

---

## Run the Application

```powershell
dotnet run
```

---

## Swagger

Once the application starts, navigate to:

```
https://localhost:{port}/swagger
```

Use the **Authorize** button to provide a valid JWT access token before accessing protected endpoints.

---

# Future Improvements

- Idempotent transaction requests
- In-memory caching for reference data
- Audit logging
- Distributed caching (Redis)
- OpenTelemetry tracing
- Background jobs
- Docker support
- CI/CD pipeline

---

# Authors

**Nour El-Din Mohamed**

Backend Software Engineer
ASP.NET Core • .NET 8 • EF Core • SQL Server • Clean Architecture • System Design • System Architecture • SOLID Principles

**Omar Youssef**

FullStack Developer
ASP.NET Core • .NET 8 • EF Core • SQL Server • Clean Architecture • React.js • Tailwind.css
