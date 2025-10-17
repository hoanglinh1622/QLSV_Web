using StudentManagement.Models;

namespace StudentManagement.Services
{
    public class StudentService : IStudentService
    {
        private readonly ICsvService _csvService;
        private const string FileName = "students.csv";

        public StudentService(ICsvService csvService)
        {
            _csvService = csvService;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _csvService.ReadAsync<Student>(FileName);
        }

        public async Task<Student?> GetByIdAsync(string id)
        {
            var students = await GetAllAsync();
            return students.FirstOrDefault(s => s.Id == id);
        }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            var students = await GetAllAsync();
            return students.FirstOrDefault(s => s.UserId == userId);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            var students = await GetAllAsync();
            students.Add(student);
            await _csvService.WriteAsync(FileName, students);
            return student;
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            var students = await GetAllAsync();
            var existing = students.FirstOrDefault(s => s.Id == student.Id);

            if (existing == null) return false;

            students.Remove(existing);
            students.Add(student);
            await _csvService.WriteAsync(FileName, students);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var students = await GetAllAsync();
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null) return false;

            students.Remove(student);
            await _csvService.WriteAsync(FileName, students);
            return true;
        }

        public async Task<List<Student>> SearchAsync(string keyword)
        {
            var students = await GetAllAsync();
            return students.Where(s =>
                s.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                s.StudentCode.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}
