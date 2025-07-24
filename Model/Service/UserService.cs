using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model.DB;
using LuxoraStore.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace LuxoraStore.Model.Service
{
    public class UserService : IUser
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        public UserService(ApplicationContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
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
                var token = _jwtHelper.GenerateToken(user.Username, user.Email, user.Id);
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
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
