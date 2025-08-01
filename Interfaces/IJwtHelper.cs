namespace LuxoraStore.Interfaces
{
    public interface IJwtHelper
    {
        public string GenerateToken(string username, string email, int userId, string role);
        public bool ValidateToken(string token);
    }
}
