using LuxoraStore.Model.DTO;

namespace LuxoraStore.Interfaces
{
    public interface IUser
    {
        Task<string?> LoginAsync(LoginDTO loginDTO);
        Task<bool> RegisterAsync(UserDTO userDTO);
        Task<bool> UserExistsAsync(string username, string email);
    }
}
