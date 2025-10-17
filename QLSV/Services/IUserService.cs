using StudentManagement.Models;

namespace StudentManagement.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string password, string role = "Student");
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameAsync(string username);
    }
}