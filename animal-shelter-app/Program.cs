using animal_shelter_app.Data;
using animal_shelter_app.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка базы данных (Azure SQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// 3. Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGladPet", policy =>
    {
        policy.WithOrigins(
                "https://gladpet-web-app-hecufnfgf3g5ggcd.swedencentral-01.azurewebsites.net",
                "https://00fitkp3-7240.euw.devtunnels.ms", // Актуальный адрес тунеля
                "http://localhost:3000",
                "http://localhost:5173"
              )
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

// ВАЖНО: CORS должен быть в самом начале конвейера запросов
app.UseCors("AllowGladPet");

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

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();








//using animal_shelter_app.Data;
//using animal_shelter_app.Models;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// 1. Меняем SQLite на Azure SQL (SQL Server)
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2. Добавляем настройку JWT-аутентификации
//var jwtKey = builder.Configuration["Jwt:Key"]!;
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
//        };
//    });

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowGladPet", policy =>
//    {
//        policy.WithOrigins(
//                  "https://gladpet-web-app-hecufnfg3g5ggcd.swedencentral-01.azurewebsites.net",
//                  "https://00fltkp3-7240.euw.devtunnels.ms"
//              )
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//        Name = "Authorization",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();

//if (args.Contains("--create-admin"))
//{
//    using var scope = app.Services.CreateScope();
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    string adminEmail = "admin@animalshelter.com";
//    if (!context.Users.Any(u => u.Email == adminEmail))
//    {
//        var passwordHasher = new PasswordHasher<User>();

//        var adminUser = new User
//        {
//            Email = adminEmail,
//            FullName = "System Admin",
//            Role = UserRole.Admin,
//            CreatedAt = DateTime.UtcNow
//        };

//        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "AdminPassword123!");

//        context.Users.Add(adminUser);
//        context.SaveChanges();

//        Console.WriteLine("\n[SUCCESS] Администратор успешно создан!");
//        Console.WriteLine($"Email: {adminEmail}");
//        Console.WriteLine("Пароль: AdminPassword123!\n");
//    }
//    else
//    {
//        Console.WriteLine("\n[INFO] Администратор с таким email уже существует в базе.\n");
//    }

//    return;
//}

//// ВАЖНО: CORS должен стоять в самом начале пайплайна запросов
////app.UseCors("AllowGladPet");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// UseAuthentication должен идти строго ПЕРЕД UseAuthorization
//app.UseAuthentication();
//app.UseAuthorization();

//app.UseCors("AllowGladPet");

//app.MapControllers();
//app.Run();