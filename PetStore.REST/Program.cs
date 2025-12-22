using PetStore.Infrastructure;
using PetStore.Infrastructure.Models;
using PetStore.Infrastructure.Repository;
using PetStore.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get MongoDB connection string from configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") 
    ?? throw new InvalidOperationException("MongoDB connection string not found");

// Register PetStoreContext as singleton
builder.Services.AddSingleton<PetStoreContext>(sp => new PetStoreContext(mongoConnectionString));

// Register repositories
builder.Services.AddScoped<IRepository<DogModel>>(sp =>
{
    var context = sp.GetRequiredService<PetStoreContext>();
    return new MongoRepository<DogModel>(context.Dogs);
});

builder.Services.AddScoped<IRepository<CustomerModel>>(sp =>
{
    var context = sp.GetRequiredService<PetStoreContext>();
    return new MongoRepository<CustomerModel>(context.Customers);
});

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

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Redirect root to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

