# TPS API Endpoints

This document defines the API contract for the **Transaction Processing System (TPS)**. It serves as a development checklist to ensure all required endpoints are implemented according to the assignment requirements.

---

# 1. Transactions Module

This is the core module of the system.

---

## Create Transaction

**Endpoint**

```http
POST /api/transactions
```

**Description**

Creates a new transaction, generates a unique receipt number, stores the transaction, and returns the created transaction details.

**Priority**

🟢 Required

---

## Get Transaction by Receipt Number

**Endpoint**

```http
GET /api/transactions/receipt/{receiptNumber}
```

**Example**

```http
GET /api/transactions/receipt/PAY-20260730-15-000025
```

**Description**

Returns a transaction using its unique receipt number.

**Priority**

🟢 Required

**Requirement Covered**

- Search by receipt number.

---

## Search Transactions

**Endpoint**

```http
GET /api/transactions
```

**Query Parameters**

| Parameter | Type | Required |
|-----------|------|----------|
| customerId | int | No |
| date | DateOnly | No |

**Examples**

Search by customer

```http
GET /api/transactions?customerId=15
```

Search by date

```http
GET /api/transactions?date=2026-07-30
```

Search by customer and date

```http
GET /api/transactions?customerId=15&date=2026-07-30
```

**Description**

Allows searching transactions using one or more filters.

**Requirement Covered**

- Search by customer.
- Search by date.

---

## Daily Transaction Summary

**Endpoint**

```http
GET /api/transactions/daily-summary
```

**Query Parameters**

| Parameter | Type | Required |
|-----------|------|----------|
| date | DateOnly | Yes |

**Example**

```http
GET /api/transactions/daily-summary?date=2026-07-30
```

**Example Response**

```json
{
  "date": "2026-07-30",
  "totalTransactions": 120,
  "totalAmount": 15420.50,
  "successfulTransactions": 110,
  "failedTransactions": 10
}
```

**Description**

Returns aggregated transaction statistics for a single day.

**Requirement Covered**

- Produce daily transaction summaries.

---

# 2. Customers Module

Transactions reference customers, therefore customer lookup endpoints are useful.

---

## Get Customer

**Endpoint**

```http
GET /api/customers/{id}
```

Returns a customer by Id.

---

## Get All Customers

**Endpoint**

```http
GET /api/customers
```

Returns all customers.

---

## Create Customer *(Optional)*

**Endpoint**

```http
POST /api/customers
```

Create a new customer.

> This endpoint is optional if customer data will be seeded.

---

# 3. Branches Module

---

## Get All Branches

**Endpoint**

```http
GET /api/branches
```

Returns all branches.

---

## Get Branch

**Endpoint**

```http
GET /api/branches/{id}
```

Returns a branch by Id.

---

## Create Branch *(Optional)*

**Endpoint**

```http
POST /api/branches
```

Creates a new branch.

> Optional if branches are seeded.

---

# 4. Payment Methods Module

---

## Get All Payment Methods

**Endpoint**

```http
GET /api/payment-methods
```

Returns all supported payment methods.

---

## Get Payment Method

**Endpoint**

```http
GET /api/payment-methods/{id}
```

Returns a payment method by Id.

---

## Create Payment Method *(Optional)*

**Endpoint**

```http
POST /api/payment-methods
```

Creates a new payment method.

> Optional if payment methods are seeded.

---

# Assignment Requirements Mapping

| Functional Requirement | Endpoint / Feature |
|-------------------------|-------------------|
| Process concurrent transactions | `POST /api/transactions` |
| Generate receipt number | `POST /api/transactions` |
| Ensure sequence uniqueness | Receipt generation service |
| Store transaction, customer, payment method and branch | `POST /api/transactions` |
| Search by receipt | `GET /api/transactions/receipt/{receiptNumber}` |
| Search by customer | `GET /api/transactions?customerId=...` |
| Search by date | `GET /api/transactions?date=...` |
| Produce daily transaction summaries | `GET /api/transactions/daily-summary` |
| Support joins with customer, payment method and branch | Search and report queries |

---

# What "Support Joins" Means

This is **not** a separate endpoint.

It means that when retrieving transaction data, the application should include related information from:

- Customer
- Branch
- Payment Method

For example, a transaction search response could be:

```json
{
  "receiptNumber": "PAY-20260730-15-000001",
  "customer": {
    "id": 15,
    "name": "Ahmed Ali"
  },
  "branch": {
    "id": 2,
    "name": "Cairo"
  },
  "paymentMethod": {
    "id": 1,
    "name": "Visa"
  },
  "amount": 500.00
}
```

This can be implemented using:

- Entity Framework Core `Include(...)`
- Projection using `Select(...)`
- SQL JOIN queries

---

# Recommended Scope

For this assessment, the recommended API surface consists of the following endpoints:

| Endpoint | Purpose | Priority |
|----------|---------|----------|
| `POST /api/transactions` | Create transaction and generate receipt | ⭐⭐⭐⭐⭐ |
| `GET /api/transactions/receipt/{receiptNumber}` | Find transaction by receipt number | ⭐⭐⭐⭐⭐ |
| `GET /api/transactions` | Search transactions | ⭐⭐⭐⭐⭐ |
| `GET /api/transactions/daily-summary` | Daily transaction report | ⭐⭐⭐⭐⭐ |
| `GET /api/payment-methods` | Retrieve available payment methods | ⭐⭐⭐⭐ |

Customers and Branches can be seeded into the database unless CRUD operations are explicitly required by the project specifications.
