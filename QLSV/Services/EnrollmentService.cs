using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ICsvService _csvService;
        private readonly ICourseService _courseService;
        private const string FileName = "enrollments.csv";

        public EnrollmentService(ICsvService csvService, ICourseService courseService)
        {
            _csvService = csvService;
            _courseService = courseService;
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _csvService.ReadAsync<Enrollment>(FileName);
        }

        public async Task<Enrollment?> GetByIdAsync(string id)
        {
            var enrollments = await GetAllAsync();
            return enrollments.FirstOrDefault(e => e.Id == id);
        }

        public async Task<List<Enrollment>> GetByStudentIdAsync(string studentId)
        {
            var enrollments = await GetAllAsync();
            return enrollments.Where(e => e.StudentId == studentId).ToList();
        }

        public async Task<List<Enrollment>> GetByCourseIdAsync(string courseId)
        {
            var enrollments = await GetAllAsync();
            return enrollments.Where(e => e.CourseId == courseId).ToList();
        }

        public async Task<Enrollment?> GetEnrollmentAsync(string studentId, string courseId)
        {
            var enrollments = await GetAllAsync();
            return enrollments.FirstOrDefault(e =>
                e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<Enrollment> EnrollAsync(string studentId, string courseId)
        {
            // Check if already enrolled
            var existing = await GetEnrollmentAsync(studentId, courseId);
            if (existing != null)
                throw new InvalidOperationException("Sinh viên đã đăng ký khóa học này");

            // Check course availability
            var course = await _courseService.GetByIdAsync(courseId);
            if (course == null)
                throw new InvalidOperationException("Khóa học không tồn tại");

            if (!course.IsActive)
                throw new InvalidOperationException("Khóa học đã đóng đăng ký");

            if (course.IsCompleted)
                throw new InvalidOperationException("Khóa học đã hoàn thành, không thể đăng ký");

            if (course.EnrolledStudents >= course.MaxStudents)
                throw new InvalidOperationException("Khóa học đã đầy");

            // Create new enrollment
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = "Active"
            };

            var enrollments = await GetAllAsync();
            enrollments.Add(enrollment);
            await _csvService.WriteAsync(FileName, enrollments);

            // Update course enrolled count
            course.EnrolledStudents++;
            await _courseService.UpdateAsync(course);

            return enrollment;
        }

        public async Task<bool> UnenrollAsync(string enrollmentId)
        {
            var enrollments = await GetAllAsync();
            var enrollment = enrollments.FirstOrDefault(e => e.Id == enrollmentId);
            if (enrollment == null) return false;

            var course = await _courseService.GetByIdAsync(enrollment.CourseId);
            if (course != null && course.EnrolledStudents > 0)
            {
                course.EnrolledStudents--;
                await _courseService.UpdateAsync(course);
            }

            enrollments.Remove(enrollment);
            await _csvService.WriteAsync(FileName, enrollments);
            return true;
        }

        public async Task<bool> IsEnrolledAsync(string studentId, string courseId)
        {
            var enrollment = await GetEnrollmentAsync(studentId, courseId);
            return enrollment != null;
        }

        // existing implemented method (keeps the same name as interface)
        public async Task UpdateStatusByCourseAsync(string courseId, bool isCompleted)
        {
            var enrollments = await GetAllAsync();
            bool changed = false;

            foreach (var e in enrollments)
            {
                if (e.CourseId == courseId)
                {
                    e.Status = isCompleted ? "Completed" : "Active";
                    changed = true;
                }
            }

            if (changed)
            {
                await _csvService.WriteAsync(FileName, enrollments);
            }
        }
    }
}
