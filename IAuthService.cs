using LibraryManagement.Models;
namespace LibraryManagement.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string username, string email, string password);
        Task<string?> LoginAsync(string username, string password);
    }
}
