using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using EnterpriseCrudApp.Infrastructure.Data;
using EnterpriseCrudApp.Infrastructure.UnitOfWork;
using EnterpriseCrudApp.Domain.Interfaces;
using EnterpriseCrudApp.Application.Services;
using EnterpriseCrudApp.Application.Interfaces;
using EnterpriseCrudApp.Application.Mappings;
using EnterpriseCrudApp.Application.Validators;
using System.Text.Json.Serialization;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase
    });

// Entity Framework Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EnterpriseCrudAppDb"));

// AutoMapper Configuration - manual configuration to avoid ambiguity
var mapperConfiguration = new MapperConfiguration(config =>
{
    config.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(mapperConfiguration);
builder.Services.AddSingleton<IMapper>(provider => new Mapper(mapperConfiguration));

// FluentValidation Configuration
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Enterprise CRUD API",
        Version = "v1",
        Description = "A comprehensive Employee Management API built with .NET 9, Clean Architecture, and best practices",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Enterprise Development Team",
            Email = "dev@enterprise.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enterprise CRUD API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Welcome endpoint
app.MapGet("/", () => new
{
    Message = "Welcome to Enterprise CRUD API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    SwaggerUI = "/swagger",
    HealthCheck = "/health"
}).WithTags("Info").WithName("GetApiInfo");

app.Run();
