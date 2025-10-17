using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class UserService : IUserService
    {
        private readonly ICsvService _csvService;
        private const string FileName = "users.csv";

        public UserService(ICsvService csvService)
        {
            _csvService = csvService;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var users = await _csvService.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);
        }

        public async Task<User?> RegisterAsync(string username, string password, string role = "Student")
        {
            var users = await _csvService.ReadAsync<User>(FileName);

            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            users.Add(newUser);
            await _csvService.WriteAsync(FileName, users);
            return newUser;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var users = await _csvService.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var users = await _csvService.ReadAsync<User>(FileName);
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
