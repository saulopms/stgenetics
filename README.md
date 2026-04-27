# Good Hamburger

REST API + Blazor Server frontend for a hamburger restaurant order management system.

---

## Quick Start

You need **.NET 10 SDK** installed. Run both projects simultaneously in separate terminals:

```bash
# Terminal 1 — REST API (http://localhost:5022)
cd src/GoodHamburger.API
dotnet run

# Terminal 2 — Blazor frontend (http://localhost:5106)
cd src/GoodHamburger.Web
dotnet run
```

Once both are running:

| URL | Description |
|-----|-------------|
| `http://localhost:5022` | Swagger UI (explore & test the API) |
| `http://localhost:5106` | Blazor Server frontend |

---

## Credentials

> **Security note:** Credentials are stored in plain text inside `appsettings.json`. This is intentional for demo convenience — in a real system they would be hashed and stored in a database (or managed via an identity provider).

| Username | Password |
|----------|----------|
| `admin`  | `admin123` |

Use these credentials in Swagger's **Authorize** dialog or on the Blazor login page.

---

## Running the Tests

```bash
# Unit tests (22 tests)
dotnet test tests/GoodHamburger.Tests.Unit

# Integration tests (18 tests — spins up an in-process test server)
dotnet test tests/GoodHamburger.Tests.Integration

# All tests at once
dotnet test
```

All 40 tests pass with no external dependencies required.

---

## Project Structure

```
src/
  GoodHamburger.Domain/          Domain entities, enums, exceptions, interfaces
  GoodHamburger.Application/     Use cases, DTOs, service interfaces and implementations
  GoodHamburger.Infrastructure/  In-memory repository, static menu catalog, DI wiring
  GoodHamburger.API/             ASP.NET Core controllers, JWT auth, Swagger, middleware
  GoodHamburger.Web/             Blazor Server frontend (consumes the REST API)

tests/
  GoodHamburger.Tests.Unit/        Domain + Application layer unit tests (xUnit + Moq)
  GoodHamburger.Tests.Integration/ Full HTTP integration tests (WebApplicationFactory)
```

### Layer responsibilities

| Layer | Depends on | Responsibility |
|-------|-----------|---------------|
| Domain | nothing | Entities (`Order`, `MenuItem`, `OrderItem`), `DiscountCalculator`, domain exceptions |
| Application | Domain | `IOrderService`, `IMenuService`, DTOs, validation logic |
| Infrastructure | Domain, Application | `InMemoryOrderRepository`, `MenuCatalog`, `AddInfrastructureServices()` |
| API | Application, Infrastructure | Controllers, JWT setup, Swagger, exception middleware, `AddApplicationServices()` |
| Web | — | Blazor pages, `OrderApiClient`, `MenuApiClient`, auth state provider |

Dependency injection is registered in extension methods per layer (`AddApplicationServices()`, `AddInfrastructureServices()`), keeping `Program.cs` free of direct service registrations.

---

## Business Rules

The menu contains exactly **5 items**:

| Id | Name | Category | Price |
|----|------|----------|-------|
| 1 | X Burger | Sandwich | R$ 5.00 |
| 2 | X Egg | Sandwich | R$ 7.50 |
| 3 | X Bacon | Sandwich | R$ 10.00 |
| 4 | Fries | Fries | R$ 3.99 |
| 5 | Soda | Soda | R$ 2.99 |

**Order constraints** (validated on create and update):
- Each order may contain **at most one item per category** (one sandwich, one fries, one soda).
- Duplicate item IDs in the same request are rejected.
- Only IDs 1–5 are valid.

**Automatic discount rules**:

| Combination | Discount |
|-------------|----------|
| Sandwich + Fries + Soda | 20 % |
| Sandwich + Soda | 15 % |
| Sandwich + Fries | 10 % |
| Anything else | 0 % |

---

## API Endpoints

All endpoints require a **Bearer JWT token** (obtain from `POST /api/auth/login`).

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/auth/login` | Authenticate and receive a JWT token |
| GET | `/api/menu` | List all 5 menu items |
| GET | `/api/orders` | List all orders |
| POST | `/api/orders` | Create a new order |
| GET | `/api/orders/{id}` | Get a single order |
| PUT | `/api/orders/{id}` | Replace an order's items (recalculates discount) |
| DELETE | `/api/orders/{id}` | Delete an order |

Error responses follow **RFC 7807 Problem Details** (`application/problem+json`).

---

## Architecture Decisions

### In-memory persistence
`InMemoryOrderRepository` uses `ConcurrentDictionary<Guid, Order>` — thread-safe and zero-config. All data is lost on restart. Swapping to a database requires only a new `IOrderRepository` implementation; no application or domain code changes.

### Static menu catalog
The menu is fixed by the challenge spec, so it lives as a read-only list in `MenuCatalog` (a singleton). There is no menu management UI or API.

### JWT with hardcoded users
`appsettings.json` holds a `Users` array validated at login time. Tokens expire in 60 minutes and are signed with a 32-character secret. See the security note above.

### Blazor Server + typed HttpClients
The frontend uses Blazor Server (no WASM download latency). Three typed clients (`AuthApiClient`, `MenuApiClient`, `OrderApiClient`) share a base URL from `ApiSettings:BaseUrl`. A `DelegatingHandler` (`AuthHttpMessageHandler`) injects the Bearer token on every request after login.

### CORS
The API allows requests from `http://localhost:5106` (Blazor dev address) and `https://localhost:5003`. Adjust `AllowedOrigins:Blazor` in `appsettings.json` for other environments.
