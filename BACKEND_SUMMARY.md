# Inventory Management API - Backend Implementation

## Architecture Overview

### Clean Architecture Layers
- **Inventory.Core**: Domain entities (Product, Warehouse, Stock, User)
- **Inventory.Core.Interface**: Interfaces for repositories and services
- **Inventory.Infrastructure**: Data access, DbContext, Repositories, UnitOfWork
- **Inventory.Services**: Business logic (StockService, AuthService)
- **Inventory.Api**: API controllers, middleware, DTOs

## Key Features Implemented

### 1. Authentication & Authorization (JWT)
- **AuthService**: Handles user registration and login with BCrypt password hashing
- **JWT Token Generation**: 8-hour token expiration
- **Middleware-based Auth**: `AuthenticationResponseMiddleware` provides consistent 401/403 responses
- All endpoints except `/auth/register` and `/auth/login` require authentication

### 2. Global Exception Handling
- **ExceptionHandlingMiddleware**: Centralized error handling
  - `KeyNotFoundException` → 404 Not Found
  - `ArgumentException` / `InvalidOperationException` → 400 Bad Request
  - `DbUpdateException` (duplicate keys) → 409 Conflict
  - `UnauthorizedAccessException` → 401 Unauthorized
  - All others → 500 Internal Server Error
- Controllers are clean - no try-catch blocks needed

### 3. API Endpoints

#### Auth Controller (No Auth Required)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and receive JWT token

#### Products Controller (Auth Required)
- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product (validates unique code)

#### Warehouses Controller (Auth Required)
- `GET /api/warehouses` - List all warehouses
- `GET /api/warehouses/{id}` - Get warehouse by ID
- `POST /api/warehouses` - Create new warehouse (validates unique code)

#### Stock Controller (Auth Required)
- `POST /api/stock` - Add initial stock or increase quantity
- `GET /api/stock?productCode={code}` - Get stock for a product across warehouses
- `GET /api/stock?warehouseCode={code}` - Get all stock in a warehouse

#### Orders Controller (Auth Required)
- `POST /api/orders` - Transfer stock between warehouses
  - Validates source != destination
  - Checks sufficient stock availability
  - Uses database transactions for atomicity

### 4. Data Validation
- DTOs use Data Annotations for input validation
- Required fields, MaxLength constraints, Range validation
- Database enforces unique constraints on codes

### 5. Repository Pattern with Unit of Work
- Generic repository for CRUD operations
- Custom `StockRepository` with eager loading of navigation properties
- UnitOfWork manages transactions and repository instances
- Database transactions for stock transfers

### 6. Database
- **SQL Server** with Entity Framework Core
- Automatic migrations on startup
- Composite key for Stock (ProductId, WarehouseId)
- Unique indexes on Product.Code, Warehouse.Code, User.Username

## Docker Configuration

### Services
1. **inventory-db**: SQL Server 2022
   - Port: 1433
   - Database: InventoryDb
   
2. **inventory-api**: .NET 9.0 API
   - Port: 8080
   - Auto-connects to database
   - Environment variables for connection string and JWT config

### Running with Docker
```bash
docker-compose up -d
```

## Configuration (appsettings.json)
- Connection string for SQL Server
- JWT settings (Key, Issuer, Audience)
- CORS enabled for frontend integration

## Testing the API

### 1. Register a User
```bash
POST /api/auth/register
{
  "username": "admin",
  "password": "Admin123!",
  "role": "Admin"
}
```

### 2. Login
```bash
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin123!"
}
```
Response includes JWT token

### 3. Create Product (with token)
```bash
POST /api/products
Authorization: Bearer {token}
{
  "code": "PROD001",
  "description": "Sample Product"
}
```

### 4. Create Warehouse
```bash
POST /api/warehouses
Authorization: Bearer {token}
{
  "code": "WH001",
  "name": "Main Warehouse"
}
```

### 5. Add Stock
```bash
POST /api/stock
Authorization: Bearer {token}
{
  "productCode": "PROD001",
  "warehouseCode": "WH001",
  "quantity": 100
}
```

### 6. Transfer Stock
```bash
POST /api/orders
Authorization: Bearer {token}
{
  "productCode": "PROD001",
  "sourceWarehouseCode": "WH001",
  "destinationWarehouseCode": "WH002",
  "quantity": 50
}
```

## Senior-Level Features
✅ JWT Authentication with secure password hashing
✅ Middleware for exception handling and auth responses
✅ Repository pattern with Unit of Work
✅ Database transactions for data integrity
✅ Input validation with Data Annotations
✅ Clean Architecture with proper separation of concerns
✅ Docker containerization
✅ CORS support for frontend
✅ Swagger for API documentation
✅ Async/await throughout
✅ Dependency injection
✅ Navigation property eager loading for optimal queries

## Next Steps
- Unit tests (xUnit) for business logic
- Angular UI with module-based architecture
- Pagination for list endpoints
- Additional authorization policies (role-based)
