using FluentValidation.Results;
using LuxoraStore.Helpers;
using LuxoraStore.Interfaces;
using LuxoraStore.Model;
using LuxoraStore.Model.DTO;
using LuxoraStore.Model.GeneralResponseStatus;
using LuxoraStore.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LuxoraStore.Controllers
{
    // Tambahkan ini ke atas controller untuk
    // mengamankan semua endpoint secara default
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ApplicationContext _context;
        private readonly IJwtHelper _jwtHelper;
        private ValidationResult _validationResult;
        private ValidationResult _loginValidator;

        public UserController(IUser user, ApplicationContext context, IJwtHelper jwtHelper)
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

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO dataEntry)
        {
            try
            {
                //Karena ValidatorRequestUser butuh akses ke database(_context) untuk menjalankan 
                //MustAsync(...) yang mengecek apakah Username dan Email sudah terdaftar.
                ValidatorRequestUser validations = new ValidatorRequestUser(_context);
                _validationResult = await validations.ValidateAsync(dataEntry);

                if (_validationResult.IsValid)
                {
                    var data = await _user.RegisterAsync(dataEntry);

                    if (data)
                    {
                        return Ok(new GeneralRespose
                        {
                            StatusCode = "01",
                            Statusdesc = "Registration successful!",
                            Data = dataEntry
                        }); // 200 OK
                    }

                    return StatusCode(500, new GeneralRespose
                    {
                        StatusCode = "2",
                        Statusdesc = "Failed to save user data.",
                        Data = dataEntry
                    }); // 500 Internal Server Error
                }

                // Jika error karena duplikat username/email → 409 Conflict
                if (_validationResult.Errors.Any(e =>
                    e.ErrorMessage.Contains("already registered", StringComparison.OrdinalIgnoreCase)))
                {
                    return Conflict(new GeneralRespose
                    {
                        StatusCode = "03",
                        Statusdesc = _validationResult.ToString(),
                        Data = dataEntry
                    }); // 409 Conflict
                }

                // Validasi umum lainnya → 400 Bad Request
                return BadRequest(new GeneralRespose
                {
                    StatusCode = "02",
                    Statusdesc = _validationResult.ToString(),
                    Data = dataEntry
                }); // 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralRespose
                {
                    StatusCode = "99",
                    Statusdesc = $"Failed | {ex.Message}",
                    Data = null
                }); // 500 Internal Server Error
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                LoginValidator validationRules = new LoginValidator();
                _loginValidator = validationRules.Validate(loginDTO);

                if (!_loginValidator.IsValid)
                {
                    return BadRequest(new GeneralRespose
                    {
                        StatusCode = "02",
                        Statusdesc = _loginValidator.ToString(),
                        Data = null
                    }); 
                }

                var token = await _user.LoginAsync(loginDTO);
                if (token != null)
                {
                    return Ok(new GeneralRespose
                    {
                        StatusCode = "01",
                        Statusdesc = "Login successful!",
                        Data = new { Token = token }
                    }); 
                }
                else
                {
                    return Unauthorized(new GeneralRespose
                    {
                        StatusCode = "401",
                        Statusdesc = "Incorrect username or password",
                        Data = null
                    }); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralRespose
                {
                    StatusCode = "500",
                    Statusdesc = $"Failed | {ex.Message}",
                    Data = null
                }); 
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUser([FromForm] UserDTO userDTO, IFormFile? image)
        {
            try
            {
                var result = await _user.UpdateUserProfile(userDTO, image);
                if (!result)
                {
                    return BadRequest(new GeneralResponse
                    {
                        StatusCode = "02",
                        Statusdesc = "Gagal update data user.",
                        Data = userDTO
                    });
                }

                return Ok(new GeneralResponse
                {
                    StatusCode = "01",
                    Statusdesc = "Berhasil update data user.",
                    Data = userDTO
                });
            }
            catch (InvalidOperationException ex)
            {
                // Error dari ImageHelper (validasi ukuran/format)
                return BadRequest(new GeneralResponse
                {
                    StatusCode = "03",
                    Statusdesc = $"Gagal upload gambar: {ex.Message}",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse
                {
                    StatusCode = "500",
                    Statusdesc = "Terjadi kesalahan internal server.",
                    Data = null
                });
            }
        }


        //============================ FORGOT PASSWORD ============================\\

        [AllowAnonymous]
        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request)
        {
            try
            {
                var result = await _user.SendOtpAsync(request.EmailOrUsername);
                if (result)
                {
                    return Ok(new GeneralResponse
                    {
                        StatusCode = "01",
                        Statusdesc = "OTP telah dikirim ke email kamu.",
                        Data = request.EmailOrUsername
                    });
                }

                return NotFound(new GeneralResponse
                {
                    StatusCode = "404",
                    Statusdesc = "User tidak ditemukan atau email tidak tersedia.",
                    Data = request.EmailOrUsername
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new GeneralResponse
                {
                    StatusCode = "500",
                    Statusdesc = "Terjadi kesalahan internal server. Silakan coba lagi nanti.",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(VerifyOtpAndResetPasswordDTO request)
        {
            try
            {
                var result = await _user.ResetPasswordWithOtpAsync(request);
                if (result)
                {
                    return Ok(new GeneralResponse
                    {
                        StatusCode = "01",
                        Statusdesc = "Password berhasil direset.",
                        Data = null
                    });
                }

                return BadRequest(new GeneralResponse
                {
                    StatusCode = "02",
                    Statusdesc = "OTP salah atau sudah kadaluarsa.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResetPassword Error] {ex.Message}");

                return StatusCode(500, new GeneralResponse
                {
                    StatusCode = "500",
                    Statusdesc = $"Terjadi kesalahan pada server: {ex.Message}",
                    Data = null
                });
            }
        }



        // ENDPOINT UNTUK CUSTOMER SAJA
        //[Authorize]
        [Authorize(Roles = "Customer")]
        [HttpGet("customer-only")]
        public IActionResult CustomerOnlyEndpoint()
        {
            return Ok(new
            {
                Message = "Halo Customer, ini endpoint khusus untukmu."
            });
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

    }
}
