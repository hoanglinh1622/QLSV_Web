using StudentManagement.Models;

namespace StudentManagement.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(string id);
        Task<Student?> GetByUserIdAsync(string userId);
        Task<Student> CreateAsync(Student student);
        Task<bool> UpdateAsync(Student student);
        Task<bool> DeleteAsync(string id);
        Task<List<Student>> SearchAsync(string keyword);
    }
}
