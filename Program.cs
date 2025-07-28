using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using LuxoraStore.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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





//Penempatan Config Bearer JWT
//Agar bisa klik ?? Authorize dan isi token di Swagger, 
//kamu bisa tambahkan konfigurasi Bearer seperti ini:
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "LuxoraStore API", Version = "v1" });

    // Tambahkan definisi security schema Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan hanya token-nya saja. Contoh: eyJhbGciOiJIUzI1NiIs...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // Ganti dari ApiKey ke Http
        Scheme = "Bearer",              // Harus Bearer (huruf besar kecil penting!)
        BearerFormat = "JWT"
    });


    // Tambahkan aturan security ke semua endpoint
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});
















// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ? Tambahkan ini sebelum UseAuthorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
