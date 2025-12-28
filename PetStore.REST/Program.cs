using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetStore.Infrastructure;
using PetStore.Infrastructure.Models;
using PetStore.Infrastructure.Repository;
using PetStore.Common.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to support JWT authentication
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Get connection string from configuration (supports environment variables)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found. Set ConnectionStrings__DefaultConnection environment variable.");

// Register DbContext with PostgreSQL
builder.Services.AddDbContext<PetStoreContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PetStoreContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication (supports environment variables)
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? Environment.GetEnvironmentVariable("Jwt__Key")
    ?? throw new InvalidOperationException("JWT Key not found. Set Jwt__Key environment variable.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? Environment.GetEnvironmentVariable("Jwt__Issuer")
    ?? "PetStoreAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? Environment.GetEnvironmentVariable("Jwt__Audience")
    ?? "PetStoreClient";

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

// Register repositories
builder.Services.AddScoped<IRepository<DogModel>, EfRepository<DogModel>>();
builder.Services.AddScoped<IRepository<CustomerModel>, EfRepository<CustomerModel>>();

// Register repository adapters
builder.Services.AddScoped<IRepositoryAdapter<DogModel>>(sp =>
{
    var repository = sp.GetRequiredService<IRepository<DogModel>>();
    return new RepositoryAdapter<DogModel>(repository);
});

builder.Services.AddScoped<IRepositoryAdapter<CustomerModel>>(sp =>
{
    var repository = sp.GetRequiredService<IRepository<CustomerModel>>();
    return new RepositoryAdapter<CustomerModel>(repository);
});

// Register CRUD services
builder.Services.AddScoped<ICrudServiceAsync<DogModel>>(sp =>
{
    var adapter = sp.GetRequiredService<IRepositoryAdapter<DogModel>>();
    return new CrudServiceAsyncRepository<DogModel>(adapter);
});

builder.Services.AddScoped<ICrudServiceAsync<CustomerModel>>(sp =>
{
    var adapter = sp.GetRequiredService<IRepositoryAdapter<CustomerModel>>();
    return new CrudServiceAsyncRepository<CustomerModel>(adapter);
});

// Configure CORS (if needed for testing from different origins)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed database with roles and test users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PetStoreContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed roles
        string[] roles = { "Customer", "Employee", "Admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        
        // Seed test users (only in Development environment)
        if (builder.Environment.IsDevelopment())
        {
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin123!";
            var employeePassword = Environment.GetEnvironmentVariable("EMPLOYEE_PASSWORD") ?? "Employee123!";
            var customerPassword = Environment.GetEnvironmentVariable("CUSTOMER_PASSWORD") ?? "Customer123!";

            if (await userManager.FindByEmailAsync("admin@petstore.com") == null)
            {
                var adminUser = new User
                {
                    UserName = "admin@petstore.com",
                    Email = "admin@petstore.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            
            if (await userManager.FindByEmailAsync("employee@petstore.com") == null)
            {
                var employeeUser = new User
                {
                    UserName = "employee@petstore.com",
                    Email = "employee@petstore.com",
                    FirstName = "Employee",
                    LastName = "User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(employeeUser, employeePassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }
            
            if (await userManager.FindByEmailAsync("customer@petstore.com") == null)
            {
                var customerUser = new User
                {
                    UserName = "customer@petstore.com",
                    Email = "customer@petstore.com",
                    FirstName = "Customer",
                    LastName = "User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(customerUser, customerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Redirect root to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Get port from environment variable (Render sets PORT)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

