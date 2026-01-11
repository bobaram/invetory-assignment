# ROLE-BASED AUTHORIZATION SUMMARY

## User Roles

### ADMIN Role
**Credentials:**
- Username: admin / Password: Admin123!
- Username: superadmin / Password: SuperAdmin123!

**Permissions (Full Access):**
✅ View Products (GET /products)
✅ View Warehouses (GET /warehouses)
✅ View Stock (GET /stock)
✅ **Create Products** (POST /products) - ADMIN ONLY
✅ **Create Warehouses** (POST /warehouses) - ADMIN ONLY
✅ **Add Stock** (POST /stock) - ADMIN ONLY
✅ **Transfer Stock** (POST /orders) - ADMIN ONLY

---

### USER Role (Regular User)
**Credentials:**
- Username: user / Password: User123!

**Permissions (Read-Only):**
✅ View Products (GET /products)
✅ View Warehouses (GET /warehouses)
✅ View Stock (GET /stock)
❌ Create Products - **FORBIDDEN** (403 error)
❌ Create Warehouses - **FORBIDDEN** (403 error)
❌ Add Stock - **FORBIDDEN** (403 error)
❌ Transfer Stock - **FORBIDDEN** (403 error)

---

## Implementation

### Backend (API Controllers)
Role restrictions enforced using \[Authorize(Roles = "Admin")]\ attribute:

1. **ProductsController**
   - POST /products → Admin only

2. **WarehousesController**
   - POST /warehouses → Admin only

3. **StockController**
   - POST /stock → Admin only

4. **OrdersController**
   - Entire controller → Admin only

### Frontend (Angular UI)
The UI currently does NOT hide buttons based on role. Both Admin and User will see all buttons, but:
- Users will get **403 Forbidden** errors when trying to create/modify
- Admins can perform all operations

---

## Testing Authorization

### Test as Admin:
1. Login: http://localhost:4200/auth/login
   - Username: admin
   - Password: Admin123!
2. You can create products, warehouses, add stock, and transfer stock

### Test as User:
1. Login: http://localhost:4200/auth/login
   - Username: user
   - Password: User123!
2. You can ONLY view products, warehouses, and stock
3. Attempting to create/modify will show error: "Failed to create..." (403 Forbidden)

---

## Summary

**ADMIN = Full Control (Create, Read)**
**USER = Read-Only Access**

This is standard enterprise role-based authorization!
