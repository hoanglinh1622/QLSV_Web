using StudentManagement.Models;

namespace StudentManagement.Services
{
    public interface IEnrollmentService
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(string id);
        Task<List<Enrollment>> GetByStudentIdAsync(string studentId);
        Task<List<Enrollment>> GetByCourseIdAsync(string courseId);
        Task<Enrollment?> GetEnrollmentAsync(string studentId, string courseId);
        Task<Enrollment> EnrollAsync(string studentId, string courseId);
        Task<bool> UnenrollAsync(string enrollmentId);
        Task<bool> IsEnrolledAsync(string studentId, string courseId);
        Task UpdateStatusByCourseAsync(string id, bool isCompleted);
    }
}