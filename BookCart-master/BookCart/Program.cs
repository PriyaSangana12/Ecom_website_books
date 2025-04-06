using BookCart.DataAccess;
using BookCart.Interfaces;
using BookCart.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookCart API",
        Description = "An ASP.NET Core Web API for managing the book data",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Ankit Sharma",
            Url = new Uri("https://ankitsharmablogs.com/"),
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/AnkitSharma-007/BookCart/blob/master/LICENSE"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard JWT Authorization header. Example: \"bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Add Service injection here
builder.Services.AddTransient<IBookService, BookDataAccessLayer>();
builder.Services.AddTransient<ICartService, CartDataAccessLayer>();
builder.Services.AddTransient<IOrderService, OrderDataAccessLayer>();
builder.Services.AddTransient<IUserService, UserDataAccessLayer>();
builder.Services.AddTransient<IWishlistService, WishlistDataAccessLayer>();

builder.Services.AddDbContext<BookDBContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
        ClockSkew = TimeSpan.Zero // Override the default clock skew of 5 mins
    };
});

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
            "https://localhost:53424",
            "http://localhost:53424",
            "https://localhost:7073",
            "http://localhost:7073"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy(UserRoles.Admin, Policies.AdminPolicy());
    config.AddPolicy(UserRoles.User, Policies.UserPolicy());
});

var app = builder.Build();

// Database connection test
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDBContext>();
    try
    {
        Console.WriteLine("Testing database connection...");
        var canConnect = db.Database.CanConnect();
        Console.WriteLine($"Database connection successful: {canConnect}");
        if (canConnect)
        {
            var tables = db.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList();
            Console.WriteLine($"Tables found: {string.Join(", ", tables)}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}



// Apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDBContext>();
    db.Database.Migrate();
}

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        BookCart.Data.BookSeeder.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Add test endpoint before other middleware
app.MapGet("/test-db", async (BookDBContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        if (!canConnect)
        {
            return Results.Problem("Could not connect to database");
        }
        
        var tables = db.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList();
        if (!tables.Any())
        {
            return Results.Problem("No tables found in database");
        }

        return Results.Ok(new { 
            success = true, 
            message = "Database connection successful",
            tables = tables,
            connectionString = db.Database.GetConnectionString() 
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: $"Database error: {ex.Message}", statusCode: 500);
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add CORS middleware
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

app.UseAuthentication();
app.UseAuthorization();

// Add API controllers
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();