using animal_shelter_app.Data;
using animal_shelter_app.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Меняем SQLite на Azure SQL (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Добавляем настройку JWT-аутентификации
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

// Add services to the container.
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

// admin@animalshelter.com
// AdminPassword123!
var app = builder.Build();

if (args.Contains("--create-admin"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Проверяем, есть ли уже админ с таким email
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

        // Хэшируем надежный пароль
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

    return; // Завершаем работу программы после создания, чтобы сервер не запускался в режиме веб-приложения
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. ВАЖНО: UseAuthentication должен идти строго ПЕРЕД UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();