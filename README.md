# Inventory Management API

A RESTful API for managing inventory operations including products, warehouses, stock, and orders. Built with .NET 9 and deployed with Docker.

## Features

- **Clean Architecture** - Separation of concerns with layered design
- **JWT Authentication** - Secure user registration and login
- **RESTful API** - Full CRUD operations for all resources
- **Docker Deployment** - Containerized API and SQL Server database
- **Unit Tests** - 21 comprehensive tests with 100% pass rate
- **Global Exception Handling** - Consistent error responses
- **Business Logic Validation** - Stock transfer validation, duplicate prevention

## Tech Stack

- **.NET 9** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server 2022** - Relational database
- **Docker & Docker Compose** - Containerization
- **xUnit & Moq** - Unit testing framework
- **BCrypt.NET** - Password hashing
- **JWT Bearer Tokens** - Authentication

## Architecture

```
├── Inventory.Api/              # API Controllers, DTOs, Middleware
├── Inventory.Core/             # Domain Entities (Product, Warehouse, Stock, User)
├── Inventory.Infrastructure/   # EF Core DbContext, Repositories
├── Inventory.Services/         # Business Logic Services
├── Inventory.Tests/            # Unit Tests
└── docker-compose.yml          # Docker orchestration
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Git

### Running with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/bobaram/invetory-assignment.git
   cd invetory-assignment
   ```

2. **Start the containers**
   ```bash
   docker-compose up -d
   ```

3. **Verify containers are running**
   ```bash
   docker ps
   ```

4. **API is now available at:** `http://localhost:8080`

### Running Locally (Without Docker)

1. **Update connection string** in `Inventory.Api/appsettings.json`

2. **Run the API**
   ```bash
   cd Inventory.Api
   dotnet run
   ```

3. **API runs on:** `http://localhost:5000`

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login and get JWT token | No |

### Products

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/products` | List all products | Yes |
| POST | `/api/products` | Create new product | Yes |

### Warehouses

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/warehouses` | List all warehouses | Yes |
| POST | `/api/warehouses` | Create new warehouse | Yes |

### Stock

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/stock` | Add stock to warehouse | Yes |
| GET | `/api/stock?productCode={code}` | Get stock for a product | Yes |
| GET | `/api/stock?warehouseCode={code}` | Get stock for a warehouse | Yes |

### Orders (Stock Transfers)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/orders` | Transfer stock between warehouses | Yes |

## API Usage Examples

### 1. Register a User

```bash
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!",
    "role": "Admin"
  }'
```

**Response:**
```json
{
  "message": "User registered successfully",
  "userId": 1
}
```

### 2. Login

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Create a Product (Authenticated)

```bash
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "code": "PROD001",
    "description": "Laptop Computer"
  }'
```

### 4. Create a Warehouse

```bash
curl -X POST http://localhost:8080/api/warehouses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "code": "WH001",
    "name": "Main Warehouse"
  }'
```

### 5. Add Stock

```bash
curl -X POST http://localhost:8080/api/stock \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "productCode": "PROD001",
    "warehouseCode": "WH001",
    "quantity": 100
  }'
```

### 6. Transfer Stock Between Warehouses

```bash
curl -X POST http://localhost:8080/api/orders \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "productCode": "PROD001",
    "sourceWarehouseCode": "WH001",
    "destinationWarehouseCode": "WH002",
    "quantity": 25
  }'
```

## Running Tests

```bash
cd Inventory.Tests
dotnet test
```

**Test Coverage:**
- ProductServiceTests (6 tests)
- WarehouseServiceTests (6 tests)
- StockServiceTests (6 tests)
- AuthServiceTests (3 tests)

**Total: 21 tests - All Passing ✅**

## Docker Commands

```bash
# Start containers
docker-compose up -d

# Stop containers
docker-compose down

# View API logs
docker logs inventoryassignment-inventory-api-1

# View database logs
docker logs inventory-db

# Rebuild and restart
docker-compose down
docker-compose build
docker-compose up -d
```

## Environment Variables

Configure in `docker-compose.yml` or `appsettings.json`:

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection string | SQL Server on port 1433 |
| `Jwt__Key` | JWT signing key | your-secret-key-here |
| `Jwt__Issuer` | JWT issuer | InventoryAPI |
| `Jwt__Audience` | JWT audience | InventoryAPIUsers |
| `SA_PASSWORD` | SQL Server SA password | YourStrong@Passw0rd |

## Business Logic

### Stock Transfer Validation

- ✅ Validates sufficient quantity in source warehouse
- ✅ Prevents transfers to the same warehouse (self-transfer)
- ✅ Atomic transactions ensure data integrity
- ✅ Returns appropriate HTTP status codes (400, 404, 409)

### Duplicate Prevention

- ✅ Product codes must be unique
- ✅ Warehouse codes must be unique
- ✅ Usernames must be unique

## Project Structure

```
Inventory.Api/
├── Controllers/          # API Controllers
├── DTOs/                 # Data Transfer Objects
├── Middleware/           # Custom Middleware
├── Program.cs            # Application entry point
└── Dockerfile            # Docker image definition

Inventory.Core/
└── Entities/             # Domain Models

Inventory.Infrastructure/
├── Data/                 # DbContext
└── Repositories/         # Data Access Layer

Inventory.Services/
├── Auth/                 # Authentication Service
├── Product/              # Product Service
├── Stock/                # Stock Service
├── Warehouse/            # Warehouse Service
└── Interfaces/           # Service Contracts

Inventory.Tests/
└── Services/             # Unit Tests
```

## Troubleshooting

### Issue: Database connection fails

**Solution:** Ensure SQL Server container is running and healthy:
```bash
docker logs inventory-db
```

### Issue: API returns 401 Unauthorized

**Solution:** Ensure you're including the JWT token in the Authorization header:
```
Authorization: Bearer YOUR_TOKEN_HERE
```

### Issue: Port 8080 already in use

**Solution:** Stop existing containers or change port in `docker-compose.yml`

## Next Steps

- [ ] Implement Angular UI with module-based architecture
- [ ] Add pagination for list endpoints
- [ ] Implement order history tracking
- [ ] Add audit logging
- [ ] Deploy to cloud provider (Azure/AWS)

---

**API Status:** ✅ Fully functional and deployed with Docker

**Author:** SCAD Software Technical Assessment
