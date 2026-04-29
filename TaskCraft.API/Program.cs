using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskCraft.API.Middleware;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Services;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;
using TaskCraft.Infrastructure.Repositories;
using TaskCraft.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration =====
var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

// ===== Database Configuration =====
builder.Services.AddDbContext<TaskCraftDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TaskCraft.Infrastructure")));

// ===== Dependency Injection - Repositories =====
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();

// ===== Dependency Injection - Services =====
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ===== FluentValidation =====
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<TaskCraft.Application.Validators.Auth.LoginRequestDtoValidator>();

// ===== JWT Authentication =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove delay of token expiration
    };
});

builder.Services.AddAuthorization();

// ===== CORS Configuration =====
// Cross-Origin Resource Sharing allows frontend applications from different origins to communicate with the API
// Configuration is environment-specific:
//   - Development: Allows localhost ports for React, Vite, Angular, Vue
//   - Production: Only allows specified production domains
// See: TaskCraft.API/Docs/CORS_GUIDE.md for detailed configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Read allowed origins from appsettings.json (environment-specific)
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:5173" }; // Fallback defaults

        policy.WithOrigins(allowedOrigins)  // Specific origins (secure)
              .AllowAnyMethod()             // GET, POST, PUT, DELETE, PATCH, etc.
              .AllowAnyHeader()             // Content-Type, Authorization, etc.
              .AllowCredentials();          // Allows cookies and auth headers
    });
});

// ===== Controllers =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ===== Swagger/OpenAPI Configuration =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API Info
    options.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "TaskCraft API",
        Description = @"A comprehensive Task Management API built with ASP.NET Core.

**Authentication:** Use /api/auth/login to get a JWT token. Then add it to requests with the Authorization header: `Bearer {your-token}`",
        Contact = new() { Name = "TaskCraft Team", Email = "support@taskcraft.com" },
        License = new() { Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT") }
    });

    // Enable XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ===== Build Application =====
var app = builder.Build();

// ===== Middleware Pipeline =====

// Global Error Handling (must be one of the first middleware)
app.UseErrorHandling();

// Swagger (Development & Staging)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskCraft API V1");
        options.RoutePrefix = string.Empty; // Swagger UI at root (https://localhost:port/)
        options.DocumentTitle = "TaskCraft API Documentation";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS - Must be before Authentication/Authorization
// Handles preflight (OPTIONS) requests and adds CORS headers to responses
app.UseCors("AllowReactApp");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// ===== Database Migration (Optional - Auto-apply migrations) =====
// Uncomment the following lines to automatically apply migrations on startup
/*
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskCraftDbContext>();
    dbContext.Database.Migrate();
}
*/

// ===== Run Application =====
app.Run();
