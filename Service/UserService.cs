using Azure;
using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using LuxoraStore.Model.DB;
using LuxoraStore.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace LuxoraStore.Service
{
    public class UserService : IUser
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ApplicationContext context, JwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }
        public async Task<bool> RegisterAsync(UserDTO userDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cek apakah user sudah ada
                if (await UserExistsAsync(userDTO.Username, userDTO.Email))
                    return false;

                // Hash password
                var passwordHash = HashPassword(userDTO.Password);

                // Buat user baru
                var user = new User
                {
                    Name = "",
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    Password = passwordHash,
                    Role = "Customer",
                    PhoneNumber = "",
                    Address = "",
                    Image = "",
                    UserStatus = GeneralStatusData.GeneralStatusDataAll.Published,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[RegisterAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat registrasi", ex);
            }
        }

        public async Task<string?> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                // Mencari user berdasarkan username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDTO.Username);

                if (user == null)
                    return null; // User tidak ditemukan

                // Verifikasi password
                if (!VerifyPassword(loginDTO.Password, user.Password))
                    return null; // Password salah

                // Generate JWT Token
                var token = _jwtHelper.GenerateToken(user.Username, user.Email, user.Id, user.Role);

                var httpContext = _httpContextAccessor.HttpContext;

                // Simpan token dan role di cookie
                httpContext?.Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(5) // Sesuaikan dengan expiry token JWT
                    //Expires = DateTime.Now.AddHours(1) // Sesuaikan dengan expiry token
                });

                httpContext?.Response.Cookies.Append("user_role", user.Role, new CookieOptions
                {
                    HttpOnly = false // Untuk diakses JS
                });


                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
        }

        public List<UserDTO> GetAllUser()
        {
            var data = _context.Users.Where(x => x.UserStatus != GeneralStatusData.GeneralStatusDataAll.delete && x.Id != 1).Select(x => new UserDTO
            {
                Id = x.Id,
                Name = x.Name,
                Username = x.Username,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                Password = "*******",
                UserStatus = x.UserStatus

            }).ToList();
            return data;
        }

        public User GetUserById(int id)
        {
            var data = _context.Users.Where(x => x.Id == id && x.UserStatus != GeneralStatusData.GeneralStatusDataAll.delete).FirstOrDefault();
            if (data == null)
            {
                return new User();
            }

            return data;
        }

        public bool DeleteUser(int id)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return false;
            }

            data.UserStatus = GeneralStatusData.GeneralStatusDataAll.delete;
            _context.SaveChanges();
            return true;
        }

        // Method untuk hash password menggunakan SHA256 (sederhana untuk contoh)
        // Untuk production, gunakan BCrypt atau Argon2

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }


    }
}
