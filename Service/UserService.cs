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
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        public UserService(ApplicationContext context, IJwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor, IEmailHelper emailHelper, ILogger<UserService> logger, IImageHelper imageHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
            _emailHelper = emailHelper;
            _logger = logger;
            _imageHelper = imageHelper;
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

                var passwordHash = HashPassword(userDTO.Password);

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
                _logger.LogError(ex, "Terjadi kesalahan saat melakukan registrasi pengguna.");
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
                    return null; 

                // Verifikasi password
                if (!VerifyPassword(loginDTO.Password, user.Password))
                    return null; // Password salah

                var token = _jwtHelper.GenerateToken(user.Username, user.Email, user.Id, user.Role);

                var httpContext = _httpContextAccessor.HttpContext;

                // Simpan token dan role di cookie
                httpContext?.Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(5)
                });

                httpContext?.Response.Cookies.Append("user_role", user.Role, new CookieOptions
                {
                    HttpOnly = false // Untuk diakses JS
                });

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat proses login pengguna.");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
        }

        public async Task<bool> SendOtpAsync(string emailOrUsername)
        {
            try
            {
                emailOrUsername = emailOrUsername.Trim().ToLower();
                Console.WriteLine($"[SEND OTP] EmailOrUsername masuk: '{emailOrUsername}'");

                var user = await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Email.ToLower() == emailOrUsername || x.Username.ToLower() == emailOrUsername);

                //if (user == null || string.IsNullOrEmpty(user.Email))
                //    return false;

                if (user == null)
                {
                    Console.WriteLine("[SEND OTP] User tidak ditemukan di DB.");
                    return false;
                }
                else
                {
                    Console.WriteLine($"[SEND OTP] User ditemukan: {user.Username}, {user.Email}");
                }



                // Hapus OTP lama jika sudah expired
                if (user.OtpExpiredAt != null && user.OtpExpiredAt < DateTime.Now)
                {
                    user.OtpCode = null;
                    user.OtpExpiredAt = null;
                }

                // Generate OTP baru
                var otp = new Random().Next(100000, 999999).ToString();
                user.OtpCode = otp;
                user.OtpExpiredAt = DateTime.Now.AddMinutes(10);

                await _context.SaveChangesAsync();

                // Kirim OTP ke email user
                var sent = await _emailHelper.SendOtpEmailAsync(user.Email, otp);
                return sent;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan OTP ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mengirim OTP.");
                return false;
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(VerifyOtpAndResetPasswordDTO dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    (u.Email.ToLower() == dto.EmailOrUsername.ToLower() || u.Username.ToLower() == dto.EmailOrUsername.ToLower())
                    && u.UserStatus != GeneralStatusData.GeneralStatusDataAll.delete);

                if (user == null)
                    return false;

                if (user.OtpCode != dto.OtpCode)
                    return false;

                if (user.OtpExpiredAt == null || user.OtpExpiredAt < DateTime.Now)
                    return false;

                user.Password = HashPassword(dto.NewPassword);

                // Hapus OTP setelah berhasil reset password
                user.OtpCode = null;
                user.OtpExpiredAt = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan password baru ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mereset password.");
                return false;
            }
        }


        public async Task<bool> UpdateUserProfile(UserDTO userDTO, IFormFile? image)
        {
            try
            {
                var user = await _context.Users.FindAsync(userDTO.Id);
                if (user == null) return false;

                // Update data umum
                user.Name = userDTO.Name;
                user.Username = userDTO.Username;
                user.Email = userDTO.Email;
                user.PhoneNumber = userDTO.PhoneNumber;
                user.Address = userDTO.Address;
                user.UserStatus = GeneralStatusData.GeneralStatusDataAll.Published;
                user.UpdatedAt = DateTime.UtcNow;

                // Hash password jika tidak kosong
                if (!string.IsNullOrEmpty(userDTO.Password))
                {
                    user.Password = HashPassword(userDTO.Password);
                }

                // ✅ Simpan gambar baru jika dikirim
                if (image != null)
                {
                    // Hapus gambar lama kalau ada
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        _imageHelper.Delete(user.Image);
                    }

                    // Simpan gambar baru
                    user.Image = _imageHelper.Save(image, "user");
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Upluad gambar gaga.l");
                throw new Exception($"Upload gambar gagal: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat update user.");
                throw new Exception("Terjadi kesalahan saat update user.", ex);
            }
        }




        public List<UserViewDTO> GetAllUser()
        {
            var data = _context.Users.Where(x => x.UserStatus != GeneralStatusData.GeneralStatusDataAll.delete && x.Id != 1).Select(x => new UserViewDTO
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
