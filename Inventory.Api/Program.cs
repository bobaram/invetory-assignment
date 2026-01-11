using Inventory.Api.Middleware;
using Inventory.Core.Interface.Repositories;
using Inventory.Core.Entities;
using Inventory.Services.Interfaces;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repositories;
using Inventory.Services.Auth;
using Inventory.Services.Product;
using Inventory.Services.Stock;
using Inventory.Services.Warehouse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=StrongP@ssw0rd!;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASuperSecretKeyForMySeniorDevTest123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "InventoryApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "InventoryClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Ensuring database is created...");
        db.Database.EnsureCreated();
        Console.WriteLine("Database created successfully.");

        if (!db.Products.Any())
        {
            Console.WriteLine("Seeding initial products...");
            db.Products.AddRange(
                new Product { Code = "LAPTOP001", Description = "Dell XPS 15 Laptop" },
                new Product { Code = "MOUSE001", Description = "Logitech MX Master 3 Mouse" },
                new Product { Code = "KEYBOARD001", Description = "Mechanical Gaming Keyboard" },
                new Product { Code = "MONITOR001", Description = "27-inch 4K Monitor" },
                new Product { Code = "HEADSET001", Description = "Wireless Noise-Cancelling Headset" },
                new Product { Code = "WEBCAM001", Description = "HD Webcam with Microphone" },
                new Product { Code = "PRINTER001", Description = "Color Laser Printer" },
                new Product { Code = "DESK001", Description = "Adjustable Standing Desk" },
                new Product { Code = "CHAIR001", Description = "Ergonomic Office Chair" },
                new Product { Code = "TABLET001", Description = "iPad Pro 12.9 inch" }
            );
            db.SaveChanges();
            Console.WriteLine("Seeded 10 dummy products successfully.");
        }

        if (!db.Warehouses.Any())
        {
            Console.WriteLine("Seeding initial warehouses...");
            db.Warehouses.AddRange(
                new Warehouse { Code = "WH-MAIN", Name = "Main Warehouse - New York" },
                new Warehouse { Code = "WH-WEST", Name = "West Coast Warehouse - Los Angeles" },
                new Warehouse { Code = "WH-SOUTH", Name = "Southern Warehouse - Atlanta" }
            );
            db.SaveChanges();
            Console.WriteLine("Seeded 3 warehouses successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Error: {ex.Message}");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<AuthenticationResponseMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();