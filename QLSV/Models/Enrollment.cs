// Models/Enrollment.cs
namespace StudentManagement.Models
{
    public class Enrollment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StudentId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Active"; // Active, Completed, Dropped
        public double? Grade { get; set; } // Điểm số (nullable)
        public string? Notes { get; set; } // Ghi chú
    }
}