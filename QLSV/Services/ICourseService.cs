using StudentManagement.Models;

namespace StudentManagement.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(string id);
        Task<Course> CreateAsync(Course course);
        Task<bool> UpdateAsync(Course course);
        Task<bool> DeleteAsync(string id);
        Task<List<Course>> GetActiveCoursesAsync();
    }
}