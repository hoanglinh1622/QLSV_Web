using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICsvService _csvService;
        private const string FileName = "courses.csv";

        public CourseService(ICsvService csvService)
        {
            _csvService = csvService;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _csvService.ReadAsync<Course>(FileName);
        }

        public async Task<Course?> GetByIdAsync(string id)
        {
            var courses = await GetAllAsync();
            return courses.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Course> CreateAsync(Course course)
        {
            var courses = await GetAllAsync();
            courses.Add(course);
            await _csvService.WriteAsync(FileName, courses);
            return course;
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            var courses = await GetAllAsync();
            var existing = courses.FirstOrDefault(c => c.Id == course.Id);

            if (existing == null) return false;

            courses.Remove(existing);
            courses.Add(course);
            await _csvService.WriteAsync(FileName, courses);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var courses = await GetAllAsync();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null) return false;

            courses.Remove(course);
            await _csvService.WriteAsync(FileName, courses);
            return true;
        }

        public async Task<List<Course>> GetActiveCoursesAsync()
        {
            var courses = await GetAllAsync();
            return courses.Where(c => c.IsActive).ToList();
        }
    }
}