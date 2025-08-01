using static LuxoraStore.Model.GeneralStatusData;

namespace LuxoraStore.Model.DTO
{
    public class UserViewDTO
    {
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty; // path gambar
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = "*******"; // hidden
        public GeneralStatusDataAll UserStatus { get; set; }
    }
}
