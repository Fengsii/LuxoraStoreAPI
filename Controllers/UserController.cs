using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using LuxoraStore.Model.DTO;
using LuxoraStore.Model.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LuxoraStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;

        public UserController(IUser user, ApplicationContext context, JwtHelper jwtHelper)
        {
            _user = user;
            _context = context;
            _jwtHelper = jwtHelper;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }



        //[HttpPost]
        //public async Task<IActionResult> Register(UserDTO model)
        //{
        //    try
        //    {

        //        if (!ModelState.IsValid)
        //        {
        //            var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors));
        //            Console.WriteLine($"Validasi gagal: {errors}");
        //            TempData["Error"] = "Data tidak valid";
        //            return View(model);
        //        }

        //        Console.WriteLine("Memeriksa keberadaan user...");
        //        var userExists = await _user.UserExistsAsync(model.Username, model.Email);
        //        if (userExists)
        //        {
        //            Console.WriteLine("User sudah ada");
        //            TempData["Error"] = "Username/email sudah digunakan";
        //            return View(model);
        //        }


        //        Console.WriteLine("Membuat user baru...");
        //        var result = await _user.RegisterAsync(model);

        //        if (!result)
        //        {
        //            Console.WriteLine("Registrasi gagal (return false)");
        //            TempData["Error"] = "Registrasi gagal";
        //            return View(model);
        //        }

        //        Console.WriteLine("Registrasi berhasil");
        //        TempData["Success"] = "Registrasi berhasil! Silakan login.";
        //        return RedirectToAction("Login");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.ToString()}");
        //        TempData["Error"] = $"Gagal registrasi: {ex.Message}";
        //        return View(model);
        //    }
        //}


        //public IActionResult Login()
        //{
        //    // Cek token jika ada
        //    // Cek apakah user sudah punya token valid
        //    if (Request.Cookies.TryGetValue("jwt_token", out var token))
        //    {
        //        if (_jwtHelper.ValidateToken(token))
        //        {
        //            // Ambil role dari cookie
        //            if (Request.Cookies.TryGetValue("user_role", out var role))
        //            {
        //                return role == "Admin"
        //                    ? RedirectToAction("Index", "DashboardAdmin")
        //                    : RedirectToAction("Index", "DashboardUser");
        //            }
        //        }
        //    }

        //    // Jika tidak ada token/tidak valid, tampilkan halaman login
        //    return View();
        //}


        //[HttpPost]
        //public async Task<IActionResult> Login(LoginDTO model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            TempData["Error"] = "Data tidak valid";
        //            return View(model); // Kembali ke view dengan error
        //        }

        //        // Panggil service untuk login
        //        var token = await _user.LoginAsync(model);
        //        if (token == null)
        //        {
        //            TempData["Error"] = "Username/password salah";
        //            return View(model);
        //        }

        //        // Dapatkan user dari database (untuk role)
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        //        if (user == null)
        //        {
        //            TempData["Error"] = "User tidak ditemukan di database";
        //            return View(model);
        //        }

        //        // Simpan token dan role di cookie
        //        Response.Cookies.Append("jwt_token", token, new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict,
        //            Expires = DateTime.Now.AddMinutes(5) // Sesuaikan dengan expiry token JWT
        //            //Expires = DateTime.Now.AddHours(1) // Sesuaikan dengan expiry token
        //        });

        //        Response.Cookies.Append("user_role", user.Role, new CookieOptions
        //        {
        //            HttpOnly = false // Untuk diakses JS
        //        });

        //        // Redirect berdasarkan role
        //        return user.Role == "Admin"
        //            ? RedirectToAction("Index", "DashboardAdmin")
        //            : RedirectToAction("Index", "DashboardUser");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception saat login: {ex}");
        //        TempData["Error"] = $"Terjadi kesalahan saat login: {ex.Message}";
        //        return View(model);
        //    }
        //}

        //public IActionResult Logout()
        //{
        //    // Hapus cookie token
        //    Response.Cookies.Delete("jwt_token");

        //    // Hapus cookie role
        //    Response.Cookies.Delete("user_role");

        //    // Pesan berhasil logout
        //    TempData["Success"] = "Berhasil logout";

        //    // Kembali ke halaman login
        //    return RedirectToAction("Login");
        //}
    }
}
