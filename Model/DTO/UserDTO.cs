using static LuxoraStore.Model.GeneralStatusData;

namespace LuxoraStore.Model.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public IFormFile? Image { get; set; } 
        public GeneralStatusDataAll UserStatus { get; set; }
    }
}
