using clockapi.Auth;
using ClockApi.Contexts;
using ClockApi.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load("clockapi.env");

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClockApi", Version = "v1" });

    // Add support for file uploads
    c.OperationFilter<FileUploadOperationFilter>();
});

// Configure database contexts
builder.Services.AddDbContext<ApiContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentException("Connection string 'DefaultConnection' is null or empty.");
    }
    options.UseSqlite(connectionString);
    Console.WriteLine($"Using database: {connectionString}");
});
builder.Services.AddDbContext<AuthContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AdditionalConnection")));

// Add JWT authentication
builder.Services.AddJwtAuthentication();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.Requirements.Add(new RoleRequirement("Admin")));
    options.AddPolicy("EditorPolicy", policy =>
        policy.Requirements.Add(new RoleRequirement("Editor")));
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddScoped<AuthenticationService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:5175")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            policy.WithOrigins("https://www.codeweb.no")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    });
});

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000);
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps("/etc/ssl/certfile.pfx", Env.GetString("CertPassword"));
        });
    });
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps("certfile.pfx", Env.GetString("CertPassword"));
        });
    });
}

var app = builder.Build();

app.UseCors("AllowLocalhost");

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider("/var/www/clockapi/wwwroot"),
        RequestPath = "/images/blog",
        ServeUnknownFileTypes = true,
        DefaultContentType = "image/jpeg",
        ContentTypeProvider = new FileExtensionContentTypeProvider
        {
            Mappings = { [".jpg"] = "image/jpeg", [".png"] = "image/png" }
        }
    });
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClockApi v1");
    c.RoutePrefix = string.Empty;
});

app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; connect-src 'self' https://91.192.221.9:5001; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; img-src 'self' data: https://91.192.221.9:5001;";
    await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();