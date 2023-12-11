using ChatDemoAPI.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using ChatDemoAPI.Services.Interfaces;
using ChatDemoAPI.Services;

// Create a new web application builder.
var builder = WebApplication.CreateBuilder(args);

// Add controllers to the service collection.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Add API explorer endpoints.
builder.Services.AddEndpointsApiExplorer();

// Add Swagger documentation generation.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Chat demo API",
        Description = "An ASP.NET Core Web API for managing SQL vector database with WhatsApp interface.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

// Add CORS policy to allow all origins, headers, and methods.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Get the SQL connection string from the configuration.
var connectionString = builder.Configuration.GetConnectionString("SQLConnectionString");

// Add the database context to the service collection.
builder.Services.AddDbContext<ChatDemoAPIContext>(x => x.UseSqlServer(connectionString));

// Add the chat service to the service collection.
builder.Services.AddScoped<IChatService, ChatService>();

// Add the conversation to the service collection.
builder.Services.AddScoped<IConversation, ConversationService>();

// Build the application.
var app = builder.Build();

// Enable Swagger and Swagger UI.
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS with the "AllowAllOrigins" policy.
app.UseCors("AllowAllOrigins");

// Migrate the database on application startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatDemoAPIContext>();
    db.Database.Migrate();
}

// Enable routing.
app.UseRouting();

// Enable HTTPS redirection.
app.UseHttpsRedirection();

// Enable authorization.
app.UseAuthorization();

// Map controllers.
app.MapControllers();

// Run the application.
app.Run();