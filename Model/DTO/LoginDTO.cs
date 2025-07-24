namespace LuxoraStore.Model.DTO
{
    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; } // Store hashed password
        public string Email { get; set; }
    }
}
