using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using LuxoraStore.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLconnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQLconnection"))
    )
);

// Konfigurasi JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// Validasi apakah secret key ada
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT Secret Key tidak ditemukan di appsettings.json");
}

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero // Tidak ada toleransi waktu
    };
});

// Registrasi Services
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IUser, UserService>();
//Untuk bisa menyetel cookie dari service, kamu perlu mengakses HttpContext, 
//dan cara paling umum adalah menyuntikkan IHttpContextAccessor ke dalam service kamu.
builder.Services.AddHttpContextAccessor(); // Untuk mengakses HttpContext




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
