using animal_shelter_app.Data;
using animal_shelter_app.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка базы данных (Azure SQL)
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)
    )
);

// 2. Настройка JWT-аутентификации
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[]
    {
        "https://gladpet-web-app-hecufnfgf3g5ggcd.swedencentral-01.azurewebsites.net",
        "http://volodymyrstrohan-001-site1.dtempurl.com",
        "https://volodymyrstrohan-001-site1.dtempurl.com",
        "http://localhost:3000",
        "http://localhost:5173"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGladPet", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (args.Contains("--create-admin"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    string adminEmail = "admin@animalshelter.com";
    if (!context.Users.Any(u => u.Email == adminEmail))
    {
        var passwordHasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Email = adminEmail,
            FullName = "System Admin",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "AdminPassword123!");

        context.Users.Add(adminUser);
        context.SaveChanges();

        Console.WriteLine("\n[SUCCESS] Администратор успешно создан!");
        Console.WriteLine($"Email: {adminEmail}");
        Console.WriteLine("Пароль: AdminPassword123!\n");
    }
    else
    {
        Console.WriteLine("\n[INFO] Администратор с таким email уже существует в базе.\n");
    }

    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowGladPet");

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
